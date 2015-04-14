using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;
using DomainObject;

namespace DataAccess
{
    public class StockCountRepositsory : IStockCountRepository
    {
        public IDatabase db { get; set; }
        static object locker = new object();

        public IEnumerable<IStockCount> GetStockCounts()
        {
            lock (locker)
            {
                return (from i in db.Table<StockCount>() select i).ToList();
            }
        }

        public IStockCount GetStockItemSize(int siteItemId, int stockItemSizeId)
        {
            lock (locker)
            {
                return db.Table<StockCount>().FirstOrDefault(x => x.SiteItemId == siteItemId && x.StockItemSizeId == stockItemSizeId);
            }
        }

        public List<IStockCount> GetUploadStockCountItem(long siteId)
        {
            return db.Query<StockCount>("SELECT A.* FROM StockCount A INNER JOIN StockCountItem B ON A.SiteItemId = B.SiteItemId WHERE B.SiteId = ? AND A.Updated = 1 ", siteId).ToList<IStockCount>();
        }

        public int UpdateStockCount(IStockCount stockCount)
        {
            lock (locker)
            {
                return db.Execute("UPDATE StockCount SET CurrentCount = ?, PreviousCount = ?, Updated = 1 WHERE SiteItemId = ? AND StockItemSizeId = ? ", stockCount.CurrentCount, stockCount.PreviousCount, stockCount.SiteItemId, stockCount.StockItemSizeId);
            }
        }

        public int InsertStockCount(IStockCount stockCount)
        {
            lock (locker)
            {
                return db.Insert(stockCount);
            }
        }

        public int InsertStockCounts(IEnumerable<IStockCount> stockCountList)
        {
            lock (locker)
            {
                return db.InsertAll(stockCountList);
            }
        }

        public int DeleteStockItemSize(IStockCount stockCount)
        {
            lock (locker)
            {
                return db.Delete<StockCountItem>(stockCount);
            }
        }

        public int ResetStockCount()
        {
            lock (locker)
            {
                return db.Execute(@"DELETE FROM StockCount WHERE StockItemSizeId IN(Select A.StockItemSizeId FROM StockItemSize A LEFT OUTER JOIN StockCountItem B ON A.StockItemId = B.StockItemId WHERE B.StockItemId IS NULL)");
            }
        }

        public List<IStockCount> GetStockCountBySiteItemId(long siteItemId)
        {
            return db.Table<StockCount>().Where(x => x.SiteItemId == siteItemId).ToList<IStockCount>();
        }
    }
}
