using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using FooBlog.common;
using Npgsql;
using NpgsqlTypes;

namespace FooBlog
{
    public partial class do_reset : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                formPanel.Visible = false;
                errorPanel.Visible = true;
                errorLabel.Text =
                    "Please log out first, or reset your password in the <a href=\"edit_profile.aspx\">profile editor</a>.";
            }

            if (Page.IsPostBack) return;

            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);

            string resetId = Request.QueryString["id"];
            string token = Request.QueryString["token"];

            if (FooStringHelper.IsValidAlphanumeric(resetId, 16) && FooStringHelper.IsValidAlphanumeric(token, 24))
            {
                string resetAccount = GetAccountForReset(resetId, token);

                if (!String.IsNullOrEmpty(resetAccount))
                {
                    formPanel.Visible = true;
                }

                else
                {
                    errorPanel.Visible = true;
                    errorLabel.Text = "Invalid request.";
                }
            }

            else
            {
                errorPanel.Visible = true;
                errorLabel.Text = "Invalid request.";
            }
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {
            string password = passText.Text.Trim();
            string confirmation = confirmText.Text.Trim();

            if (password != confirmation)
            {
                errorLabel.Text = "The password and confirmation do not match.";
                return;
            }

            string resetId = Request.QueryString["id"];
            string token = Request.QueryString["token"];

            if (!String.IsNullOrEmpty(resetId) && !String.IsNullOrEmpty(token) && !String.IsNullOrEmpty(password))
            {
                if (FooSessionHelper.IsValidRequest(HttpContext.Current, RequestToken.Value))
                {
                    string userId = GetAccountForReset(resetId, token);

                    if (!String.IsNullOrEmpty(userId))
                    {
                        bool doReset = UpdatePassword(userId, password);

                        if (doReset)
                        {
                            errorPanel.Visible = false;
                            formPanel.Visible = false;
                            successPanel.Visible = true;

                            string email = FooEmailHelper.GetEmailForAccount(userId);

                            var emailObj = new EmailObject
                                {
                                    Body =
                                        "Your FooBlog password has been reset. If you did not perform this action, please contact a FooBlog administrator using your registered email account",
                                    Subject = "FooBlog Password Reset",
                                    ToAddress = email
                                };

                            FooEmailHelper.SendEmail(emailObj);

                            successLabel.Text =
                                "Your password has been reset. You can proceed to <a href=\"login.aspx\">login</a> again.";

                            errorPanel.Visible = false;
                            errorLabel.Text = "";
                        }
                    }
                }

                else
                {
                    errorPanel.Visible = true;
                    errorLabel.Text = "Invalid request.";
                }
            }

            else
            {
                errorPanel.Visible = true;
                errorLabel.Text = "Passwords do not match.";
            }

            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
        }

        public static string GetAccountForReset(string resetId, string token)
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
                                "SELECT userid FROM resets WHERE resetid= @RESETID",
                            CommandType = CommandType.Text,
                            Connection = conn
                        };

                    var idParam = new NpgsqlParameter
                        {
                            ParameterName = "@RESETID",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 16,
                            Direction = ParameterDirection.Input,
                            Value = resetId
                        };
                    cmd.Parameters.Add(idParam);

                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    string result = String.Empty;

                    while (dr.Read())
                    {
                        result = dr["userid"].ToString();
                    }

                    dr.Close();

                    return !String.IsNullOrEmpty(result) ? FooCryptHelper.Decrypt(result, token) : null;
                }
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                return null;
            }
        }

        public static bool UpdatePassword(string id, string pass)
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
                                "UPDATE Users SET (passwordhash) = (@PASSWORDHASH) WHERE userid= @USERID;",
                            CommandType = CommandType.Text,
                            Connection = conn
                        };

                    var idParam = new NpgsqlParameter
                        {
                            ParameterName = "@USERID",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 16,
                            Direction = ParameterDirection.Input,
                            Value = id
                        };
                    cmd.Parameters.Add(idParam);

                    var hashParam = new NpgsqlParameter
                        {
                            ParameterName = "@PASSWORDHASH",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Direction = ParameterDirection.Input,
                            Value = FooCryptHelper.CreateShaHash(pass)
                        };
                    cmd.Parameters.Add(hashParam);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                return true;
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                return false;
            }
        }
    }
}