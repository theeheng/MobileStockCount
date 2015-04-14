using System;
using System.Collections.Generic;

namespace DomainInterface
{
    public interface IStockCountItem
    {
        long SiteItemId { get; set; }
        long CategoryId { get; set; }
        String ItemName { get; set; }
        String CategoryName { get; set; }
        String CategoryHierarchy { get; set; }
        long SupplierId { get; set; }
        long SiteId { get; set; }
        long StockItemId { get; set; }
        double CostPrice { get; set; }
        List<IStockItemSize> StockItemSizes { get; set; }
        List<IStockCount> Count { get; set; }
    }
}