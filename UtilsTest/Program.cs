using ContentUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Search.EntitySearch;
using Microsoft.Azure.CognitiveServices.Search.EntitySearch.Models;
using Newtonsoft.Json;
using MigLibUtils.Extensions;
using Uol.PagSeguro.Resources;
using MigLibUtils.Services.PagSeguro;
using System.Data.SqlTypes;
using MigLibUtils.Services.PagSeguro.BizClasses;

namespace UtilsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var arqnovo = "C:\\Users\\Rafael\\OneDrive\\Documentos\\_TEMP\\TESTE_ARTIGOS_WORD\\GERADO_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".html";
                var arqnovowar = "C:\\Users\\Rafael\\OneDrive\\Documentos\\_TEMP\\TESTE_ARTIGOS_WORD\\GERADO_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".txt";

                Console.WriteLine("Informe o arquivo:");
                var arq = Console.ReadLine();

                //------------------
                //var converter = new Mammoth.DocumentConverter();

                //var result = converter.ConvertToHtml(arq);

                //var html = result.Value; // The generated HTML
                //var warnings = result.Warnings; // Any warnings during conversion

                //File.WriteAllText(arqnovo, html, Encoding.GetEncoding("windows-1252"));
                //File.WriteAllText(arqnovowar, JsonConvert.SerializeObject(warnings, Formatting.Indented), Encoding.GetEncoding("windows-1252"));

                var conteudo = MigLibUtils.FileProcess.Word.ToHTML(Convert.ToBase64String(File.ReadAllBytes(arq)), arq);
                conteudo = ContentUtils.HTMLFormat.CleanHtmlFromWord(conteudo, return_only_body: false);
                              
                File.WriteAllText(arqnovo, conteudo, Encoding.GetEncoding("windows-1252"));
                Process.Start(arqnovo);
            }
        }

        static void MainPAGSEGURO(string[] args)
        {
            Console.WriteLine("informe 1 (sandbox) 2 (producao)  == DEFAULT 1:");
            var opt = Console.ReadKey();
            var issandbox = (opt == new ConsoleKeyInfo('1', ConsoleKey.D1, false, false, false) || opt == new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false) ? true : false);

            var apic = new APIServices(issandbox, 0);


            Console.WriteLine("informe OPCAO:");
            Console.WriteLine("1) gerar session");
            Console.WriteLine("2) gerar plano");
            Console.WriteLine("3) consultar ordens");
            Console.WriteLine("4) status transação");
            Console.WriteLine("5) consultar status da adesão");
            Console.WriteLine("6) retry de ordem de pagamento");

            var mnu = Console.ReadKey();
            Console.WriteLine("-----------------------");

            if (mnu == new ConsoleKeyInfo('1', ConsoleKey.D1, false, false, false))
            {
                var ret = apic.GetSessionToken(out string sessiontoken, out RestTrace restTrace);

                if (ret != null)
                    Console.WriteLine("ERRO:" + ret);

                else
                    Console.WriteLine("sessao:" + sessiontoken);
            }
            else if (mnu == new ConsoleKeyInfo('2', ConsoleKey.D2, false, false, false))
            {
                var plan = new MigLibUtils.Services.PagSeguro.BizClasses.PGRecurrencePlan()
                {
                    preApproval = new MigLibUtils.Services.PagSeguro.BizClasses.PGRecurrencePlan.preApprovalData()
                    {
                        amountPerPayment = 19.90M,
                        cancelURL = null,
                        charge = "AUTO",
                        membershipFee = null,
                        name = "PLANO TESTE " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                        period = "MONTHLY",
                        reference = "PTESTE-" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                        trialPeriodDuration = null

                    },
                    maxUses = null,
                    reviewURL = null
                };


                var ret = apic.RegisterRecurrencePlan(plan, out string plancode, out RestTrace restTrace);

                if (ret != null)
                    Console.WriteLine("ERRO:" + ret);
                else
                    Console.WriteLine("codigo:" + plancode);

            }
            else if (mnu == new ConsoleKeyInfo('3', ConsoleKey.D3, false, false, false))
            {
                Console.WriteLine("informe o pre-approval");
                var code = Console.ReadLine();
                var ret = apic.GetPaymentOrders(code, out PaymentOrdersResponse orders, out RestTrace restTrace);

                if (ret != null)
                    Console.WriteLine("ERRO:" + ret);
                else
                    Console.WriteLine(orders.JSONString(true));
            }
            else if (mnu == new ConsoleKeyInfo('4', ConsoleKey.D4, false, false, false))
            {
                Console.WriteLine("informe o código da transacao");
                var code = Console.ReadLine();
                var ret = apic.CheckTransaction(code, out transaction tran, out RestTrace restTrace);

                if (ret != null)
                    Console.WriteLine("ERRO:" + ret);
                else
                    Console.WriteLine(tran.JSONString(true));

            }
            else if (mnu == new ConsoleKeyInfo('5', ConsoleKey.D5, false, false, false))
            {
                Console.WriteLine("informe o pre-approval");
                var code = Console.ReadLine();
                var ret = apic.GetSubscription(code, out PreapprovalResponse data, out RestTrace restTrace);

                if (ret != null)
                    Console.WriteLine("ERRO:" + ret);
                else
                    Console.WriteLine(data.JSONString(true));

            }
            else if (mnu == new ConsoleKeyInfo('6', ConsoleKey.D6, false, false, false))
            {
                Console.WriteLine("informe o pre-approval");
                var code = Console.ReadLine();

                Console.WriteLine("informe o payorder");
                var codepay = Console.ReadLine();

                var retrydata = new PreApprovalChangeMethod()
                {
                    type = "CREDITCARD",
                    sender = new Sender()
                    {
                        ip = "77.54.97.39",
                        hash = null
                    },
                    creditCard = JsonConvert.DeserializeObject<Creditcard>("{ \"token\": \"04af49e838d34c25b8f8a41c61254db5\", " +
                            "\"holder\": { \"name\": \"MARCIO MADUREIRA\", \"birthDate\": \"12/07/1971\", " +
                            "\"documents\": [ { \"type\": \"CPF\", \"value\": \"01198447702\" } ], " +
                            "\"billingAddress\": { \"street\": \"Avenida Rio Branco\", \"number\": \"110\", \"complement\": \"33 andar\", \"district\": \"Centro\", \"city\": \"Rio de Janeiro\", \"state\": \"RJ\", \"country\": \"BRA\", \"postalCode\": \"20040001\" }, " +
                            "\"phone\": { \"areaCode\": \"21\", \"number\": \"988766686\" } " +
                           "}" +
                           "}")
                };

                var ret = apic.RetryPayment(code, codepay, retrydata, out RestTrace restTrace);

                if (ret != null)
                    Console.WriteLine("ERRO:" + ret + restTrace.JSONString(true));
                else
                    Console.WriteLine("SUCESSO.");

            }
            Console.ReadLine();



        }


        static void MainWord(string[] args)
        {
            Console.WriteLine("informe um arquivo Word:");
            var arquivo = Console.ReadLine();

            var cont = Convert.ToBase64String(File.ReadAllBytes(arquivo));

            cont = MigLibUtils.FileProcess.Word.ToHTML(cont, arquivo);

            cont = HTMLFormat.CleanAndRepair(cont, false, null, null, null, true, false, true, true);

            arquivo = arquivo + ".HTML_" + DateTime.Now.ToString("yyyy_MM_DD_HH_mm_ss") + ".html";
            File.WriteAllText(arquivo, cont, Encoding.GetEncoding("windows-1252"));

            Process.Start(arquivo);
            Console.WriteLine("arquivo:  " + arquivo);
            Console.ReadLine();

        }


        static void MainPagSeguro(string[] args)
        {
            PagSeguroConfiguration.UrlXmlConfiguration = "C:\\VisualStudio\\migalhasAPI\\arquivos\\PagSeguroConfig.xml";

            var l = MigLibUtils.Services.PagSeguro.Servicos.BuscaPorData(0, DateTime.Parse("2020-07-15"), out string error);

            Console.WriteLine(l.JSONString(true));
            Console.WriteLine(error ?? "");
            Console.ReadLine();


        }

        static void MainEntity(string[] args)
        {
            while (true)
            {
                Console.WriteLine("informe uma entidade:");
                var entidade = Console.ReadLine();
                //Console.WriteLine(WikipediaIntegration.Search.SimpleSearch(entidade));

                var ret = AzureIntegration.Search.SearchEntity(entidade);

                try
                {
                    Console.WriteLine(ret.Name);
                    Console.WriteLine(ret.Description);
                    Console.WriteLine(ret.ImageUrl);
                    Console.WriteLine(ret.ImageUrlThumbnail);
                    Console.WriteLine(ret.GeneralContent);
                    Console.ReadLine();
                }
                finally
                {
                    Console.Clear();
                }

            }
        }


        static void Main3(string[] args)
        {
            while (true)
            {
                Console.WriteLine("informe uma entidade:");
                var entidade = Console.ReadLine();
                var client = new EntitySearchAPI(new ApiKeyServiceClientCredentials("59af8519d4594b538dd75698d72905a3"));
                var entityData = client.Entities.Search(query: entidade, market: "pt-BR");
                if (entityData.Entities == null)
                {
                    Console.WriteLine("========= NAO LOCALIZADO ==============");
                    Console.WriteLine(JsonConvert.SerializeObject(entityData, Formatting.Indented));

                }
                else
                {
                    var mainEntity = entityData.Entities.Value.Where(thing => thing.EntityPresentationInfo.EntityScenario == EntityScenario.DominantEntity).FirstOrDefault();
                    Console.WriteLine(mainEntity.Description);
                    Console.WriteLine(JsonConvert.SerializeObject(mainEntity, Formatting.Indented));

                }

                Console.ReadKey();
                Console.Clear();
            }

        }


        static void Main2(string[] args)
        {

            while (true)
            {
                Console.WriteLine("== encode ");
                var enc = Console.ReadLine();

                Console.WriteLine("== 1 - html    2 - arquivo ");
                var op = Console.ReadLine();


                string arq;

                if (op == "2")
                {
                    Console.WriteLine("==informe o caminho completo do arquivo:");
                    arq = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("== digitar html ");
                    arq = Console.ReadLine();
                }

                if (op == "2" && !File.Exists(arq))
                {
                    Console.WriteLine("arquivo nao existe.");
                }
                else
                {
                    try
                    {
                        #region ENCODE
                        //                //
                        //                // Resumo:
                        //                //     No or unknown encoding.
                        //                Raw = 0,
                        ////
                        //// Resumo:
                        ////     The American Standard Code for Information Interchange (ASCII) encoding scheme.
                        //Ascii = 1,
                        ////
                        //// Resumo:
                        ////     The ISO/IEC 8859-15 encoding scheme, also knows as Latin-0 and Latin-9.
                        //Latin0 = 2,
                        ////
                        //// Resumo:
                        ////     The ISO/IEC 8859-1 encoding scheme, also knows as Latin-1.
                        //Latin1 = 3,
                        ////
                        //// Resumo:
                        ////     The UTF-8 encoding scheme.
                        //Utf8 = 4,
                        ////
                        //// Resumo:
                        ////     The ISO/IEC 2022 encoding scheme.
                        //Iso2022 = 5,
                        ////
                        //// Resumo:
                        ////     The MacRoman encoding scheme.
                        //MacRoman = 6,
                        ////
                        //// Resumo:
                        ////     The Windows-1252 encoding scheme.
                        //Win1252 = 7,
                        ////
                        //// Resumo:
                        ////     The Code page 858 encoding scheme, also know as CP 858, IBM 858, or OEM 858.
                        //Ibm858 = 8,
                        ////
                        //// Resumo:
                        ////     The UTF-16LE (Little Endian) encoding scheme.
                        //Utf16LittleEndian = 9,
                        ////
                        //// Resumo:
                        ////     The UTF-16BE (Big Endian) encoding scheme.
                        //Utf16BigEndian = 10,
                        ////
                        //// Resumo:
                        ////     The UTF-16 encoding scheme, with endianess detected using a BOM.
                        //Utf16 = 11,
                        ////
                        //// Resumo:
                        ////     The Big-5 or Big5 encoding scheme, used in Taiwan, Hong Kong, and Macau for Traditional
                        ////     Chinese characters.
                        //Big5 = 12,
                        ////
                        //// Resumo:
                        ////     The Shift JIS encoding scheme for Japanese characters.
                        //ShiftJIS = 13
                        #endregion

                        if (op == "2")
                        {
                            arq = File.ReadAllText(arq, System.Text.Encoding.GetEncoding("windows-1252"));
                            op = "1";
                        }




                        string parsed = ContentUtils.HTMLFormat.CleanAndRepair(arq, op == "2", enc, null, "Latin1", false, false, true);
                        //string parsed = ContentUtils.HTMLFormat.CleanHtml(arq);

                        Console.WriteLine(parsed);

                        //var nome = Path.GetDirectoryName(arq) + "\\" + Path.GetFileNameWithoutExtension(arq) + "-corrigido" + Path.GetExtension(arq);

                        //File.WriteAllText(nome, parsed, FileUtils.GetFileEncoding(arq));


                        //Console.WriteLine("arquivo salvo:" + nome);

                        //var st = new ProcessStartInfo()
                        //{
                        //    FileName = nome
                        //};

                        //Process.Start(st);

                        //st.FileName = "notepad.exe";
                        //st.Arguments = nome;

                        //Process.Start(st);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("---------------------- ERRO AO PROCESSAR:" + ex.ToString());
                    }
                }

                Console.WriteLine("\n\n\n");


            }


        }
    }
}
