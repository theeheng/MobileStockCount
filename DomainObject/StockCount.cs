using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainConstant;
using DomainInterface;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace DomainObject
{
    public class StockCount : IStockCount
    {
        [ForeignKey(typeof(StockCountItem))]
        public long SiteItemId { get; set; }
        public double? CurrentCount { get; set; }
        public double? PreviousCount { get; set; }
        [ForeignKey(typeof(StockItemSize))]
        public long StockItemSizeId { get; set; }
        [Ignore]
        public DBOperation operation { get; set; }
        public bool Updated { get; set; }
    }
}
