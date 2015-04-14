using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainConstant;
using DomainInterface;
//using DomainObject;
using IoC;
using SQLitePCL.Ugly;
using Xamarin.Forms;

namespace FourthFnB
{
    public partial class HomePage : ContentPage, INotifyPropertyChanged
    {
        private bool isBusy;
        
        public string AccessToken
        {
            get { return Application.Current.Properties[ApplicationProperties.AccessToken].ToString(); }
        }
        private IFnBWebAPI fnbWebAPI;
        private IDatabase database;
        private IStockRepository stockRepository;
        private IStockCountRepository stockCountRepository;
        private IStockItemSizeRepository stockItemSizeRepository;
        private IStockCountItemRepository stockCountItemRepository;
        private IStockItemSizeBarcodeRepository stockItemSizeBarcodeRepository;
        private IEnumerable<ISite> siteList;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsBusy
        {
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this,
                            new PropertyChangedEventArgs("IsBusy"));
                    }
                }
            }
            get { return isBusy; }
        }

        public HomePage(IEnumerable<ISite> sList)
        {
            InitializeComponent();
            this.BindingContext = this;
            this.IsBusy = false;
            
            database = IoCContainer.Container.Resolve(typeof(IDatabase), IoCInstanceConstant.StockDatabaseInstance) as IDatabase;
            stockRepository = IoCContainer.Container.Resolve(typeof(IStockRepository), null) as IStockRepository;
            stockCountRepository = IoCContainer.Container.Resolve(typeof(IStockCountRepository), null) as IStockCountRepository;
            stockItemSizeRepository = IoCContainer.Container.Resolve(typeof(IStockItemSizeRepository), null) as IStockItemSizeRepository;
            stockCountItemRepository = IoCContainer.Container.Resolve(typeof(IStockCountItemRepository), null) as IStockCountItemRepository;
            stockItemSizeBarcodeRepository = IoCContainer.Container.Resolve(typeof(IStockItemSizeBarcodeRepository), null) as IStockItemSizeBarcodeRepository;
            stockRepository.db = database;
            stockCountRepository.db = database;
            stockItemSizeRepository.db = database;
            stockCountItemRepository.db = database;
            stockItemSizeBarcodeRepository.db = database;
            this.fnbWebAPI = IoCContainer.Container.Resolve(typeof(IFnBWebAPI), IoCInstanceConstant.WebAPIInstance) as IFnBWebAPI;
            this.siteList = sList;
            InitializeSitePicker();
            UpdateLabel();

            MessagingCenter.Subscribe<IBarcodeScanner, IBarcodeResult>(this, "Barcode",
            async (barcodeSender, arg) =>
            {
                barcodeLbl.Text = "Format: " + arg.Format.ToString() + " \n Barcode Text: " + arg.Text;

                IStockCountItem searchResult = this.stockCountItemRepository.GetStockCountItemFromBarcode(arg.Text, arg.Format.ToString());

                if (searchResult != null)
                {
                    searchResult.StockItemSizes =
                        this.stockItemSizeRepository.GetStockItemSizeByStockItemId(searchResult.StockItemId);
                    searchResult.Count = this.stockCountRepository.GetStockCountBySiteItemId(searchResult.SiteItemId);

                    try
                    {
                        var test = new StockCountEntry();
                        test.BindingContext = searchResult;

                        await Navigation.PushAsync(test);
                    }
                    catch (Exception ex)
                    {
                        string test = ex.Message;
                    }
                }

            });
        }

        private void InitializeSitePicker()
        {
            if (siteList != null && siteList.Count() >= 1)
            {
                foreach (ISite site in siteList)
                {
                    sitePkr.Items.Add(site.UnitSiteName);
                }

                sitePkr.SelectedIndex = 0;
            }
            else
            {
                sitePkr.IsVisible = false;
            }
        }

        public void UpdateLabel()
        {
            IEnumerable<IStock> stockList = stockRepository.GetStocks();
            IStock s = null;

            if (stockList != null && stockList.Any())
            {
                s = stockList.ElementAt(0);
                s.Name = "Retrieve from DB: " + s.Name;
            }
            else
            {
                s = IoCContainer.Container.Resolve(typeof(IStock), null) as IStock;
                s.Name = "New Stock in DB";

                stockRepository.SaveStock(s);

                stockList = stockRepository.GetStocks();

                s = stockList.ElementAt(0);

                s.Name = "Create to DB: " + s.Name;
            }


            this.accessTokenLbl.Text = s.Name + " " + AccessToken;
            
        }

        async void OnStockCountBarcodeButtonClicked(object sender, EventArgs args)
        {
            // Take the picture
            IBarcodeScanner pictureTake =
                DependencyService.Get<IBarcodeScanner>();
            pictureTake.ScanBarcode();
        }

        private async void OnStockCountSearchButtonClicked(object sender, EventArgs args)
        {
            IEnumerable<IStockCountItem> searchResult = this.stockCountItemRepository.GetStockCountItemFromName("Cask");

            if (searchResult != null)
            {

                var cpage = new CarouselPage();
                cpage.Title = "Stock Count";

                foreach (IStockCountItem s in searchResult)
                {
                    s.StockItemSizes = this.stockItemSizeRepository.GetStockItemSizeByStockItemId(s.StockItemId);
                    s.Count = this.stockCountRepository.GetStockCountBySiteItemId(s.SiteItemId);

                    var stockCountEntry = new StockCountEntry();
                    stockCountEntry.BindingContext = s;
                    cpage.Children.Add(stockCountEntry);
                }
        
                try
                {
                    await Navigation.PushAsync(cpage);
                }
                catch (Exception ex)
                {
                    string test = ex.Message;
                }
            }
        }

        async void OnSiteSelectedIndexChanged(object sender, EventArgs args)
        {
            this.IsBusy = true;

            int siteId = this.siteList.ElementAt(sitePkr.SelectedIndex).SiteId;
            Application.Current.Properties[ApplicationProperties.Site] = siteId;

            IStockPeriodHeader sph = await this.fnbWebAPI.GetCurrentStockPeriod(AccessToken, siteId);

            if (sph != null)
            {
                stockPeriodLbl.Text = String.Format("{0:d/M/yyyy}", sph.StartDate.Date) + " - " + String.Format("{0:d/M/yyyy}", sph.EndDate.Date) + " : " + sph.StatusDisplayText;
            }
            else
            {
                stockPeriodLbl.Text = string.Empty;
            }

            CheckSiteItem(siteId);

            this.IsBusy = false;
        }

        private void CheckSiteItem(long siteId)
        {
            int countSiteItem = stockCountItemRepository.GetStockCountItemCountBySite(siteId);

            if (countSiteItem <= 0)
            {
                ScanBarcodeBtn.IsVisible = false;
                SearchBtn.IsVisible = false;
                UploadBtn.IsVisible = false;
                ViewBtn.IsVisible = false;
            }
            else
            {
                ScanBarcodeBtn.IsVisible = true;
                SearchBtn.IsVisible = true;
                UploadBtn.IsVisible = true;
                ViewBtn.IsVisible = true;
            }
        }


        async void OnUploadStockItemButtonClicked(object sender, EventArgs args)
        {
            this.IsBusy = true;

            try
            {
                long siteId = Convert.ToInt64(Application.Current.Properties[ApplicationProperties.Site]);

                IList<IUploadStockCount> upload = new List<IUploadStockCount>();

                IEnumerable<IStockCount> stockCount = this.stockCountRepository.GetUploadStockCountItem(siteId);

                foreach (IStockCount sc in stockCount)
                {
                    IUploadStockCount u =
                        IoCContainer.Container.Resolve(typeof (IUploadStockCount), null) as IUploadStockCount;
                    u.AccessToken = AccessToken;
                    u.SiteId = siteId;
                    u.SiteItemId = sc.SiteItemId;
                    u.Quantity = sc.CurrentCount.Value;
                    u.StockItemSizeId = sc.StockItemSizeId;
                    upload.Add(u);
                }

                var result = await this.fnbWebAPI.UploadStockCount(upload);

                await DisplayAlert("Upload Stock Item", "No. of item uploaded : " + result.Count() +"/" + upload.Count(), "OK");
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            this.IsBusy = false;
        }

        async void OnDownloadStockItemButtonClicked(object sender, EventArgs args)
        {
            this.IsBusy = true;

            try
            {
                long siteId = Convert.ToInt64(Application.Current.Properties[ApplicationProperties.Site]);

                var stockCountItemList =
                    await
                        this.fnbWebAPI.GetStockCountItem(AccessToken,siteId);

                stockCountItemRepository.ResetStockCountItemBySite(siteId);

                stockCountRepository.ResetStockCount();
                stockItemSizeRepository.ResetStockItemSize();
                stockItemSizeBarcodeRepository.ResetStockItemSizeBarcode();

                stockCountItemRepository.InsertStockCountItems(stockCountItemList);

                IEnumerable<IStockItemSize> stockItemSizeList = stockItemSizeRepository.GetStockItemSizes();

                var distinctStockItemSizeFromStockCountItem = stockCountItemList.SelectMany(x => x.StockItemSizes)
                    .GroupBy(
                        x =>
                            new
                            {
                                x.StockItemSizeId,
                                x.StockItemId,
                                x.Size,
                                x.UnitOfMeasureCode,
                                x.UnitOfMeasureId,
                                x.ConversionRatio,
                                x.CaseSizeDescription,
                                x.IsDefault,
                                x.StockCount
                            }).ToList();

                IList<IStockItemSize> stockItemSizeInsertList = new List<IStockItemSize>();
                IList<IStockCount> stockCountInsertList = new List<IStockCount>();

                foreach (var s in distinctStockItemSizeFromStockCountItem)
                {
                    if (!stockItemSizeList.Any(x => x.StockItemSizeId == s.Key.StockItemSizeId))
                    {
                        IStockItemSize size =
                            IoCContainer.Container.Resolve(typeof (IStockItemSize), null) as IStockItemSize;
                        size.StockItemSizeId = s.Key.StockItemSizeId;
                        size.StockItemId = s.Key.StockItemId;
                        size.Size = s.Key.Size;
                        size.UnitOfMeasureCode = s.Key.UnitOfMeasureCode;
                        size.UnitOfMeasureId = s.Key.UnitOfMeasureId;
                        size.ConversionRatio = s.Key.ConversionRatio;
                        size.CaseSizeDescription = s.Key.CaseSizeDescription;
                        size.IsDefault = s.Key.IsDefault;
                        size.StockCount = s.Key.StockCount;

                        if (s.Key.StockCount.HasValue)
                        {
                            IStockCount count =
                                IoCContainer.Container.Resolve(typeof (IStockCount), null) as IStockCount;
                            count.SiteItemId =
                                stockCountItemList.Single(x => x.StockItemId == s.Key.StockItemId).SiteItemId;
                            count.CurrentCount = s.Key.StockCount;
                            count.PreviousCount = null;
                            count.StockItemSizeId = s.Key.StockItemSizeId;
                            count.Updated = false;
                            stockCountInsertList.Add(count);
                        }

                        stockItemSizeInsertList.Add(size);
                    }
                }

                stockItemSizeRepository.InsertStockItemSizes(stockItemSizeInsertList);
                stockCountRepository.InsertStockCounts(stockCountInsertList);

                IList<IStockItemSizeBarcode> stockItemSizeBarcodeList = new List<IStockItemSizeBarcode>();

                IStockItemSizeBarcode barcode = IoCContainer.Container.Resolve(typeof(IStockItemSizeBarcode), null) as IStockItemSizeBarcode;
                barcode.BarcodeContent = "21040971";
                barcode.BarcodeFormat = "CODE_39";
                barcode.StockItemSizeId = 1594;
                barcode.BarcodeType = "TEXT";

                IStockItemSizeBarcode barcode1 = IoCContainer.Container.Resolve(typeof(IStockItemSizeBarcode), null) as IStockItemSizeBarcode;
                barcode1.BarcodeContent = "9794024190944209";
                barcode1.BarcodeFormat = "CODE_128";
                barcode1.StockItemSizeId = 1503;
                barcode1.BarcodeType = "TEXT";

                IStockItemSizeBarcode barcode2 = IoCContainer.Container.Resolve(typeof(IStockItemSizeBarcode), null) as IStockItemSizeBarcode;
                barcode2.BarcodeContent = "29P0667450X";
                barcode2.BarcodeFormat = "CODE_39";
                barcode2.StockItemSizeId = 1390;
                barcode2.BarcodeType = "TEXT";

                IStockItemSizeBarcode barcode3 = IoCContainer.Container.Resolve(typeof(IStockItemSizeBarcode), null) as IStockItemSizeBarcode;
                barcode3.BarcodeContent = "BETBB61229";
                barcode3.BarcodeFormat = "CODE_39";
                barcode3.StockItemSizeId = 1451;
                barcode3.BarcodeType = "TEXT";

                stockItemSizeBarcodeList.Add(barcode);
                stockItemSizeBarcodeList.Add(barcode1);
                stockItemSizeBarcodeList.Add(barcode2);
                stockItemSizeBarcodeList.Add(barcode3);

                stockItemSizeBarcodeRepository.InsertStockItemSizeBarcodes(stockItemSizeBarcodeList);
                /*
                 * 

INSERT INTO StockItemSizeBarcode (BarcodeContent, BarcodeFormat, StockItemSizeId, BarcodeType)
VALUES('21040971','CODE_39',1594,'TEXT')

INSERT INTO StockItemSizeBarcode (BarcodeContent, BarcodeFormat, StockItemSizeId, BarcodeType)
VALUES('9794024190944209','CODE_128',1503,'TEXT')

INSERT INTO StockItemSizeBarcode (BarcodeContent, BarcodeFormat, StockItemSizeId, BarcodeType)
VALUES('29P0667450X','CODE_39',1390,'TEXT')

INSERT INTO StockItemSizeBarcode (BarcodeContent, BarcodeFormat, StockItemSizeId, BarcodeType)
VALUES('BETBB61229','CODE_39',1451,'TEXT')
                 * */

                await DisplayAlert("Download Stock Item", "Stock Count Item Inserted : " + stockCountItemList.Count() + "\n Stock Item Size Inserted : " +
                    stockItemSizeInsertList.Count() + "\n Stock Count Inserted : " + stockCountInsertList.Count(), "OK");

                CheckSiteItem(siteId);

            }
            catch (Exception ex)
            {
                DisplayAlert("Download Stock Item", "Error while download stock item : " + ex.Message, "OK");
            }

            this.IsBusy = false;
        }

        async void OnViewStockCountButtonClicked(object sender, EventArgs args)
        { 
            IEnumerable<IStockCountItem> result = stockCountItemRepository.GetStockCountItems();

            ViewGridPage gridPage = new ViewGridPage(this.database);

            await Navigation.PushAsync(gridPage);

            /*
           
            IList<StockCountItem> test = new List<StockCountItem>();

            foreach (IStockCountItem s in result)
            {
                StockCountItem x = new StockCountItem();
                x.ItemName = s.ItemName;
                x.CategoryHierarchy = s.CategoryHierarchy;

                test.Add(x);
            }
            await Navigation.PushAsync(new ContentPage {
                Content = new ColumnishGrid<StockCountItem>
                {
                    BackgroundColor = Color.White,
                    VerticalOptions = LayoutOptions.FillAndExpand,

                    RowHeight = 50,

                    Columns = new ColumnishGrid<StockCountItem>.ColumnInfo[] {
						new ColumnishGrid<StockCountItem>.ColumnInfo {
							Title = "Item Name",
							PropertyName = "ItemName",
							Width = 100,
							FillColor = Color.Aqua,
							HorizontalAlignment = TextAlignment.Start,
						},
						new ColumnishGrid<StockCountItem>.ColumnInfo {
							Title = "Category Hierarchy",
							PropertyName = "CategoryHierarchy",
							Width = 150,
							FillColor = Color.Yellow,
							HorizontalAlignment = TextAlignment.End,
						}
					},
                    Rows = test
                }
			})
            ;
             */
            // await Navigation.PushAsync(new ViewStockCount());
        }
    }
}
