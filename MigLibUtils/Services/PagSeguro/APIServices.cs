using APIBase.Utils;
using MigLibUtils.Services.PagSeguro.BizClasses;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MigLibUtils.Services.PagSeguro
{
    public class APIServices
    {
        public static string[] pMeuEmail { internal get; set; } = { "administracao@migalhas.com.br", "" }; // Colque seu email de cadastro do PagSeguro aqui
        public static string[] pMeuToken { internal get; set; } = { "1FF654F1E8DD4E7A8907142ED3FC9E1F", "" }; // Coloque seu token de acesso do PagSeguro aqui
        public static string pUrlBase { internal get; set; } = "https://ws.pagseguro.uol.com.br/v2/"; // Coloque seu token de acesso do PagSeguro aqui
        public static Dictionary<string, string> pSpecificUrlBaseByAction = new Dictionary<string, string>();

        public static string[] pMeuEmailSandBox { internal get; set; } = { "administracao@migalhas.com.br", "" }; // Colque seu email de cadastro do PagSeguro aqui
        public static string[] pMeuTokenSandBox { internal get; set; } = { "F7C7DFB946DC47B980CF9F6DF65528E4", "" }; // Coloque seu token de acesso do PagSeguro aqui
        public static string pUrlBaseSandBox { internal get; set; } = "https://ws.sandbox.pagseguro.uol.com.br/v2/"; // Coloque seu token de acesso do PagSeguro aqui
        public static Dictionary<string, string> pSpecificUrlBaseSandBoxByAction = new Dictionary<string, string>();



        static APIServices()
        {
            pSpecificUrlBaseByAction["pre-approvals"] = "https://ws.pagseguro.uol.com.br/";

            pSpecificUrlBaseSandBoxByAction["pre-approvals"] = "https://ws.sandbox.pagseguro.uol.com.br/";
        }


        public bool IsSandBox = true;
        public int ContractId = 0;


        public APIServices() { }

        public APIServices(bool issandbox, int contractdid)
        {
            IsSandBox = issandbox;
            ContractId = contractdid;
        }

        public string GetSessionToken(out string sessiontoken, out RestTrace restTrace)
        {
            sessiontoken = null;

            var ret = RestCall(out restTrace, null, "sessions", null, true, "application/xml", "*/*");
            if (ret == null)
                return "Erro ao obter retorno.";

            try
            {
                var r = XDocument.Parse(ret.ToString()).XPathSelectElement("/session/id").CreateReader();
                r.MoveToContent();
                sessiontoken = r.ReadInnerXml();
                return null;
            }
            catch (Exception ex)
            {

                return "Excecao: " + ex.Message + "|ret=" + ret;
            }

        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string RegisterRecurrencePlan(PGRecurrencePlan plan, out string plancode, out RestTrace restTrace)
        {
            plancode = null;

            var req = new PreApprovalRequestCreatePlan()
            {
                preApprovalRequest = plan
            };

            var xmlbody = req.XMLString("//preApprovalRequest", true);

            return RegisterRecurrencePlan(xmlbody, out plancode, out restTrace);
        }

        public string RegisterRecurrencePlan(Dictionary<string, object> dicParms, out string plancode, out RestTrace restTrace)
        {
            plancode = null;

            var sett = new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter> { new DecimalFormatConverter() },
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                Formatting = Formatting.None,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            var strjson = dicParms.JSONString(true, sett);


            var doc = JsonConvert.DeserializeXmlNode(strjson);

            return RegisterRecurrencePlan(doc.InnerXml, out plancode, out restTrace);

        }

        public string RegisterRecurrencePlan(string xmlbody, out string plancode, out RestTrace restTrace)
        {
            plancode = null;

            var ret = RestCall(out restTrace,null, "pre-approvals/request", xmlbody, true, "application/xml", "application/vnd.pagseguro.com.br.v3+xml;charset=ISO-8859-1");

            if (ret == null)
                return "Erro ao obter retorno.";

            try
            {
                var r = XDocument.Parse(ret.ToString()).XPathSelectElement("/preApprovalRequest/code").CreateReader();
                r.MoveToContent();
                plancode = r.ReadInnerXml();
                return null;
            }
            catch (Exception ex)
            {
                return "Excecao: " + ex.Message + "|ret=" + ret;
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string SubscribePlan(PreApprovalRequest req, out string subscode, out RestTrace restTrace)
        {
            subscode = null;

            var jsonbody = req.JSONString(true);

            return SubscribePlan(jsonbody, out subscode, out restTrace);
        }

        public string SubscribePlan(Dictionary<string, object> dicParms, out string subscode, out RestTrace restTrace)
        {
            subscode = null;

            var jsonbody = dicParms.JSONString();
            return SubscribePlan(jsonbody, out subscode, out restTrace);


        }

        public string SubscribePlan(string jsonbody, out string subscode, out RestTrace restTrace)
        {
            subscode = null;

            var ret = RestCall(out restTrace,null, "pre-approvals", jsonbody, true, "application/json", "application/vnd.pagseguro.com.br.v1+json;charset=ISO-8859-1");

            if (ret == null)
                return "Erro ao obter retorno.";

            try
            {
                var r = ret.JSONDic();
                subscode = r["code"].ToString();
                return null;
            }
            catch (Exception ex)
            {
                return "Excecao: " + ex.Message + "|ret=" + ret;
            }


        }

        public string SuspendSubscription(string code, out RestTrace restTrace)
        {


            var ret = RestCall(out restTrace,null, "pre-approvals/" + code + "/status", (new { status = "SUSPENDED" }).JSONString(), true, "application/json", "application/vnd.pagseguro.com.br.v3+json;charset=ISO-8859-1", "PUT");

            if (ret == null)
                return "Erro ao obter retorno.";

            return null;
        }

        public string CancelSubscription(string code, out RestTrace restTrace)
        {
            var ret = RestCall(out restTrace,null, "pre-approvals/" + code + "/cancel", "", true, "application/json", "application/vnd.pagseguro.com.br.v3+json;charset=ISO-8859-1", "PUT");

            if (ret == null)
                return "Erro ao obter retorno.";

            return null;

        }

        public string ResumeSubscription(string code, out RestTrace restTrace)
        {
            var jsonbody = (new { status = "ACTIVE" }).JSONString();

            var ret = RestCall(out restTrace,null, "pre-approvals/" + code + "/status", jsonbody, true, "application/json", "application/vnd.pagseguro.com.br.v3+json;charset=ISO-8859-1", "PUT");

            if (ret == null)
                return "Erro ao obter retorno.";

            return null;

        }

        public string DiscountPaymentSubscription(string code, decimal pctdiscount, out RestTrace restTrace)
        {
            var jsonbody = (new { type = "DISCOUNT_PERCENT", value = pctdiscount }).JSONString();

            var ret = RestCall(out restTrace,null, "pre-approvals/" + code + "/discount", jsonbody, true, "application/json", "application/vnd.pagseguro.com.br.v3+json;charset=ISO-8859-1", "PUT");

            if (ret == null)
                return "Erro ao obter retorno.";

            return null;

        }

        public string ChangePaymentMethodSubscription(string code, PreApprovalChangeMethod req, out RestTrace restTrace)
        {
            var jsonbody = req.JSONString(true);

            var ret = RestCall(out restTrace,null, "pre-approvals/" + code + "/payment-method", jsonbody, true, "application/json", "application/vnd.pagseguro.com.br.v3+json;charset=ISO-8859-1", "PUT");

            if (ret == null)
                return "Erro ao obter retorno.";

            return null;

        }

        public string GetSubscription(string code, out PreapprovalResponse resp, out RestTrace restTrace)
        {
            resp = null;

            var ret = RestCall(out restTrace,null, "pre-approvals/" + code, "", true, "application/json", "application/vnd.pagseguro.com.br.v3+json;charset=ISO-8859-1", "GET");

            if (ret == null)
                return "Erro ao obter retorno.";

            resp = JsonConvert.DeserializeObject<PreapprovalResponse>(ret);

            return null;

        }

        public string GetPaymentOrders(string code, out PaymentOrdersResponse orders, out RestTrace restTrace)
        {
            orders = null;

            var ret = RestCall(out restTrace,null, "pre-approvals/" + code + "/payment-orders", "", true, "application/json", "application/vnd.pagseguro.com.br.v3+json;charset=ISO-8859-1", "GET");


            if (ret == null)
                return "Erro ao obter retorno.";

            orders = JsonConvert.DeserializeObject<PaymentOrdersResponse>(ret);

            return null;

        }

        public string RetryPayment(string code, string payordercode, PreApprovalChangeMethod retrydata, out RestTrace restTrace)
        {

            var ret = RestCall(out restTrace, null, "pre-approvals/" + code + "/payment-orders/" + payordercode + "/payment", retrydata.JSONString(true), true, "application/json", "application/vnd.pagseguro.com.br.v3+json;charset=ISO-8859-1", "POST");

            if (ret == null)
                return "Erro ao obter retorno.";

            return null;

        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public string Buy(BizClasses.Buy.payment pay, out string transactioncode, out transaction transactiondata, out RestTrace restTrace)
        {
            transactiondata = null;
            transactioncode = null;

            var xmlnbody = pay.XMLString("//payment", true);

            return Buy(xmlnbody, out transactioncode, out transactiondata, out restTrace);
        }


        public string Buy(string xmlbody, out string transactioncode, out transaction transactiondata, out RestTrace restTrace)
        {
            transactiondata = null;
            transactioncode = null;

            var ret = RestCall(out restTrace, null, "transactions", xmlbody, true, "application/xml", "*/*");

            if (ret == null)
                return "Erro ao obter retorno.";

            try
            {
                if (XDocument.Parse(ret.ToString()).XPathSelectElement("/errors/error") != null)
                {
                    return ret;
                }

                var r = XDocument.Parse(ret.ToString()).XPathSelectElement("/transaction/code").CreateReader();
                r.MoveToContent();
                transactioncode = r.ReadInnerXml();
                transactiondata = ret.FromXMLString<transaction>();
                return null;
            }
            catch (Exception ex)
            {
                return "Excecao: " + ex.Message + "|ret=" + ret;
            }


        }

        //----------------------------------------------------------------------------------------------------------------------------
        public string CheckTransaction(string code, out transaction tran, out RestTrace restTrace)
        {
            tran = null;
            var ret = RestCall(out restTrace, null, "transactions/" + code, "", true, "application/json", "*/*", "GET");

            if (ret == null)
                return "Erro ao obter retorno.";

            tran = ret.FromXMLString<transaction>();

            return null;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //headers accept
        //"*/*"
        //"application/vnd.pagseguro.com.br.v1+json;charset=ISO-8859-1"
        string RestCall(out RestTrace restTrace, string url, string action_parms, string body, 
                        bool add_auth_token = true, string contenttype = "application/xml", 
                        string HeaderAccept = null, string method = "POST", int clientTimeout = 30000)
        {
            restTrace = new RestTrace()
            {
                Body = body,
                Method = method,
                Url = (url ?? "") + action_parms,
                ExecDate = DateTime.Now
            };
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                if (contenttype == null)
                    contenttype = "application/xml";

                if (url == null)
                {
                    url = IsSandBox ? pUrlBaseSandBox : pUrlBase;

                    if (IsSandBox && pSpecificUrlBaseSandBoxByAction.ContainsKey(action_parms.Split('/')[0]))
                        url = pSpecificUrlBaseSandBoxByAction[action_parms.Split('/')[0]];
                    else if (!IsSandBox && pSpecificUrlBaseByAction.ContainsKey(action_parms.Split('/')[0]))
                        url = pSpecificUrlBaseByAction[action_parms.Split('/')[0]];
                }

                if (add_auth_token)
                    action_parms += "?email=" + (IsSandBox ? pMeuEmailSandBox[ContractId] : pMeuEmail[ContractId]) + "&token=" + (IsSandBox ? pMeuTokenSandBox[ContractId] : pMeuToken[ContractId]);

                url = url + (url.EndsWith("/") ? "" : "/") + action_parms;

                restTrace.Url = url;

                var client = new RestClient(url);

                client.Timeout = clientTimeout;

                Method mtd = (Method)Enum.Parse(typeof(Method), method);

                var request = new RestRequest(mtd);
                request.AddHeader("Content-Type", contenttype);
                restTrace.Headers.Add(new KeyValuePair<string, string>("Content-Type", contenttype));

                if (HeaderAccept != null)
                {
                    request.AddHeader("Accept", HeaderAccept);
                    restTrace.Headers.Add(new KeyValuePair<string, string>("Accept", HeaderAccept));
                }

                request.AddParameter(contenttype,
                    body,
                    ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);

                restTrace.MilisecondExec = DateTime.Now.Subtract(restTrace.ExecDate).TotalMilliseconds;
                if (!response.IsSuccessful)
                {
                    restTrace.ExecException = response.ErrorException;
                    restTrace.ResponseHttpCode = System.Net.HttpStatusCode.ExpectationFailed;
                    restTrace.Response = response.ErrorMessage;
                    if (restTrace.Response == null)
                        restTrace.Response = response.Content;

                    return null;
                }

                restTrace.ResponseHttpCode = response.StatusCode;
                restTrace.Response = response.Content;

                return response.Content;

            }
            catch (Exception ex)
            {
                restTrace.MilisecondExec = DateTime.Now.Subtract(restTrace.ExecDate).TotalMilliseconds;
                restTrace.ExecException = ex;
                return null;
            }
        }

    }

    public class RestTrace
    {
        public DateTime ExecDate { get; set; }
        public double MilisecondExec { get; set; }
        public System.Net.HttpStatusCode ResponseHttpCode { get; set; }
        public string Url { get; set; }
        public List<KeyValuePair<string, string>> Headers = new List<KeyValuePair<string, string>>();
        public string Method { get; set; }
        public string Body { get; set; }
        public string Response { get; set; }
        public Exception ExecException { get; set; }
    }

    public class DecimalFormatConverter : JsonConverter
    {


        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal) || objectType == typeof(decimal?) || objectType == typeof(double) || objectType == typeof(double?) || objectType == typeof(float);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue($"{value:0.00}".Replace(",", "."));
        }

        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
