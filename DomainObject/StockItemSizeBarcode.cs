using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;
using SQLiteNetExtensions.Attributes;

namespace DomainObject
{
    public class StockItemSizeBarcode : IStockItemSizeBarcode
    {
        public string BarcodeContent { get; set; }
        public string BarcodeFormat { get; set; }
        [ForeignKey(typeof(StockItemSize))]
        public long StockItemSizeId { get; set; }
        public string BarcodeType { get; set; }
    }
}
