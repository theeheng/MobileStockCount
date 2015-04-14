using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;
using DomainObject;

namespace DataAccess
{
    public class StockRepository : IStockRepository
    {
        public IDatabase db { get; set; }
        static object locker = new object();

        public IEnumerable<IStock> GetStocks()
        {
            lock (locker)
            {
                return (from i in db.Table<Stock>() select i).ToList();
            }
        }

        public IStock GetStock(int id)
        {
            lock (locker)
            {
                return db.Table<Stock>().FirstOrDefault(x => x.Id == id);
            }
        }

        public int SaveStock(IStock item)
        {
            lock (locker)
            {
                if (item.Id != 0)
                {
                    db.Update(item);
                    return item.Id;
                }
                else
                {
                    return db.Insert(item);
                }
            }
        }

        //		public int DeleteStock(int id) 
        //		{
        //			lock (locker) {
        //				return Delete<Stock> (new Stock () { Id = id });
        //			}
        //		}
        public int DeleteStock(IStock stock)
        {
            lock (locker)
            {
                return db.Delete<Stock>(stock.Id);
            }
        }
    }
}
