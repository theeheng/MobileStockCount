using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainInterface
{
    public interface IStockRepository
    {
        IDatabase db { get; set; }
        IEnumerable<IStock> GetStocks();
        IStock GetStock(int id);
        int SaveStock(IStock item);
        int DeleteStock(IStock stock);
    }
}
