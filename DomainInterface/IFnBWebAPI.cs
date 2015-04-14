using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainInterface
{
    public interface IFnBWebAPI
    {
        Task<string> Login(string userName, string password);
        Task<IEnumerable<IOrganisation>> GetOrganisation(string accessToken);
        Task<IEnumerable<IUserProfile>> GetUserProfile(string accessToken, string uniqueOrganisationId);
        Task<IUserProfile> SetUserProfile(IAuthenticationProfile profile);
        Task<IEnumerable<ISite>> GetSite(string accessToken);
        Task<IStockPeriodHeader> GetCurrentStockPeriod(string accessToken, long siteId);
        Task<IEnumerable<IStockCountItem>> GetStockCountItem(string accessToken, long siteId);
        Task<IEnumerable<IUploadStockCount>> UploadStockCount(IEnumerable<IUploadStockCount> stockCount);
    }
}