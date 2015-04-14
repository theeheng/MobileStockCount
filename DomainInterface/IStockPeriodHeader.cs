using System;

namespace DomainInterface
{
    public interface IStockPeriodHeader
    {
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        String StatusDisplayText { get; set; }
        long StockPeriodHeaderID { get; set; }
    }
}