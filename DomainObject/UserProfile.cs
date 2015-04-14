using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;

namespace DomainObject
{
    public class UserProfile : IUserProfile
    {
        public long UserProfileID { get; set; }
        public long ProfileID { get; set; }
        public long ConfigurationID { get; set; }
        public String ConfigurationName { get; set; }
        public String ProfileName { get; set; }
        public String ProfileFullName { get; set; }
    }
}
