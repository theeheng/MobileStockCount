using System.Collections.Generic;

namespace DomainInterface
{
    public interface IStockItemSizeRepository
    {
        IDatabase db { get; set; }
        IEnumerable<IStockItemSize> GetStockItemSizes();
        IStockItemSize GetStockItemSize(int stockItemSizeId);
        int InsertStockItemSize(IStockItemSize stockItemSize);
        int InsertStockItemSizes(IEnumerable<IStockItemSize> stockItemSizeList);
        int DeleteStockItemSize(IStockItemSize stockItemSize);
        int ResetStockItemSize();
        List<IStockItemSize> GetStockItemSizeByStockItemId(long stockItemId);
    }
}