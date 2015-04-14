using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;
using DomainObject;

namespace DataAccess
{
    public class StockItemSizeRepository : IStockItemSizeRepository
    {
        public IDatabase db { get; set; }
        static object locker = new object();

        public IEnumerable<IStockItemSize> GetStockItemSizes()
        {
            lock (locker)
            {
                return (from i in db.Table<StockItemSize>() select i).ToList();
            }
        }

        public IStockItemSize GetStockItemSize(int stockItemSizeId)
        {
            lock (locker)
            {
                return db.Table<StockItemSize>().FirstOrDefault(x => x.StockItemSizeId == stockItemSizeId);
            }
        }

        public List<IStockItemSize> GetStockItemSizeByStockItemId(long stockItemId)
        {
            lock (locker)
            {
                return db.Table<StockItemSize>().Where(x => x.StockItemId == stockItemId).ToList<IStockItemSize>();
            }
        }


        public int InsertStockItemSize(IStockItemSize stockItemSize)
        {
            lock (locker)
            {
                return db.Insert(stockItemSize);
            }
        }

        public int InsertStockItemSizes(IEnumerable<IStockItemSize> stockItemSizeList)
        {
            lock (locker)
            {
                return db.InsertAll(stockItemSizeList);
            }
        }

        public int DeleteStockItemSize(IStockItemSize stockItemSize)
        {
            lock (locker)
            {
                return db.Delete<StockCountItem>(stockItemSize.StockItemSizeId);
            }
        }

        public int ResetStockItemSize()
        {
            lock (locker)
            {
                return db.Execute(@"DELETE FROM StockItemSize WHERE StockItemSizeId IN(Select A.StockItemSizeId FROM StockItemSize A LEFT OUTER JOIN StockCountItem B ON A.StockItemId = B.StockItemId WHERE B.StockItemId IS NULL)");
            }
        }

        /*
         *  db.execSQL("DELETE FROM StockCount WHERE StockItemSizeId IN(Select A.StockItemSizeId FROM StockItemSize A LEFT OUTER JOIN StockCountItem B ON A.StockItemId = B.StockItemId WHERE B.StockItemId IS NULL)");
            db.execSQL("DELETE FROM StockItemSizeBarcode WHERE StockItemSizeId IN(Select A.StockItemSizeId FROM StockItemSize A LEFT OUTER JOIN StockCountItem B ON A.StockItemId = B.StockItemId WHERE B.StockItemId IS NULL)");
            db.execSQL("DELETE FROM StockItemSize WHERE StockItemSizeId IN(Select A.StockItemSizeId FROM StockItemSize A LEFT OUTER JOIN StockCountItem B ON A.StockItemId = B.StockItemId WHERE B.StockItemId IS NULL)");
*/
    }
}
