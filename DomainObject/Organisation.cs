using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;

namespace DomainObject
{
    public class Organisation : IOrganisation
    {
        public int OrganisationId { get; set; }
        public String OrganisationCode { get; set; }
        public String OrganisationName { get; set; }
        public String UniqueConfigId { get; set; }
        public String UniqueOrganisationId { get; set; }
    }
}
