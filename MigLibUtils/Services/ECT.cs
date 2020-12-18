using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIBase.Answer;

namespace MigLibUtils.Services
{
    public class ECT
    {
        public class Address
        {
            public string CEP { get; set; }
            public string Endereco { get; set; }
            public string Complemento2 { get; set; }
            public string Bairro { get; set; }
            public string Cidade { get; set; }
            public int? IdCidade { get; set; }
            public string UF { get; set; }
            public int IdPais { get; set; } = 30; //tabela MigCountry
            public string Pais { get; set; } = "Brasil";
            public string IBGECidade { get; set; }
        }


        public static APIAnswer ConsultaCEP(string cep, out Address addr)
        {
            //TODO: colocar tratamento de serviço com PARAMETRIZAÇÃO DE URL, ERRO, TIMEOUT
            //TODO: trazer para cá todos os serviços de consulta

            addr = null;
            try
            {
                var oEnder = new ECTService.AtendeClienteClient();
                var ret = oEnder.consultaCEP(cep);

                if (ret == null)
                {
                    return APIAnswer.Error(-1, "Nao encontrado.");
                }

                addr = new Address()
                {
                    CEP = ret.cep,
                    Bairro = ret.bairro,
                    Cidade = ret.cidade,
                    Complemento2 = ret.complemento2,
                    Endereco = ret.end,
                    UF = ret.uf
                };
                return APIAnswer.Returns(addr);

            }
            catch (Exception ex)
            {
                if (ex.ToString().ToUpper().Contains("CEP INV"))
                    return APIAnswer.Error(-1, ex.ToString(), "CEP INVALIDO");
                return APIAnswer.Error(-10, ex.ToString());
            }



        }

    }
}
