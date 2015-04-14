using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;
using DomainObject;

namespace DataAccess
{
    public class StockCountItemRepository : IStockCountItemRepository
    {
        public IDatabase db { get; set; }
        static object locker = new object();

        public IEnumerable<IStockCountItem> GetStockCountItems()
        {
            lock (locker)
            {
                return (from i in db.Table<StockCountItem>() select i).ToList();
            }
        }

        public IStockCountItem GetStockCountItem(long siteItemId)
        {
            lock (locker)
            {
                return db.Table<StockCountItem>().FirstOrDefault(x => x.SiteItemId == siteItemId);
            }
        }

        public int GetStockCountItemCountBySite(long siteId)
        {
            lock (locker)
            {
                return db.Table<StockCountItem>().Where(x => x.SiteId == siteId).Count();
            }
        }


        public IStockCountItem GetStockCountItemFromBarcode(string barcodeContent, string barcodeFormat)
        {
            lock (locker)
            {
                IEnumerable<IStockCountItem> stock = db.Query<StockCountItem>("SELECT A.* FROM StockCountItem A INNER JOIN StockItemSize B ON A.StockItemId = B.StockItemId INNER JOIN StockItemSizeBarcode C ON B.StockItemSizeId = C.StockItemSizeId WHERE C.BarCodeContent = ? AND C.BarCodeFormat = ? AND NOT(C.StockItemSizeId IS NULL) ", barcodeContent, barcodeFormat);

                if (stock != null && stock.Any())
                {
                    return stock.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }

        public IEnumerable<IStockCountItem> GetStockCountItemFromName(string name)
        {
            lock (locker)
            {
                 return db.Query<StockCountItem>("SELECT A.* FROM StockCountItem A WHERE ItemName LIKE ?", "%"+name+"%").ToList<IStockCountItem>();
                //return db.Table<StockCountItem>().Where(x => x.CategoryId == 390).ToList();
            }
        }

        public int InsertStockCountItem(IStockCountItem stockCountItem)
        {
            lock (locker)
            {
                return db.Insert(stockCountItem);
            }
        }

        public int InsertStockCountItems(IEnumerable<IStockCountItem> stockCountItemList)
        {
            lock (locker)
            {
                return db.InsertAll(stockCountItemList);
            }
        }

        //		public int DeleteStock(int id) 
        //		{
        //			lock (locker) {
        //				return Delete<Stock> (new Stock () { Id = id });
        //			}
        //		}
        public int DeleteStockCountItem(IStockCountItem stockCountItem)
        {
            lock (locker)
            {
                return db.Delete<StockCountItem>(stockCountItem.SiteItemId);
            }
        }

        public int ResetStockCountItemBySite(long siteId)
        {
            try
            {
                lock (locker)
                {
                    return db.Execute(@"DELETE FROM StockCountItem Where SiteId = ?", siteId);
                }

            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
