using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;
using SQLite.Net.Attributes;

namespace DomainObject
{
    public class StockItemSize : IStockItemSize

{
    [PrimaryKey]
    public long StockItemSizeId { get; set; }
    public long StockItemId { get; set; }
    public double Size { get; set; }
    public String UnitOfMeasureCode { get; set; }
    public long UnitOfMeasureId { get; set; }
    public double ConversionRatio { get; set; }
    public String CaseSizeDescription { get; set; }
    public bool IsDefault { get; set; }
    public double? StockCount { get; set; }

        public string GetBindableName()
        {
            return Size.ToString() + " " + UnitOfMeasureCode;
        }
}
}
