using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;
using SQLite.Net.Attributes;

namespace DomainObject
{
    public class StockCountItem : IStockCountItem
    {
        [PrimaryKey]
        public long SiteItemId { get; set; }
        public long CategoryId { get; set; }
        public String ItemName { get; set; }
        public String CategoryName { get; set; }
        public String CategoryHierarchy { get; set; }
        public long SupplierId { get; set; }
        public long SiteId { get; set; }
        public long StockItemId { get; set; }
        public double CostPrice { get; set; }

        [Ignore]
        public List<IStockItemSize> StockItemSizes { get; set; }

        [Ignore]
        public List<IStockCount> Count { get; set; }

        public StockCountItem()
        {
            
        }
    }
}
