using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Configuration;
namespace Obtem_Ip_Externo
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {

                Log.GeraLog("\n\n\n\nInicicou Programa");
                if (Conexao.TestaConexaoInternet())
                {
                    Log.GeraLog("Conectado a internet");
                    if (Conexao.TestaPing(ConfigurationManager.AppSettings["Banco"]))
                    {
                        Log.GeraLog("Conectou com o servidor");
                        string externalip = new WebClient().DownloadString(ConfigurationManager.AppSettings["link_ip_externo"]);
                        Log.GeraLog("Pegou IP Externo: " + externalip);
                        Log.GeraLog("Vai verificar o ultimo IP encontrado");
                        var conn = Banco.ConectaOracle();
                        string Ultimo_IP = null;
                        if (conn.State == ConnectionState.Closed)
                        {
                            conn.Open();
                        }

                        if (conn.State == ConnectionState.Open)
                        {
                            string ConsultaUltimoIp = "select l.id_ip, l.ip, l.flg_email_enviado from lista_ip l WHERE ROWNUM = 1 ORDER BY L.ID_IP DESC";

                            OracleCommand sm = new OracleCommand(ConsultaUltimoIp, conn);
                            OracleDataReader dr = sm.ExecuteReader();
                            while (dr.Read())
                            {
                                Ultimo_IP = dr["ip"] + "";

                            }
                            dr.Close();
                            conn.Close();
                        }
                        else
                        {
                            Log.GeraLog("Falha ao abrir conexão com o banco de dados para obeter o ultimo IP");
                        }
                        Log.GeraLog("Ultimo IP Encontrado: " + Ultimo_IP);
                        if (!string.IsNullOrEmpty(externalip))
                        {
                            if (!externalip.Trim().Equals(Ultimo_IP))
                            {
                                Log.GeraLog("Vai salvar no Banco de Dados o IP encontrado");
                                if (conn.State == ConnectionState.Closed)
                                {
                                    conn.Open();
                                }
                                if (conn.State == ConnectionState.Open)
                                {
                                    OracleCommand oc = new OracleCommand("prc_insere_ip", conn);
                                    oc.CommandType = CommandType.StoredProcedure;
                                    oc.Parameters.Add("v_ip", OracleDbType.Varchar2).Value = externalip;
                                    OracleDataAdapter da = new OracleDataAdapter(oc);
                                    oc.ExecuteNonQuery();
                                    oc.Dispose();
                                    Log.GeraLog("IP gravado com sucesso no banco de dados");
                                    Log.GeraLog("Vai enviar e-mail");
                                    if (Email.EnviaEmail(externalip))
                                    {
                                        Log.GeraLog("E-mail enviado com sucesso");
                                        var update = "update lista_ip set flg_email_enviado = '1' where ip = '" + externalip + "' and flg_email_enviado = '0'";
                                        Banco.ExecutaQuery(update);
                                    }
                                    else
                                    {
                                        Log.GeraLog("Falha ao enviar e-mail:" + Email.Erro);
                                    }
                                }
                                else
                                {
                                    Log.GeraLog("Não foi possivel abrir conexão com o banco de dados para gravar o IP");
                                }
                            }
                            else
                            {
                                Log.GeraLog("Ip encontrado igual ultimo ip do banco de dados");
                            }
                        }
                        else
                        {
                            Log.GeraLog("Ip Externo não encontrado");
                        }
                        //Console.WriteLine(externalip);
                    }
                    else
                    {
                        Log.GeraLog("Servidor de banco de dados não está respondendo na rede");
                    }
                }
                else
                {
                    Log.GeraLog("Não foi possivel conectar na internet");
                }

            }
            catch (Exception ex)
            {
                Log.GeraLog("Erro: " + ex.Message);
            }
        }
    }
}
