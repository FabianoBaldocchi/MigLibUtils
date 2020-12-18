using APIBase.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace MigLibUtils.Services.PagSeguro
{
    class Processamento
    {

        // Gerar pagamento do PagSeguro
        public static Dados GerarPagamento(Dados dados = null)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            if (dados == null) return null;
            dados.stringConexao = "";
            try
            {
                //URI de checkout.
                string uri = @"https://ws.pagseguro.uol.com.br/v2/checkout";
                //Conjunto de parâmetros/formData.
                var nome = dados.Nome.ReplaceDiacritics();

                System.Collections.Specialized.NameValueCollection postData =
                    new System.Collections.Specialized.NameValueCollection
                    {
                        {"email", dados.MeuEmail},
                        {"token", dados.MeuToken},
                        {"currency", "BRL"},
                        {"itemId1", dados.ProductId},
                        {"itemDescription1",  dados.TituloPagamento.ReplaceDiacritics().Left(100)},
                        {"itemAmount1", dados.Valor.Replace(",",".")},
                        {"itemQuantity1",dados.Qtd.ToString()},
                        {"itemWeight1", (dados.Peso.ToString()).PadLeft(3,'0')},
                        {"reference", dados.Referencia},
                        {"senderName", nome.Left(50)},
                        {"senderAreaCode", dados.DDD},
                        {"senderPhone", dados.NumeroTelefone},
                        {"senderEmail", dados.Email},
                        {"shippingAddressRequired", dados.SolicitaEnderecoEnvio.ToString().ToLower()}
                    };
                //String que receberá o XML de retorno.
                string xmlString = null;
                //Webclient faz o post para o servidor de pagseguro.
                using (WebClient wc = new WebClient())
                {
                    //Informa header sobre URL.
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    wc.Headers[HttpRequestHeader.AcceptCharset] = "ISO-8859-1";
                    wc.Headers[HttpRequestHeader.ContentEncoding] = "ISO-8859-1";

                    //Faz o POST e retorna o XML contendo resposta do servidor do pagseguro.
                    byte[] result;
                    try
                    {
                        result = wc.UploadValues(uri, postData);
                    }
                    catch (Exception ex)
                    {
                        var str = "";
                        postData.AllKeys.ToList().ForEach(k => str += k + ":" + postData[k].ToString() + "|");
                        throw new Exception("URI:" + uri + "|NOME(SA): " + nome + "|POSTDATA:" + str, ex);
                    }
                    //Obtém string do XML.
                    xmlString = Encoding.ASCII.GetString(result);
                }
                //Cria documento XML.
                XmlDocument xmlDoc = new XmlDocument();
                //Carrega documento XML por string.
                xmlDoc.LoadXml(xmlString);
                //Obtém código de transação (Checkout).
                var code = xmlDoc.GetElementsByTagName("code")[0];
                //Monta a URL para pagamento.                
                if (!code.InnerText.Equals(""))
                {
                    dados.CodigoAcesso = code.InnerText;
                    dados.stringConexao = string.Concat("https://pagseguro.uol.com.br/v2/checkout/payment.html?code=", code.InnerText);
                }
            }
            catch (Exception ex)
            {
                dados.CodigoAcesso = "";
                dados.stringConexao = "";
                dados.Erro = ex.ToString();
            }
            // Retorna com a URL para carregar na tela

            return dados;
        }

        // Validar situação do pagamento
        public static Dados ValidarPagamento(Dados dados = null)
        {
            if (dados == null) return null;
            Dados retorno = new Dados();
            try
            {
                //uri de consulta da transação.


                string uri = "https://ws.pagseguro.uol.com.br/v3/transactions/notifications/" + dados.CodigoAcesso +
                             "?email=" + dados.MeuEmail + "&token=" + dados.MeuToken;
                //Classe que irá fazer a requisição GET.
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                //Método do webrequest.
                request.Method = "GET";
                //String que vai armazenar o xml de retorno.
                string xmlString = null;
                //Obtém resposta do servidor.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    //Cria stream para obter retorno.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        //Lê stream.
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            //Xml convertido para string.
                            xmlString = reader.ReadToEnd();
                            //Cria xml document para facilitar acesso ao xml.
                            XmlDocument xmlDoc = new XmlDocument();
                            //Carrega xml document através da string com XML.
                            xmlDoc.LoadXml(xmlString);
                            //Busca elemento status do XML.
                            var status = xmlDoc.GetElementsByTagName("status")[0];
                            //Fecha reader.
                            reader.Close();
                            //Fecha stream.
                            dataStream.Close();
                            //Verifica status de retorno.
                            //3 = Pago. Outas Tags verificar na documentação no site do PagSeguro
                            retorno.Status = status.InnerText;
                        }
                    }
                    return retorno;
                }
            }
            catch
            {
                return null;
            }
        }

        public static Dados FazerConsulta(Dados dados, int? page = null, string transactioncode = null)
        {
            if (dados == null) return null;
            Dados retorno = new Dados();
            try
            {
                //uri de consulta da transação.


                string uri = "https://ws.pagseguro.uol.com.br/v2/transactions{codigo}" +
                             "?email=" + dados.MeuEmail + "&token=" + dados.MeuToken;

                if (page != null)
                    uri += "&maxPageResults=" + dados.maxPageResults.ToString() + "&page" + page.ToString();
                else if (transactioncode != null)
                    uri = uri.Replace("{codigo}", "/" + transactioncode);
                else
                    uri += "&maxPageResults=" + dados.maxPageResults.ToString() + "&initialDate=" + dados.DataDesde.ToString("yyyy-MM-ddTHH:mm");

                uri = uri.Replace("{codigo}", "");

                //Classe que irá fazer a requisição GET.
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                //Método do webrequest.
                request.Method = "GET";
                //String que vai armazenar o xml de retorno.
                string xmlString = null;
                //Obtém resposta do servidor.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    //Cria stream para obter retorno.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        //Lê stream.
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            //Xml convertido para string.
                            xmlString = reader.ReadToEnd();
                            //Cria xml document para facilitar acesso ao xml.

                            //Carrega xml document através da string com XML.
                            retorno.xmlDoc.LoadXml(xmlString);
                            //busca os casos de tratamento

                            //Fecha reader.
                            reader.Close();
                            //Fecha stream.
                            dataStream.Close();
                        }

                    }
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                retorno.Erro = ex.ToString();
                return dados;
            }
        }


    }
}
