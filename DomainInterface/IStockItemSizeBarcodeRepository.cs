using System.Collections.Generic;

namespace DomainInterface
{
    public interface IStockItemSizeBarcodeRepository
    {
        IDatabase db { get; set; }
        IEnumerable<IStockItemSizeBarcode> GetStockItemSizeBarcodes();
        IStockItemSizeBarcode GetStockItemSizeBarcode(string barcodeContent, string barcodeFormat , int stockItemSizeId);
        int InsertStockItemSizeBarcode(IStockItemSizeBarcode stockItemSizeBarcode);
        int InsertStockItemSizeBarcodes(IEnumerable<IStockItemSizeBarcode> stockItemSizeBarcodeList);
        int DeleteStockItemSizeBarcode(IStockItemSizeBarcode stockItemSizeBarcode);
        int ResetStockItemSizeBarcode();
    }
}