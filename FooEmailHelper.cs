using System;
using System.Configuration;
using System.Data;
using System.Net.Mail;
using FooBlog.common;
using Npgsql;
using NpgsqlTypes;

namespace FooBlog
{
    public class FooEmailHelper
    {
        public static bool SendEmail(EmailObject mailObj)
        {
            try
            {
                string senderEmail = ConfigurationManager.AppSettings["SMTP FromAddress"];
                string smtpServer = ConfigurationManager.AppSettings["SMTP Server"];
                string smtpPort = ConfigurationManager.AppSettings["SMTP Port"];

                var mail = new MailMessage(senderEmail, mailObj.ToAddress);
                var client = new SmtpClient
                    {
                        Port = Convert.ToInt32(smtpPort),
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Host = smtpServer
                    };
                mail.Subject = mailObj.Subject;
                mail.Body = mailObj.Body;
                mail.IsBodyHtml = true;
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                return false;
            }
        }

        public static string GetEmailForAccount(string userId)
        {
            try
            {
                using (var conn = new NpgsqlConnection())
                {
                    // App-DB connection.
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();
                    var cmd = new NpgsqlCommand
                        {
                            CommandText =
                                "SELECT email FROM users WHERE userid= @USERID",
                            CommandType = CommandType.Text,
                            Connection = conn
                        };

                    var idParam = new NpgsqlParameter
                        {
                            ParameterName = "@USERID",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Direction = ParameterDirection.Input,
                            Value = userId
                        };
                    cmd.Parameters.Add(idParam);

                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    string result = String.Empty;

                    while (dr.Read())
                    {
                        result = dr["email"].ToString();
                    }

                    dr.Close();

                    return !String.IsNullOrEmpty(result) ? result : null;
                }
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                return null;
            }
        }

        public static bool CheckIfEmailExists(string email, string username)
        {
            try
            {
                using (var conn = new NpgsqlConnection())
                {
                    // App-DB connection.
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd = new NpgsqlCommand {CommandType = CommandType.Text, Connection = conn};

                    if (string.IsNullOrEmpty(username))
                    {
                        cmd.CommandText = "SELECT email FROM users WHERE email= @EMAIL";

                        var emailParam = new NpgsqlParameter
                            {
                                ParameterName = "@EMAIL",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 64,
                                Direction = ParameterDirection.Input,
                                Value = email
                            };
                        cmd.Parameters.Add(emailParam);
                    }

                    else
                    {
                        cmd.CommandText = "SELECT email FROM users WHERE email= @EMAIL AND username!= @USERNAME";

                        var emailParam = new NpgsqlParameter
                            {
                                ParameterName = "@EMAIL",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 64,
                                Direction = ParameterDirection.Input,
                                Value = email
                            };
                        cmd.Parameters.Add(emailParam);

                        var nameParam = new NpgsqlParameter
                            {
                                ParameterName = "@USERNAME",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 64,
                                Direction = ParameterDirection.Input,
                                Value = username
                            };
                        cmd.Parameters.Add(nameParam);
                    }

                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    string result = String.Empty;

                    while (dr.Read())
                    {
                        result = dr["email"].ToString();
                    }

                    dr.Close();

                    return !String.IsNullOrEmpty(result);
                }
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                return false;
            }
        }
    }
}