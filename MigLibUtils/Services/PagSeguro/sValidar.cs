using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace MigLibUtils.Services.PagSeguro
{
    static class sValidar
    {
        // Validar conexão com a internet
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int description, int reservedValue);
        public static bool IsConnected()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }
        // Validar e-mail
        public static bool ValidarEmail(string email)
        {
            return Regex.IsMatch(email, ("(?<user>[^@]+)@(?<host>.+)"));
        }
        // Valida texto como decimal
        public static bool ValidarDecimal(string valor = "")
        {
            bool retorno = true;
            try
            {
                var tmp = Convert.ToDecimal(valor);
            }
            catch
            {
                retorno = false;
            }
            return retorno;
        }
       
        // Retornar valor em formato monetáio
        public static string ValidarMoeda(string valor = "", bool casadecimal = true)
        {
            try
            {
                // Verifica se o valor passado é um valor numérico
                if (!ValidarDecimal(valor))
                    return casadecimal ? "0,00" : "0";
                // Se tiver mais de um sinal negativo, remove do valor
                if (valor.Contains("-") && valor.IndexOf("-") > 0)
                    valor = valor.Replace("-", "");
                // Se não for um valor válido, retorna 0
                if (valor.Equals("") || valor.Equals("-,") || valor.Equals(",-")) valor = casadecimal ? "0,00" : "0";
                if (!valor.Equals("") && Convert.ToDecimal(valor).Equals(0)) return casadecimal ? "0,00" : "0";
                // Retornar o valor validado
                return valor.Equals("")
                    ? casadecimal ? "0,00" : "0"
                    : Math.Round(Convert.ToDecimal(valor), 2).ToString("N" + (casadecimal ? "2" : "0"));
            }
            catch
            {
                return "0,00";
            }
        }
    }
}
