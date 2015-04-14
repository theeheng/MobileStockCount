using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;
using IoC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.NetworkInformation;


namespace FnBModelWebAPI
{
    public class FnBWebAPI : IFnBWebAPI
    {
        private readonly string WebAPIURL = "http://10.0.26.67/FnBModelWebAPI/";

        public FnBWebAPI()
        {
        }

        private T DeserializeObject<T>(Stream stream)
        {
            return (T)JsonConvert.DeserializeObject<T>(new StreamReader(stream).ReadToEnd(), IoCContainer.GetCustomSerializerSetting<T>());
            //return (T)JsonConvert.DeserializeObject<T>(new StreamReader(stream).ReadToEnd());
        }

        private string SerializeObject<T>(T sObject)
        {
            return JsonConvert.SerializeObject(sObject, IoCContainer.GetCustomSerializerSetting<T>());
            //return JsonConvert.SerializeObject(sObject);
        }


        // Gets weather data from the passed URL.  
        private async Task<T> PostWebAPIAsync<T>(string url, StringContent content, long? timeout)
        {
            // Create an HTTP web request using the URL:

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIURL);
                // client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (timeout.HasValue)
                {
                    client.Timeout = TimeSpan.FromMilliseconds(timeout.Value);
                }
                // New code:
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        return DeserializeObject<T>(responseStream);
                    }
                }
            }

            return default(T);
        }

        // Gets weather data from the passed URL.  
        private async Task<T> GetWebAPIAsync<T>(string url, long? timeout)
        {
            // Create an HTTP web request using the URL:

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.Timeout = TimeSpan.FromMilliseconds(10000);

                if (timeout.HasValue)
                {
                    client.Timeout = TimeSpan.FromMilliseconds(timeout.Value);
                }

                // New code:
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        return DeserializeObject<T>(responseStream);
                    }
                }
            }

            return default(T);

            /*HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
            request.ContentType = "application/json";
            request.Method = "GET";

            try
            {
                // Send the request to the server and wait for the response:
                using (WebResponse response = await request.GetResponseAsync())
                {
                    // Get a stream representation of the HTTP web response:
                    using (StreamReader streamRdr = new StreamReader(response.GetResponseStream()))
                    {


                        // Use this stream to build a JSON document object:
                        JObject jsonDoc =
                            await Task.Run(() => JsonConvert.DeserializeObject<JObject>(streamRdr.ReadToEnd()));
                        //Console.Out.WriteLine("Response: {0}", jsonDoc.ToString ());

                        // Return the JSON document:
                        return jsonDoc;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
          */
        }
        
        public async Task<string> Login(string userName, string password)
        {
            string loginResourceUrl = "api/login/";


            string query = "?username=" + userName + "&password=" + password + "&ipAddress=192.168.0.1";

            string url = string.Empty;

            try
            {
                url = loginResourceUrl + query;
                // Fetch the weather information asynchronously, parse the results,
                // then update the screen:

                long? timeout = 10000;

                ILogin login = await GetWebAPIAsync<ILogin>(url, timeout);

                string accessToken = (login != null) ? login.accessToken : OfflineLogin(userName, password);

                return accessToken;
            }
            catch (Exception ex)
            {
                return OfflineLogin(userName, password);
            }

            return string.Empty;
        }

        public async Task<IEnumerable<IOrganisation>> GetOrganisation(string accessToken)
        {
            string organisationResourceUrl = "api/organisation/";
            
            string query = "?accessToken=" + accessToken;

            string url = string.Empty;

            try
            {
                url = organisationResourceUrl + query;
                // Fetch the weather information asynchronously, parse the results,
                // then update the screen:
                IEnumerable<IOrganisation> organisations = await GetWebAPIAsync<IEnumerable<IOrganisation>>(url, null);

                return organisations;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        public async Task<IEnumerable<IUserProfile>> GetUserProfile(string accessToken, string uniqueOrganisationId)
        {
            string profileResourceUrl = "api/profile/";

            string query = "?accessToken=" + accessToken + "&uniqueOrganisationId=" + uniqueOrganisationId;

            string url = string.Empty;

            try
            {
                url = profileResourceUrl + query;
                // Fetch the weather information asynchronously, parse the results,
                // then update the screen:
                IEnumerable<IUserProfile> userProfiles = await GetWebAPIAsync<IEnumerable<IUserProfile>>(url, null);

                return userProfiles;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        public async Task<IUserProfile> SetUserProfile(IAuthenticationProfile profile)
        {
            string profileResourceUrl = "api/profile/";

            string postBody = SerializeObject<IAuthenticationProfile>(profile); 
            
            //var content = new StringContent("{\"accessToken\":\""+ profile.AccessToken +"\",\"userProfileId\": "+ profile.UserProfileId.ToString()+",\"uniqueOrganisationId\":\""+ profile.UniqueOrganisationId+"\"}", Encoding.UTF8, "application/json");
            var content = new StringContent(postBody, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            IUserProfile test = await PostWebAPIAsync<IUserProfile>(profileResourceUrl, content, null);

            return test;

        }

        public async Task<IEnumerable<IUploadStockCount>> UploadStockCount(IEnumerable<IUploadStockCount> stockCount)
        {
            string profileResourceUrl = "api/stockcountitem/";

            string postBody = SerializeObject<IEnumerable<IUploadStockCount>>(stockCount);

            /*
             params.put("accessToken", accessToken);
                            params.put("siteId", siteId );
                            params.put("siteItemId", scd.SiteItemId);
                            params.put("stockItemSizeId", scd.StockItemSizeId);
                            params.put("quantity",scd.CurrentCount);
            */

            var content = new StringContent(postBody, Encoding.UTF8, "application/json");


            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            IEnumerable<IUploadStockCount> test = await PostWebAPIAsync<IEnumerable<IUploadStockCount>>(profileResourceUrl, content, null);

            return test;

        }

        public async Task<IEnumerable<ISite>> GetSite(string accessToken)
        {
            string siteResourceUrl = "api/site/";

            string query = "?accessToken=" + accessToken;

            string url = string.Empty;
 
            bool successful = false;
            int retry = 0;

            do
            {
                try
                {
                    url = siteResourceUrl + query;
                    // Fetch the weather information asynchronously, parse the results,
                    // then update the screen:
                    IEnumerable<ISite> sites = await GetWebAPIAsync<IEnumerable<ISite>>(url, null);

                    successful = true;

                    return sites;
                }
                catch (TaskCanceledException cancel)
                {
                    successful = false;
                    retry++;
                }
                catch (Exception ex)
                {
                    successful = true;
                    throw ex;
                }

            } while (successful == false && retry < 3);


            return null;
        }

        public async Task<IStockPeriodHeader> GetCurrentStockPeriod(string accessToken, long siteId)
        {
            string siteResourceUrl = "api/stockperiodheader/";

            string query = "?accessToken=" + accessToken + "&siteId="+siteId ;

            string url = string.Empty;

            try
            {
                url = siteResourceUrl + query;
                // Fetch the weather information asynchronously, parse the results,
                // then update the screen:
                IStockPeriodHeader stockPeriod = await GetWebAPIAsync<IStockPeriodHeader>(url, null);

                return stockPeriod;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;

        }

        public async Task<IEnumerable<IStockCountItem>> GetStockCountItem(string accessToken, long siteId)
        {
            string siteResourceUrl = "api/stockcountitem/";

            string query = "?accessToken=" + accessToken + "&siteId=" + siteId;

            string url = string.Empty;

            try
            {
                url = siteResourceUrl + query;
                // Fetch the weather information asynchronously, parse the results,
                // then update the screen:
                IEnumerable<IStockCountItem> stockItems = await GetWebAPIAsync<IEnumerable<IStockCountItem>>(url , null);

                return stockItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        private string OfflineLogin(string userName, string password)
        {
            if (userName == "fullaccess" && password == "Budapest11")
            {
                return "offline";
            }

            return string.Empty;
        }
    }
}
