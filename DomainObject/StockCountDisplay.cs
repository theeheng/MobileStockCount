using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObject
{
    public class StockCountDisplay {
        public string ItemName { get; set; }
        public double Size { get; set; }
        public string UnitOfMeasureCode { get; set; }
        public double CurrentCount { get; set; }
        public double PreviousCount { get; set; }
        public int SiteItemId { get; set; }
        public int StockItemSizeId { get; set; }
    }
}
