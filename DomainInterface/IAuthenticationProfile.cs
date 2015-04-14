using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainInterface
{
    public interface IAuthenticationProfile
    {
        string AccessToken { get; set; }
        long UserProfileId { get; set; }
        string UniqueOrganisationId { get; set; }
    }
}
