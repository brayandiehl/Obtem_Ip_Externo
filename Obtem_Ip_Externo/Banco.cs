using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace Obtem_Ip_Externo
{
    class Banco
    {


        public static string Erro { get; private set; }
        public static string LinhasAfetadas { get; private set; }

        public static OracleConnection ConectaOracle()
        {
            OracleConnection SQLConnection = new OracleConnection();
            try
            {
                SQLConnection.ConnectionString = ConfigurationManager.ConnectionStrings["Oracle"].ConnectionString;
                return SQLConnection;

            }
            catch (Exception Ex)
            {
                Erro = Ex.Message;
                return null;
            }
        }
        public static bool ExecutaQuery(string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    var conn = ConectaOracle();
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        OracleCommand comando = new OracleCommand(query, conn);
                        LinhasAfetadas = comando.ExecuteNonQuery().ToString();
                        ConectaOracle().Close();
                        return true;
                    }
                    else
                    {
                        Erro = "Falha ao abrir conexão";
                        return false;
                    }

                }
                else
                {
                    Erro = "Query não poder vazia";
                    return false;
                    //return "Query não pode ser nulla";
                }
            }
            catch (Exception EX)
            {
                Erro = "Erro: " + EX.Message;
                return false;
            }

        }

    }
}
