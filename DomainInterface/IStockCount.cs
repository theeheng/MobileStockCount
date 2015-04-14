using DomainConstant;

namespace DomainInterface
{
    public interface IStockCount
    {
        long SiteItemId { get; set; }
        double? CurrentCount { get; set; }
        double? PreviousCount { get; set; }
        long StockItemSizeId { get; set; }
        DBOperation operation { get; set; }
        bool Updated { get; set; }
    }
}