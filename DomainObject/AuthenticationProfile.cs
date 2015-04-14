using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;

namespace DomainObject
{
    public class AuthenticationProfile : IAuthenticationProfile
    {
        public string AccessToken { get; set; }
        public long UserProfileId { get; set; }
        public string UniqueOrganisationId { get; set; }
    }
}
