using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using Npgsql;
using NpgsqlTypes;

namespace FooBlog
{
    public partial class register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {
            string alias = aliasText.Text;
            string email = emailText.Text;
            string address = addressText.Text;
            string city = cityText.Text;
            string country = countryText.Text;
            string username = usernameText.Text;
            string pass = passText.Text;

            if (!String.IsNullOrEmpty(alias) && FooStringHelper.IsValidEmailAddress(email) &&
                !String.IsNullOrEmpty(address) &&
                !String.IsNullOrEmpty(city) && !String.IsNullOrEmpty(country) && !String.IsNullOrEmpty(username) &&
                !String.IsNullOrEmpty(pass))
            {
                if (FooSessionHelper.IsValidRequest(HttpContext.Current, RequestToken.Value))
                {
                    string userId = FooStringHelper.RandomString(16);

                    if (!CheckIfUsernameExists(username) && !FooEmailHelper.CheckIfEmailExists(email, username))
                    {
                        errorPanel.Visible = false;
                        formPanel.Visible = false;
                        successPanel.Visible = true;

                        string defaultGroup = ConfigurationManager.AppSettings["User Group ID"] ?? "ri3EKpc5Z5gN4FEu";

                        bool insertedUser = RegisterNewUser(userId, alias, email, address, city, country, username, pass,
                                                            defaultGroup);

                        successLabel.Text = insertedUser
                                                ? "Your account has been successfully created. You can proceed to <a href=\"login.aspx\">log on</a>."
                                                : "Failed to create account. The administrator has been notified. Please try again.";

                        errorPanel.Visible = false;
                        errorLabel.Text = "";
                    }

                    else
                    {
                        errorPanel.Visible = true;
                        errorLabel.Text = "Some details already exist in this application.";
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
                errorLabel.Text = "Incomplete or invalid details.";
            }

            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
        }

        public static bool CheckIfUsernameExists(string username)
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
                                "SELECT username FROM users WHERE username= @USERNAME",
                            CommandType = CommandType.Text,
                            Connection = conn
                        };

                    var nameParam = new NpgsqlParameter
                        {
                            ParameterName = "@USERNAME",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 32,
                            Direction = ParameterDirection.Input,
                            Value = username
                        };
                    cmd.Parameters.Add(nameParam);

                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    string result = String.Empty;

                    while (dr.Read())
                    {
                        result = dr["username"].ToString();
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

        public static bool RegisterNewUser(string id, string alias, string email, string address, string city,
                                           string country,
                                           string username, string pass, string groupId)
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
                                "INSERT INTO Users (userId, userName, userAlias, passwordHash, groupId, email, address, city, country, profileimg) VALUES (@USERID, @USERNAME, @USERALIAS, @PASSWORDHASH, @GROUPID, @EMAIL, @ADDRESS, @CITY, @COUNTRY, 'profile_default.jpg');",
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

                    var nameParam = new NpgsqlParameter
                        {
                            ParameterName = "@USERNAME",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 32,
                            Direction = ParameterDirection.Input,
                            Value = username
                        };
                    cmd.Parameters.Add(nameParam);

                    var aliasParam = new NpgsqlParameter
                        {
                            ParameterName = "@USERALIAS",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 32,
                            Direction = ParameterDirection.Input,
                            Value = alias
                        };
                    cmd.Parameters.Add(aliasParam);

                    var hashParam = new NpgsqlParameter
                        {
                            ParameterName = "@PASSWORDHASH",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Direction = ParameterDirection.Input,
                            Value = FooCryptHelper.CreateShaHash(pass)
                        };
                    cmd.Parameters.Add(hashParam);

                    var groupParam = new NpgsqlParameter
                        {
                            ParameterName = "@GROUPID",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Direction = ParameterDirection.Input,
                            Value = groupId
                        };
                    cmd.Parameters.Add(groupParam);

                    var emailParam = new NpgsqlParameter
                        {
                            ParameterName = "@EMAIL",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 64,
                            Direction = ParameterDirection.Input,
                            Value = email
                        };
                    cmd.Parameters.Add(emailParam);

                    var addressParam = new NpgsqlParameter
                        {
                            ParameterName = "@ADDRESS",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 128,
                            Direction = ParameterDirection.Input,
                            Value = address
                        };
                    cmd.Parameters.Add(addressParam);

                    var cityParam = new NpgsqlParameter
                        {
                            ParameterName = "@CITY",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 32,
                            Direction = ParameterDirection.Input,
                            Value = city
                        };
                    cmd.Parameters.Add(cityParam);

                    var countryParam = new NpgsqlParameter
                        {
                            ParameterName = "@COUNTRY",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 32,
                            Direction = ParameterDirection.Input,
                            Value = country
                        };
                    cmd.Parameters.Add(countryParam);

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