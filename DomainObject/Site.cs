using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;
using Newtonsoft.Json;

namespace DomainObject
{
    public class Site : ISite
    {
        [JsonProperty("NodeId")]
        public int SiteId { get; set; }
        [JsonProperty("NodeName")]
        public String UnitSiteName { get; set; }
        [JsonProperty("UnitName")]
        public String UnitName { get; set; }
    }

}
