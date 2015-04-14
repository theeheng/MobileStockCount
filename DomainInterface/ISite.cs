using System;

namespace DomainInterface
{
    public interface ISite
    {
        int SiteId { get; set; }
        String UnitSiteName { get; set; }
        String UnitName { get; set; }
    }
}