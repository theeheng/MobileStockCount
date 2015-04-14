using System.Collections.Generic;

namespace DomainInterface
{
    public interface IStockCountItemRepository
    {
        IDatabase db { get; set; }
        IEnumerable<IStockCountItem> GetStockCountItems();
        IStockCountItem GetStockCountItem(long siteItemId);
        int InsertStockCountItem(IStockCountItem stockCountItem);
        int InsertStockCountItems(IEnumerable<IStockCountItem> stockCountItemList);
        int DeleteStockCountItem(IStockCountItem stockCountItem);
        int ResetStockCountItemBySite(long siteId);
        IStockCountItem GetStockCountItemFromBarcode(string barcodeContent, string barcodeFormat);
        IEnumerable<IStockCountItem> GetStockCountItemFromName(string name);
        int GetStockCountItemCountBySite(long siteId);
    }
}