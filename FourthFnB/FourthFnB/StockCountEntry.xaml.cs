using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainConstant;
using DomainInterface;
using IoC;
using Xamarin.Forms;

namespace FourthFnB
{
    public class Loading : INotifyPropertyChanged
    {
        private bool isBusy;

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
    }
    public partial class StockCountEntry : ContentPage
    {
        private Loading loading;

        public StockCountEntry()
        {
            InitializeComponent();
            loading = new Loading();
            loading.IsBusy = false;
            activityIndicator.BindingContext = loading;
        }

        async void OnBackButtonClicked(object sender, EventArgs args)
        {
           await Navigation.PopAsync(true);
        }

        async void OnSaveButtonClicked(object sender, EventArgs args)
        {
            if (Quantity.Text != string.Empty)
            {
                IStockCountItem sci = (IStockCountItem) this.BindingContext;

                IStockItemSize size = sci.StockItemSizes.ElementAt(uomPkr.SelectedIndex);

                IStockCount sc = null;

                IStockCountRepository stockCountRepository =
                    IoCContainer.Container.Resolve(typeof (IStockCountRepository), null) as IStockCountRepository;
                IDatabase database =
                    IoCContainer.Container.Resolve(typeof (IDatabase), IoCInstanceConstant.StockDatabaseInstance) as
                        IDatabase;

                stockCountRepository.db = database;

                try
                {
                    if (sci.Count.Any(x => x.StockItemSizeId == size.StockItemSizeId))
                    {
                        sc = sci.Count.Where(x => x.StockItemSizeId == size.StockItemSizeId).Single();
                        sc.PreviousCount = sc.CurrentCount;
                        sc.CurrentCount = Convert.ToDouble(Quantity.Text);
                        sc.Updated = true;
                        sc.operation = DomainConstant.DBOperation.Update;

                        stockCountRepository.UpdateStockCount(sc);
                    }
                    else
                    {
                        sc = IoCContainer.Container.Resolve(typeof (IStockCount), null) as IStockCount;
                        sc.CurrentCount = Convert.ToDouble(Quantity.Text);
                        sc.PreviousCount = Convert.ToDouble(Quantity.Text);
                        sc.SiteItemId = sci.SiteItemId;
                        sc.Updated = true;
                        sc.StockItemSizeId = size.StockItemSizeId;

                        stockCountRepository.InsertStockCount(sc);
                    }
                }
                catch (Exception ex)
                {
                    string err = ex.Message;
                }

                await Navigation.PopAsync(true);
            }
            else
            {
                DisplayAlert("Update Stock Count", "Please enter a quantity.", "OK");
            }
        }

        async void OnUomSelectedIndexChanged(object sender, EventArgs args)
        {
            try
            {
                
                IStockCountItem sci = (IStockCountItem) this.BindingContext;

                IStockItemSize test = sci.StockItemSizes.ElementAt(uomPkr.SelectedIndex);


                if (sci.Count.Any(x => x.StockItemSizeId == test.StockItemSizeId))
                {
                    IStockCount sc = sci.Count.Where(x => x.StockItemSizeId == test.StockItemSizeId).Single();

                    if (sc.CurrentCount.HasValue)
                    {
                        Quantity.Text = sc.CurrentCount.Value.ToString();
                        CostPrice.Text = sci.CostPrice.ToString();
                        UpdateTotalValue();
                    }
                    else if (Quantity.Text != string.Empty)
                    {
                        Quantity.Text = string.Empty;
                        TotalValue.Text = "0";
                    }
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }

        public void OnQuantityTextChanged(object sender, EventArgs args)
        {
            UpdateTotalValue();
        }

        public void UpdateTotalValue()
        {
            if (Quantity.Text != string.Empty && CostPrice.Text != string.Empty)
            {
                double qty = Convert.ToDouble(Quantity.Text);
                double costPrice = Convert.ToDouble(CostPrice.Text);

                TotalValue.Text = Math.Round(costPrice * qty, 2).ToString();
            }
        }

    }
}
