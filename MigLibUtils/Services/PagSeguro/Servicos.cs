using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Uol.PagSeguro;
using Uol.PagSeguro.Domain;
using Uol.PagSeguro.Resources;
using Uol.PagSeguro.Service;

namespace MigLibUtils.Services.PagSeguro
{
    public class Servicos
    {

        public static string[] pMeuEmail { internal get; set; } = { "administracao@migalhas.com.br", "" }; // Colque seu email de cadastro do PagSeguro aqui
        public static string[] pMeuToken { internal get; set; } = { "1FF654F1E8DD4E7A8907142ED3FC9E1F", "" }; // Coloque seu token de acesso do PagSeguro aqui
        public static int iDefaulMaxPerPage = 100;


        public static RetornoServicos GerarPagamentoRedirect(string pReferencia, string pEmail, string pNome,
                                                    string pDDD, string pTelefone, double pValor, string pTituloPagamento,
                                                    string pProductId, int pItemQtd, out string error)
        {
            error = null;

            try
            {

                Dados dadosEnvio = new Dados
                {
                    MeuEmail = pMeuEmail[0],
                    MeuToken = pMeuToken[0],
                    TituloPagamento = pTituloPagamento,
                    Nome = pNome,
                    Email = pEmail,
                    DDD = pDDD,
                    NumeroTelefone = pTelefone,
                    Referencia = pReferencia,
                    Valor = pValor.ToString("#.00"),
                    ProductId = pProductId,
                    Qtd = pItemQtd
                };

                dadosEnvio = Processamento.GerarPagamento(dadosEnvio);

                if (!dadosEnvio.stringConexao.Equals(string.Empty))
                {
                    return new RetornoServicos()
                    {
                        Code = dadosEnvio.CodigoAcesso,
                        StringConexao = dadosEnvio.stringConexao
                    };
                }

                error = "Não gerado." + dadosEnvio.Erro;

                return null;
            }
            catch (Exception ex)
            {
                error = ex.ToString();
            }

            return null;

        }

        public static List<Transaction> BuscaPorData(int pIdConta, DateTime pDataDesde, out string error, DateTime? pDataAte = null, int? ItemsPerPage = null, string StatusCodes = "")
        {
            error = null;

            try
            {
                AccountCredentials credentials = PagSeguroConfiguration.Credentials(false);

                credentials.Email = pMeuEmail[pIdConta];
                credentials.Token = pMeuToken[pIdConta];

                var ret = new List<Transaction>();

                bool hasResult = true;
                int page = 1;

                StatusCodes = (StatusCodes == null || StatusCodes == "" ? "" : "," + StatusCodes + ",");
                StatusCodes = StatusCodes.Replace(" ", "");


                do
                {


                    TransactionSearchResult result =
                        TransactionSearchService.SearchByDate(
                            credentials,
                            pDataDesde,
                            pDataAte == null ? DateTime.Now : pDataAte.Value,
                            page,
                            ItemsPerPage == null ? iDefaulMaxPerPage : ItemsPerPage.Value);

                    if (result.Transactions.Count > 0)
                    {
                        foreach (TransactionSummary transactionsummary in result.Transactions)
                        {
                            if (StatusCodes == "" || StatusCodes.Contains("," + transactionsummary.TransactionStatus.ToString() + ","))
                            {
                                Transaction transaction = TransactionSearchService.SearchByCode(credentials, transactionsummary.Code);
                                ret.Add(transaction);
                            }



                        }

                    }

                    page++;
                    hasResult = (page <= result.TotalPages);

                } while (hasResult);

                return ret;

            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return null;
            }
        }

        public static List<Transaction> BuscaPorCodigo(int pIdConta, string Codigo, out string error)
        {
            error = null;

            try
            {
                AccountCredentials credentials = PagSeguroConfiguration.Credentials(false);

                credentials.Email = pMeuEmail[pIdConta];
                credentials.Token = pMeuToken[pIdConta];

                var ret = new List<Transaction>();



                Transaction result =
                    TransactionSearchService.SearchByCode(credentials, Codigo);

                if (result != null)
                    ret.Add(result);
                return ret;

            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return null;
            }
        }

        private static T ConvertXmlParaObjeto<T>(string sXml)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(new StringReader(sXml));

        }



        public class RetornoServicos
        {
            public string Code { get; internal set; }
            public string StringConexao { get; internal set; }

        }





    }
}
