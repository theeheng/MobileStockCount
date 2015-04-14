using System.Collections.Generic;

namespace DomainInterface
{
    public interface IStockCountRepository
    {
        IDatabase db { get; set; }
        IEnumerable<IStockCount> GetStockCounts();
        IStockCount GetStockItemSize(int siteItemId, int stockItemSizeId);
        int InsertStockCount(IStockCount stockCount);
        int InsertStockCounts(IEnumerable<IStockCount> stockCountList);
        int DeleteStockItemSize(IStockCount stockCount);
        int ResetStockCount();
        List<IStockCount> GetStockCountBySiteItemId(long siteItemId);
        List<IStockCount> GetUploadStockCountItem(long siteId);
        int UpdateStockCount(IStockCount stockCount);
    }
}