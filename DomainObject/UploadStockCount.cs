using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;

namespace DomainObject
{
    public class UploadStockCount :  IUploadStockCount
    {
        public string AccessToken { get; set; }
        public long SiteId { get; set; }
        public long SiteItemId { get; set; }
        public long StockItemSizeId { get; set; }
        public double Quantity { get; set; }
    }
}
