using DomainInterface;

namespace DomainObject
{
    public class Login : ILogin
    {
        public string accessToken {get; set;}
    }
}