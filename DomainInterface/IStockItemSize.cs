using System;

namespace DomainInterface
{
    public interface IStockItemSize : IBindablePicker
    {
        long StockItemSizeId { get; set; }
        long StockItemId { get; set; }
        double Size { get; set; }
        String UnitOfMeasureCode { get; set; }
        long UnitOfMeasureId { get; set; }
        double ConversionRatio { get; set; }
        String CaseSizeDescription { get; set; }
        bool IsDefault { get; set; }
        double? StockCount { get; set; }
        string GetBindableName();
    }
}