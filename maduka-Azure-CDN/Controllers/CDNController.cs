using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace maduka_Azure_CDN.Controllers
{
    using Newtonsoft.Json;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class CDNController : ApiController
    {
        string clientId = "[註冊的應用程式識別碼]";
        string clientSecret = "[金鑰值]";
        string strSubscriptionsId = "[MSDN訂閱代碼]";
        string strResourceGroup = "[CDN節點所在的群組名稱]";
        string strCdnProfile = "[CDN服務名稱]";
        string strCdnEndPoint = "[CDN節點名稱]";
        string strAuthority = "[MSDN訂閱的網域，如maduka.onmicrosoft.com]";

        /*
        string clientId = "[註冊的應用程式識別碼]";
        string clientSecret = "[金鑰值]";
        string strSubscriptionsId = "cef3a64a-b109-4082-b5e7-cdcc8c9e6171";
        string strResourceGroup = "maduka";
        string strCdnProfile = "madukacdn";
        string strCdnEndPoint = "madukacdnstorage";
        string strAuthority = "madukaborhotmail.onmicrosoft.com";
        */

        /// <summary>
        /// 清除CDN快取內容
        /// </summary>
        /// <param name="query">輸入的CDN清除內容JSON物件</param>
        [HttpPost]
        public string PurgeContent(PurgeContentQuery query)
        {
            // 置換設定資料
            string uri = $"https://management.azure.com/subscriptions/{strSubscriptionsId}/resourceGroups/{strResourceGroup}/providers/Microsoft.Cdn/profiles/{strCdnProfile}/endpoints/{strCdnEndPoint}/purge?api-version=2016-10-02";
            string authority = $"https://login.microsoftonline.com/{strAuthority}";

            // 取得AAD的AccessToken
            var authenticationContext = new AuthenticationContext(authority);
            ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);
            Task<AuthenticationResult> resultstr = authenticationContext.AcquireTokenAsync("https://management.core.windows.net/", clientCredential);
            var token = resultstr.Result.AccessToken;

            // 準備呼叫CDN API
            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);
            client.Headers.Add("Content-Type", "application/json");
            string result = "";

            try
            {
                // 執行清理CDN檔案路徑快取的動作
                result = client.UploadString(uri, JsonConvert.SerializeObject(query));
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        public class PurgeContentQuery
        {
            public List<string> contentPaths { get; set; }
        }
    }
}
