using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FooBlog.common;
using Npgsql;
using NpgsqlTypes;

namespace FooBlog
{
    public partial class edit_profile : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FooSessionHelper.AuthenticationCheck(HttpContext.Current);

            if (IsPostBack) return;

            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
            Load_Forms();
        }

        protected void Reset_Page()
        {
            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
            userView.ChangeMode(DetailsViewMode.ReadOnly);
            Load_Forms();
        }

        protected void Load_Forms()
        {
            string userId = FooSessionHelper.GetUserObjectFromCookie(HttpContext.Current).UserId;

            if (!FooStringHelper.IsValidAlphanumeric(userId, 16))
            {
                errorLabel.Text = "Invalid request.";
                return;
            }

            try
            {
                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT userid, useralias, email, address, city, country, profilebody, profileimg FROM users WHERE userid= @USERID",
                            conn);

                    var idParam = new NpgsqlParameter
                        {
                            ParameterName = "@USERID",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 16,
                            Direction = ParameterDirection.Input,
                            Value = FooStringHelper.RemoveInvalidChars(userId)
                        };
                    cmd.Parameters.Add(idParam);

                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        userView.DataSource = dr;
                        userView.DataBind();
                    }
                }

                errorLabel.Text = "";
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }

        protected void UserView_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            UserObject userObj = FooSessionHelper.GetUserObjectFromCookie(HttpContext.Current);
            string userId = userObj.UserId;
            string userName = userObj.Username;

            if (!FooStringHelper.IsValidAlphanumeric(userId, 16))
            {
                errorLabel.Text = "Invalid request.";
                Reset_Page();
                return;
            }

            var txtUserAlias = (TextBox) userView.FindControl("txtUserAlias");
            var txtUserEmail = (TextBox) userView.FindControl("txtUserEmail");
            var txtUserAddress = (TextBox) userView.FindControl("txtUserAddress");
            var txtUserCity = (TextBox) userView.FindControl("txtUserCity");
            var txtUserCountry = (TextBox) userView.FindControl("txtUserCountry");
            var txtUserBody = (TextBox) userView.FindControl("txtUserBody");
            var imageUploadForm = (FileUpload) userView.FindControl("imageUploadForm");

            if (!string.IsNullOrEmpty(txtUserAlias.Text) && !string.IsNullOrEmpty(txtUserEmail.Text) &&
                !string.IsNullOrEmpty(txtUserAddress.Text) && !string.IsNullOrEmpty(txtUserCity.Text) &&
                !string.IsNullOrEmpty(txtUserCountry.Text) && !string.IsNullOrEmpty(txtUserBody.Text) &&
                !string.IsNullOrEmpty(txtUserEmail.Text) && FooStringHelper.IsValidEmailAddress(txtUserEmail.Text) &&
                !FooEmailHelper.CheckIfEmailExists(txtUserEmail.Text, userName))
            {
                try
                {
                    if (FooSessionHelper.IsValidRequest(HttpContext.Current, RequestToken.Value))
                    {
                        using (var conn = new NpgsqlConnection())
                        {
                            conn.ConnectionString =
                                ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                            conn.Open();

                            var cmd = new NpgsqlCommand
                                {
                                    CommandText =
                                        "UPDATE users SET (useralias, email, address, city, country, profilebody) = (@USERALIAS, @EMAIL, @ADDRESS, @CITY, @COUNTRY, @PROFILEBODY) WHERE userid= @USERID",
                                    CommandType = CommandType.Text,
                                    Connection = conn
                                };

                            var idParam = new NpgsqlParameter
                                {
                                    ParameterName = "@USERID",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 16,
                                    Direction = ParameterDirection.Input,
                                    Value = FooStringHelper.RemoveInvalidChars(userId)
                                };
                            cmd.Parameters.Add(idParam);

                            var aliasParam = new NpgsqlParameter
                                {
                                    ParameterName = "@USERALIAS",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 32,
                                    Direction = ParameterDirection.Input,
                                    Value = txtUserAlias.Text
                                };
                            cmd.Parameters.Add(aliasParam);

                            var emailParam = new NpgsqlParameter
                                {
                                    ParameterName = "@EMAIL",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 64,
                                    Direction = ParameterDirection.Input,
                                    Value = txtUserEmail.Text
                                };
                            cmd.Parameters.Add(emailParam);

                            var addressParam = new NpgsqlParameter
                                {
                                    ParameterName = "@ADDRESS",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 128,
                                    Direction = ParameterDirection.Input,
                                    Value = txtUserAddress.Text
                                };
                            cmd.Parameters.Add(addressParam);

                            var cityParam = new NpgsqlParameter
                                {
                                    ParameterName = "@CITY",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 32,
                                    Direction = ParameterDirection.Input,
                                    Value = txtUserCity.Text
                                };
                            cmd.Parameters.Add(cityParam);

                            var countryParam = new NpgsqlParameter
                                {
                                    ParameterName = "@COUNTRY",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 32,
                                    Direction = ParameterDirection.Input,
                                    Value = txtUserCountry.Text
                                };
                            cmd.Parameters.Add(countryParam);

                            var bodyParam = new NpgsqlParameter
                                {
                                    ParameterName = "@PROFILEBODY",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 1024,
                                    Direction = ParameterDirection.Input,
                                    Value = txtUserBody.Text
                                };
                            cmd.Parameters.Add(bodyParam);

                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }

                        if (imageUploadForm.HasFile)
                        {
                            string path = HttpContext.Current.Server.MapPath("~/uploads");

                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            HttpPostedFile file = HttpContext.Current.Request.Files[0];

                            if (file.ContentLength < 2097152)
                            {
                                string fileName;

                                if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE")
                                {
                                    string[] files = file.FileName.Split(new[] {'\\'});
                                    fileName = files[files.Length - 1];
                                }
                                else
                                {
                                    fileName = file.FileName;
                                }

                                fileName = FooStringHelper.RandomFileName(fileName);
                                string filePath = Path.Combine(path, fileName);

                                try
                                {
                                    file.SaveAs(filePath);

                                    Insert_NewImage(fileName, userId);

                                    Reset_Page();
                                }
                                catch (Exception ex)
                                {
                                    FooLogging.WriteLog(ex.ToString());
                                    errorLabel.Text = "Upload failed.";
                                }
                            }

                            else
                            {
                                errorLabel.Text = "Invalid file.";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    FooLogging.WriteLog(ex.ToString());
                    errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
                }
            }

            else
            {
                errorLabel.Text = "Incomplete or invalid input.";
            }

            Reset_Page();
        }

        protected void UserView_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {
            userView.ChangeMode(e.NewMode == DetailsViewMode.Edit ? DetailsViewMode.Edit : DetailsViewMode.ReadOnly);

            Load_Forms();
        }

        protected void Insert_NewImage(string fileName, string userId)
        {
            try
            {
                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT profileimg FROM users WHERE userid= @USERID",
                            conn);

                    var idParam = new NpgsqlParameter
                        {
                            ParameterName = "@USERID",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 16,
                            Direction = ParameterDirection.Input,
                            Value = userId
                        };
                    cmd.Parameters.Add(idParam);

                    NpgsqlDataReader dr = cmd.ExecuteReader();
                    string imageFile = string.Empty;

                    while (dr.Read())
                    {
                        imageFile = dr["profileimg"].ToString();
                    }

                    dr.Close();

                    if (imageFile != string.Empty && imageFile != "profile_default.jpg")
                    {
                        string path = HttpContext.Current.Server.MapPath("~/uploads");
                        string currentFile = Path.Combine(path, imageFile);
                        File.Delete(currentFile);
                    }
                }

                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "UPDATE users SET (profileimg) = (@PROFILEIMG) WHERE userid= @USERID",
                            conn);

                    var idParam = new NpgsqlParameter
                        {
                            ParameterName = "@USERID",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 16,
                            Direction = ParameterDirection.Input,
                            Value = FooStringHelper.RemoveInvalidChars(userId)
                        };
                    cmd.Parameters.Add(idParam);

                    var imgParam = new NpgsqlParameter
                        {
                            ParameterName = "@PROFILEIMG",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 64,
                            Direction = ParameterDirection.Input,
                            Value = fileName
                        };
                    cmd.Parameters.Add(imgParam);

                    cmd.ExecuteNonQuery();
                }
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }
    }
}