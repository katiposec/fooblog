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
    public partial class reset_pass : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {
            string email = emailText.Text.Trim();

            if (!String.IsNullOrEmpty(email) || !FooStringHelper.IsValidEmailAddress(email))
            {
                if (FooSessionHelper.IsValidRequest(HttpContext.Current, RequestToken.Value))
                {
                    if (FooEmailHelper.CheckIfEmailExists(email, null))
                    {
                        UserObject user = GetUserObjByEmail(email);

                        if (user != null)
                        {
                            string resetToken = FooStringHelper.RandomString(24);
                            string resetId = MakeResetRequest(user.UserId, resetToken);
                            string resetUrl = FooStringHelper.MakeResetUrl(resetId, resetToken);
                            string emailBody =
                                String.Format(
                                    "Hi {0},<br/><br/>Your FooBlog password for account '{1}' can be reset by visiting the following link:<br/><br/><a href=\"{2}\">{3}</a><br/><br/>The link is valid for 24 hours. If you did not request this reset, simply do not visit the link - your current password will remain unchanged.<br/><br/>Cheers,<br/>The FooBlog Team.",
                                    user.UserAlias, user.Username, resetUrl, resetUrl);
                            const string emailSubject = "FooBlog Password Reset";

                            var mailObj = new EmailObject {Body = emailBody, Subject = emailSubject, ToAddress = email};

                            bool sendMail = FooEmailHelper.SendEmail(mailObj);

                            if (sendMail)
                            {
                                errorPanel.Visible = false;
                                formPanel.Visible = false;
                                successPanel.Visible = true;
                                successLabel.Text = "A reset link has been sent to your registered email account.";
                            }
                        }

                        else
                        {
                            errorPanel.Visible = true;
                            errorLabel.Text = "Invalid details.";
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
                    errorLabel.Text = "Invalid details.";
                }
            }

            else
            {
                errorPanel.Visible = true;
                errorLabel.Text = "Incomplete or invalid details.";
            }

            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
        }

        public static string MakeResetRequest(string userId, string token)
        {
            try
            {
                string resetId = FooStringHelper.RandomString(16);

                using (var conn = new NpgsqlConnection())
                {
                    // App-DB connection.
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();
                    var cmd = new NpgsqlCommand
                        {
                            CommandText =
                                "INSERT INTO Resets (resetId, userId, resetTime) VALUES (@RESETID, @USERID, @RESETTIME);",
                            CommandType = CommandType.Text,
                            Connection = conn
                        };

                    var resetParam = new NpgsqlParameter
                        {
                            ParameterName = "@RESETID",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Direction = ParameterDirection.Input,
                            Value = resetId
                        };
                    cmd.Parameters.Add(resetParam);

                    var idParam = new NpgsqlParameter
                        {
                            ParameterName = "@USERID",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Direction = ParameterDirection.Input,
                            Value = FooCryptHelper.Encrypt(userId, token)
                        };
                    cmd.Parameters.Add(idParam);

                    var timeParam = new NpgsqlParameter
                        {
                            ParameterName = "@RESETTIME",
                            NpgsqlDbType = NpgsqlDbType.Timestamp,
                            Direction = ParameterDirection.Input,
                            Value = DateTime.Now
                        };
                    cmd.Parameters.Add(timeParam);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    return resetId;
                }
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                return null;
            }
        }

        public static UserObject GetUserObjByEmail(string email)
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
                                "SELECT useralias, userid, username, groupid FROM users WHERE email= @EMAIL",
                            CommandType = CommandType.Text,
                            Connection = conn
                        };

                    var emailParam = new NpgsqlParameter
                        {
                            ParameterName = "@EMAIL",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Direction = ParameterDirection.Input,
                            Value = email
                        };
                    cmd.Parameters.Add(emailParam);

                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    string userAlias = String.Empty;
                    string userName = String.Empty;
                    string userId = String.Empty;
                    string groupId = String.Empty;

                    while (dr.Read())
                    {
                        userAlias = dr["useralias"].ToString();
                        userId = dr["userid"].ToString();
                        groupId = dr["groupid"].ToString();
                        userName = dr["userName"].ToString();
                    }

                    dr.Close();

                    if (!String.IsNullOrEmpty(userAlias) && !String.IsNullOrEmpty(userId) &&
                        !String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(groupId))
                    {
                        var userObj = new UserObject
                            {
                                Username = userName,
                                UserAlias = userAlias,
                                UserId = userId,
                                GroupId = groupId
                            };

                        return userObj;
                    }

                    return null;
                }
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                return null;
            }
        }
    }
}