using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainInterface
{
    public interface IUploadStockCount
    {
        string AccessToken {get; set;}
        long SiteId { get; set;}
        long SiteItemId { get; set;}
        long StockItemSizeId { get; set;}
        double Quantity {get; set;}
    }
}
