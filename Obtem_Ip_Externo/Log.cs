using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Obtem_Ip_Externo
{
    class Log
    {
        public static void GeraLog_(string log)
        {
            if (ConfigurationManager.AppSettings["Gerar_Log"].Trim().Equals("1"))
            {
                string nomeArquivo = ConfigurationManager.AppSettings["Caminho_Log"] + DateTime.Now.ToString("ddMMyyyy") + ".txt";

                StreamWriter writer = new StreamWriter(nomeArquivo);
                writer.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss - ") + log + "\n");
                writer.Close();
            }

           
        }

        public static void GeraLog(string mensagem)
        {
            //string arquivo = caminho + "\\" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year.ToString() + ".txt";
            string arquivo = ConfigurationManager.AppSettings["Caminho_Log"] + DateTime.Now.ToString("ddMMyyyy") + ".txt";
            StreamWriter w = File.AppendText(arquivo);
            w.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - " + mensagem);
            w.Close();
        }
    }
}
