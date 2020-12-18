using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MigLibUtils.Services.PagSeguro
{
    class Dados
    {
        // Seus dados de acesso ao PagSeguro
        public string MeuEmail { get; set; }
        public string MeuToken { get; set; }

        // Dados de Envvio para o PagSeguro
        public string Nome { get; set; }
        public string Email { get; set; }
        public string DDD { get; set; }
        public string NumeroTelefone { get; set; }
        public string Valor { get; set; }
        public string CodigoAcesso { get; set; }
        public string Referencia { get; set; }
        public string TituloPagamento { get; set; }
        public int Qtd { get; set; } = 1;
        public int Peso { get; set; } = 0;
        public bool SolicitaEnderecoEnvio { get; set; } = false;

        public string ProductId { get; set; }


        public int maxPageResults { get; set; } = 100;

        public DateTime DataDesde { get; set; }

        // Dados de Retorno do PagSeguro
        public string Status { get; set; }
        public string stringConexao { get; set; }

        public XmlDocument xmlDoc { get; set; } = new XmlDocument();

        //Dados de erro
        public string Erro { get; set; } = "";

    }
}
