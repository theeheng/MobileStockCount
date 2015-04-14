using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;

namespace DomainObject
{
    public class StockPeriodHeader : IStockPeriodHeader
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String StatusDisplayText { get; set; }
        public long StockPeriodHeaderID { get; set; }
    }

}
