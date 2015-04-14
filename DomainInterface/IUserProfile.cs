using System;

namespace DomainInterface
{
    public interface IUserProfile
    {
        long UserProfileID { get; set; }
        long ProfileID { get; set; }
        long ConfigurationID { get; set; }
        String ConfigurationName { get; set; }
        String ProfileName { get; set; }
        String ProfileFullName { get; set; }
    }
}