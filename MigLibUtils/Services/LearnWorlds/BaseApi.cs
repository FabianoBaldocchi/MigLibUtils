using APIBase.Utils;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MigLibUtils.Services.LearnWorlds
{
    class BaseApi
    {
        static string ApiUrl = "https://api-lw4.learnworlds.com";
        static string ApiAuthUrl = "https://api-lw4.learnworlds.com/oauth2";
        static string AuthAction = "access_token";
        static string ClientId = "5ed67bb169a9630f78325260";
        static string ClientSecret = "J4COSB467zwHn74QsP5dRcpnegjpQwcdUrLFq695UueL3BZzxX";

        static string AccessToken = "BW3Q8sw1hDqgCGc9mj91eGR0iQWJLxsvQ3aWBAxJ";

        static Token Authtoken { get; set; }




        internal static string APIAuthenticate()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(ApiAuthUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Lw-Client", ClientId);

            Dictionary<string, string> odic = new Dictionary<string, string>();
            odic["client_id"] = ClientId;
            odic["client_secret"] = ClientSecret;
            odic["grant_type"] = "client_credentials";

            var ret = client.PostAsync(AuthAction, new FormUrlEncodedContent(odic));
            ret.Wait();
            HttpResponseMessage response = ret.Result;
            var jsonContent = response.Content.ReadAsStringAsync();
            jsonContent.Wait();
            Token tok = JsonConvert.DeserializeObject<Token>(jsonContent.Result);

            //obtem o token de volta
            if (tok.success)
            {
                Authtoken = tok;
                return null;
            }
            else
            {
                return string.Join(",", tok == null ? new string[] { "nulo" } : tok.strErrors);
            }
        }

        internal static string GetToken(out string error)
        {
            error = null;

            if (AccessToken != null)
                return AccessToken;

            if (Authtoken == null || Authtoken.IsValid() == false)
                error = APIAuthenticate();

            if (error != null)
                return null;

            return Authtoken.tokenData.access_token;
        }


        public class Token
        {
            public TokenData tokenData { get; set; }
            public ErrorData[] errors { get; set; }
            public bool success { get; set; }

            public string[] strErrors
            {
                get
                {
                    var ret = new List<string>();
                    if (errors != null)
                        errors.ToList().ForEach(e => ret.Add(e.ToString()));

                    return ret.ToArray();
                }
            }

            public class TokenData
            {
                public string access_token { get; set; }
                public string token_type { get; set; }
                public int expires_in { get; set; }
                public DateTime GenerationDate { get; set; } = DateTime.Now;
            }



            public class ErrorData
            {
                public string code { get; set; }
                public string context { get; set; }
                public string message { get; set; }

                public override string ToString()
                {
                    return (code ?? "") + "-" + (context ?? "") + "-" + (message ?? "");
                }
            }

            internal bool IsValid()
            {
                return (success && tokenData.GenerationDate.AddSeconds(tokenData.expires_in) > DateTime.Now);
            }
        }

        internal static T CallService<T>(string action, out string error, params KeyValuePair<string, object>[] arParams)
        {
            error = null;

            var tok = GetToken(out error);
            if (error != null)
                return default;

            var client = new HttpClient();
            client.BaseAddress = new Uri(ApiUrl);


            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Lw-Client", ClientId);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tok);

            var ret = client.PostAsJsonAsync(action, arParams.ToDictionaryStringObject());
            ret.Wait();
            HttpResponseMessage response = ret.Result;

            var jsonContent = response.Content.ReadAsStringAsync();
            jsonContent.Wait();
            T retData = JsonConvert.DeserializeObject<T>(jsonContent.Result);

            return retData;
        }


    }
}
