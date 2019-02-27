using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace Obtem_Ip_Externo
{
    class Conexao
    {
        public static string Erro { get; private set; }

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int Description, int ReservedValue);

        public static bool TestaConexaoInternet()
        {
            try
            {
                Log.GeraLog("Vai testar conexão com a Internet");
                int Description;
                return InternetGetConnectedState(out Description, 0);
            }
            catch (Exception ex)
            {
                Erro = ex.Message;
                return false;
            }
        }

        public static bool TestaPing(string endereco)
        {
            try

            {
                Log.GeraLog("Vai testar comunicação com o servidor de banco de dados");
                return (new Ping().Send(endereco).Status == IPStatus.Success) ? true : false;
            }
            catch (Exception ex)
            {
                Erro = ex.Message;
                return false;
            }
        }
    }
}
