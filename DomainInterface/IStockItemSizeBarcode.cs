namespace DomainInterface
{
    public interface IStockItemSizeBarcode
    {
        string BarcodeContent { get; set; }
        string BarcodeFormat { get; set; }
        long StockItemSizeId { get; set; }
        string BarcodeType { get; set; }
    }
}