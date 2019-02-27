using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace Obtem_Ip_Externo
{
    class Email
    {

        private static string Destinatario;
        private static string SMTP;
        private static string EmailSaida;
        private static string SenhaSaida;
        private static string Porta;
        private static string Assunto;
        private static string AtivaSSL;
        public static string Erro { get; private set; }






        private static bool RetornaVariaveis()
        {


            var conn = Banco.ConectaOracle();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            if (conn.State == ConnectionState.Open)
            {
                string Consulta = "select * from v_configuracao v where v.configuracao in ('EMAIL_SAIDA','SENHA_SAIDA','SMTP_SAIDA','PORTA_SAIDA','ATIVA_SSL','EMAIL_IP_EXTERNO','ASSUNTO_IP_EXTERNO')";
                Log.GeraLog("Vai retornar os dados para enviar o e-mail");
                OracleCommand sm = new OracleCommand(Consulta, conn);
                OracleDataReader dr = sm.ExecuteReader();
                while (dr.Read())
                {
                    //Ultimo_IP = dr["ip"] + "";

                    if (dr["configuracao"].Equals("EMAIL_IP_EXTERNO".ToUpper()))
                    {
                        Destinatario = dr["valor_nao_codificado"].ToString();
                    }
                    else if (dr["configuracao"].Equals("SMTP_SAIDA".ToUpper()))
                    {
                        SMTP = dr["valor_nao_codificado"].ToString();
                    }
                    else if (dr["configuracao"].Equals("EMAIL_SAIDA".ToUpper()))
                    {
                        EmailSaida = dr["valor_nao_codificado"].ToString();
                    }
                    else if (dr["configuracao"].Equals("SENHA_SAIDA".ToUpper()))
                    {
                        SenhaSaida = dr["valor_nao_codificado"].ToString();
                    }
                    else if (dr["configuracao"].Equals("PORTA_SAIDA".ToUpper()))
                    {
                        Porta = dr["valor_nao_codificado"].ToString();
                    }
                    else if (dr["configuracao"].Equals("ATIVA_SSL".ToUpper()))
                    {
                        AtivaSSL = dr["valor_nao_codificado"].ToString();
                    }
                    else if (dr["configuracao"].Equals("ASSUNTO_IP_EXTERNO".ToUpper()))
                    {
                        Assunto = dr["valor_nao_codificado"].ToString();
                    }

                }
                dr.Close();
                conn.Close();
                return true;
            }
            else
            {
                Log.GeraLog("Não foi possivel abrir conexão com o Banco de Dados para obter os dados para enviar o e-mail");
                Erro = "Não foi possivel abrir conexão";
                return false;
            }
        }


        public static bool EnviaEmail(string IP)
        {
            //Attachment anexo = new Attachment(Arquivo_anexo);
            try
            {
                if (RetornaVariaveis())
                {
                    MailMessage objEmail = new MailMessage
                    {
                        From = new MailAddress(EmailSaida, Assunto)// Email e nome que aparecer
                    };
                    //String teste = email.Text;
                    //Console.Write(teste);
                    objEmail.To.Add(Destinatario);// email de que será enviado
                    // objEmail.Bcc.Add(DestinatariosCopia);
                    objEmail.Priority = MailPriority.Normal;
                    objEmail.IsBodyHtml = true;

                    //objEmail.Attachments.Add(anexo);
                    objEmail.Subject = Assunto; // assunto
                                                //textBox1.Text.Replace("\r\n", "<br>") // comando para pegar quebra de linha
                    objEmail.Body = "Novo IP Externo encontrado<br><br>IP: " + IP;// conteudo do e-mail
                    objEmail.SubjectEncoding = Encoding.GetEncoding("ISO-8859-1");
                    objEmail.BodyEncoding = Encoding.GetEncoding("ISO-8859-1");

                    SmtpClient objSmtp = new SmtpClient
                    {
                        Port = Convert.ToInt32(Porta), //Liberar a porta
                        EnableSsl = Convert.ToBoolean(AtivaSSL),
                        Host = SMTP, // SMTP
                                     //objSmtp.Credentials = new NetworkCredential("naorespondabra@gmail.com", "99775332"); // conta gmail que enviara o email
                        Credentials = new NetworkCredential(EmailSaida, SenhaSaida)// conta gmail que enviara o email
                    };
                    objSmtp.Send(objEmail);
                    //anexo.Dispose();
                    return true;
                }
                else
                {
                    Erro = "Dados de E-mail não foram retornados com sucesso";
                    Log.GeraLog(Erro);
                    return false;
                }
            }
            catch (Exception e)
            {

                Erro = "Erro: " + e.Message;
                Log.GeraLog(Erro);
                return false;
            }
        }
    }
}
