using APIBase.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MigLibUtils.Services
{
    public class OneSignal
    {
        internal static string server { get; set; } = "https://onesignal.com/api/v1/notifications";
        internal static string onesignalkey { get; set; } = "381532fd-302f-46ef-a5ae-e49228cddb1d";
        internal static string onesignalrestkey { get; set; } = "YzA3OWIzMzMtYmM1ZC00Yzc0LWEwMDEtNWRjNjRiOWNiMjM3";


        public static string EnviaMensagemGeral(string sTitulo,
                                                string sSubTitulo,
                                                string sMensagem,
                                                out string error,
                                                string sImagem = null,
                                                string sLinkURL = null,
                                                Dictionary<string, string> parmData = null,
                                                string[] aSegments = null,
                                                string[] aPlayerids = null)
        {
            if (aSegments == null)
                aSegments = new string[] { "Subscribed Users" }; //todos os assinantes

            error = null;

            var request = WebRequest.Create(server) as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Authorization", "Basic " + onesignalrestkey);
            var serializer = new JavaScriptSerializer();
            var obj = new Dictionary<string, object>();

            obj["app_id"] = onesignalkey;

            obj["contents"] = new { en = sMensagem };

            if (aPlayerids != null)
                obj["include_player_ids"] = aPlayerids;
            else
                obj["included_segments"] = aSegments;

            if (sTitulo != null)
                obj["headings"] = new { en = sTitulo };
            if (sSubTitulo != null)
                obj["subtitle"] = new { en = sSubTitulo };
            if (parmData != null && parmData.Count > 0)
                obj["data"] = parmData;
            if (sLinkURL != null)
                obj["url"] = sLinkURL;
            if (sImagem != null)
            {
                obj["ios_attachments"] = new KeyValuePair<string, string>("id", sImagem).ToDictionary();
                obj["big_picture"] = sImagem;
            }

            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                        //converter o retorno 
                    }
                }
            }
            catch (WebException ex)
            {
                error = ex.Message + "\r\n";
                error += new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            }

            return responseContent;


        }

    }
}
