using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;
using NpgsqlTypes;

namespace FooBlog
{
    public partial class admin_merchandise : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FooSessionHelper.AuthenticationCheck(HttpContext.Current);

            if (IsPostBack) return;

            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
            Load_Forms(string.Empty);
        }

        protected void Reset_Page(string merchId)
        {
            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
            merchView.ChangeMode(DetailsViewMode.ReadOnly);
            Load_Forms(merchId);
        }

        protected void Load_Forms(string merchId)
        {
            if (merchId != string.Empty && !FooStringHelper.IsValidAlphanumeric(merchId, 16))
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

                    var cmd = new NpgsqlCommand();

                    if (String.IsNullOrEmpty(merchId))
                    {
                        cmd.CommandText =
                            "SELECT merchid, merchname, merchbrief, merchbody, merchprice, merchimg, merchenabled FROM merchandise ORDER BY merchid DESC LIMIT 1";
                        cmd.Connection = conn;
                    }

                    else
                    {
                        cmd.CommandText =
                            "SELECT merchid, merchname, merchbrief, merchbody, merchprice, merchimg, merchenabled FROM merchandise WHERE merchid= @MERCHID";
                        cmd.Connection = conn;

                        var idParam = new NpgsqlParameter
                            {
                                ParameterName = "@MERCHID",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 16,
                                Direction = ParameterDirection.Input,
                                Value = merchId
                            };
                        cmd.Parameters.Add(idParam);
                    }

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    merchView.DataSource = ds;
                    merchView.DataBind();
                }

                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT merchid, merchname FROM merchandise",
                            conn);

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    merchGrid.DataSource = ds;
                    merchGrid.DataBind();
                }

                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT T1.reviewid, T1.reviewtime, T1.userid, T1.merchid, T1.reviewbody, T2.userid, T2.useralias, T3.merchid, T3.merchname FROM reviews AS T1 LEFT OUTER JOIN users AS T2 ON T1.userid = T2.userid LEFT OUTER JOIN merchandise AS T3 ON T1.merchid = T3.merchid",
                            conn);

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    reviewGrid.DataSource = ds;
                    reviewGrid.DataBind();
                }


                errorLabel.Text = "";
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }

        protected void MerchView_ItemDeleting(object sender, DetailsViewDeleteEventArgs e)
        {
            string merchId = merchView.SelectedValue.ToString();

            if (!FooStringHelper.IsValidAlphanumeric(merchId, 16))
            {
                errorLabel.Text = "Invalid request.";
                Reset_Page(string.Empty);
                return;
            }

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
                                CommandText = "DELETE FROM reviews WHERE merchid= @MERCHID",
                                CommandType = CommandType.Text,
                                Connection = conn
                            };

                        var param = new NpgsqlParameter
                            {
                                ParameterName = "@MERCHID",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 16,
                                Direction = ParameterDirection.Input,
                                Value = merchId
                            };
                        cmd.Parameters.Add(param);

                        cmd.ExecuteNonQuery();
                    }

                    using (var conn = new NpgsqlConnection())
                    {
                        conn.ConnectionString =
                            ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                        conn.Open();

                        var cmd = new NpgsqlCommand
                            {
                                CommandText = "DELETE FROM merchandise WHERE merchid= @MERCHID",
                                CommandType = CommandType.Text,
                                Connection = conn
                            };

                        var param = new NpgsqlParameter
                            {
                                ParameterName = "@MERCHID",
                                Size = 16,
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Direction = ParameterDirection.Input,
                                Value = merchId
                            };
                        cmd.Parameters.Add(param);

                        cmd.ExecuteNonQuery();
                    }
                }

                else
                {
                    errorLabel.Text = "Invalid request.";
                }
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }

            Reset_Page(string.Empty);
        }

        protected void MerchView_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            string merchId = merchView.SelectedValue.ToString();

            if (!FooStringHelper.IsValidAlphanumeric(merchId, 16))
            {
                errorLabel.Text = "Invalid request.";
                Reset_Page(string.Empty);
                return;
            }

            var txtMerchName = (TextBox) merchView.FindControl("txtMerchName");
            var txtMerchPrice = (TextBox) merchView.FindControl("txtMerchPrice");
            var txtMerchBrief = (TextBox) merchView.FindControl("txtMerchBrief");
            var txtMerchBody = (TextBox) merchView.FindControl("txtMerchBody");
            var imageUploadForm = (FileUpload) merchView.FindControl("imageUploadForm");
            var merchEnabledCheckbox = (CheckBox) merchView.FindControl("merchEnabledCheckbox");

            if (!string.IsNullOrEmpty(txtMerchName.Text) && !string.IsNullOrEmpty(txtMerchPrice.Text) &&
                FooStringHelper.IsValidPrice(txtMerchPrice.Text) && !string.IsNullOrEmpty(txtMerchBrief.Text) &&
                !string.IsNullOrEmpty(txtMerchBody.Text))
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
                                        "UPDATE merchandise SET (merchname, merchprice, merchbrief, merchbody, merchenabled) = (@MERCHNAME, @MERCHPRICE, @MERCHBRIEF, @MERCHBODY, @MERCHENABLED) WHERE merchid= @MERCHID",
                                    CommandType = CommandType.Text,
                                    Connection = conn
                                };

                            var idParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHID",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 16,
                                    Direction = ParameterDirection.Input,
                                    Value = merchId
                                };
                            cmd.Parameters.Add(idParam);

                            var nameParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHNAME",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 64,
                                    Direction = ParameterDirection.Input,
                                    Value = txtMerchName.Text
                                };
                            cmd.Parameters.Add(nameParam);

                            var priceParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHPRICE",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 8,
                                    Direction = ParameterDirection.Input,
                                    Value = txtMerchPrice.Text
                                };
                            cmd.Parameters.Add(priceParam);

                            var briefParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHBRIEF",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 1024,
                                    Direction = ParameterDirection.Input,
                                    Value = txtMerchBrief.Text
                                };
                            cmd.Parameters.Add(briefParam);

                            var bodyParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHBODY",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Direction = ParameterDirection.Input,
                                    Value = txtMerchBody.Text
                                };
                            cmd.Parameters.Add(bodyParam);

                            var enabledParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHENABLED",
                                    NpgsqlDbType = NpgsqlDbType.Boolean,
                                    Direction = ParameterDirection.Input,
                                    Value = merchEnabledCheckbox.Checked
                                };
                            cmd.Parameters.Add(enabledParam);

                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }

                        if (imageUploadForm.HasFile)
                        {
                            HttpPostedFile file = HttpContext.Current.Request.Files[0];
                            Insert_NewImage(merchId, file);
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

            Reset_Page(merchId);
        }

        protected void MerchView_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            string merchId = FooStringHelper.RandomString(16);
            var txtMerchName = (TextBox) merchView.FindControl("txtMerchName");
            var txtMerchPrice = (TextBox) merchView.FindControl("txtMerchPrice");
            var txtMerchBrief = (TextBox) merchView.FindControl("txtMerchBrief");
            var txtMerchBody = (TextBox) merchView.FindControl("txtMerchBody");
            var imageUploadForm = (FileUpload) merchView.FindControl("imageUploadForm");
            var merchEnabledCheckbox = (CheckBox) merchView.FindControl("merchEnabledCheckbox");

            if (!string.IsNullOrEmpty(txtMerchName.Text) && !string.IsNullOrEmpty(txtMerchPrice.Text) &&
                FooStringHelper.IsValidPrice(txtMerchPrice.Text) && !string.IsNullOrEmpty(txtMerchBrief.Text) &&
                !string.IsNullOrEmpty(txtMerchBody.Text))
            {
                try
                {
                    if (FooSessionHelper.IsValidRequest(HttpContext.Current, RequestToken.Value))
                    {
                        // Define connection string.
                        using (var conn = new NpgsqlConnection())
                        {
                            conn.ConnectionString =
                                ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                            conn.Open();

                            var cmd = new NpgsqlCommand
                                {
                                    CommandText =
                                        "INSERT INTO merchandise (merchid, merchname, merchprice, merchbrief, merchbody, merchenabled) VALUES (@MERCHID, @MERCHNAME, @MERCHPRICE, @MERCHBRIEF, @MERCHBODY, @MERCHENABLED)",
                                    CommandType = CommandType.Text,
                                    Connection = conn
                                };

                            var idParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHID",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 16,
                                    Direction = ParameterDirection.Input,
                                    Value = merchId
                                };
                            cmd.Parameters.Add(idParam);

                            var nameParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHNAME",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 64,
                                    Direction = ParameterDirection.Input,
                                    Value = txtMerchName.Text
                                };
                            cmd.Parameters.Add(nameParam);

                            var priceParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHPRICE",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 8,
                                    Direction = ParameterDirection.Input,
                                    Value = txtMerchPrice.Text
                                };
                            cmd.Parameters.Add(priceParam);

                            var briefParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHBRIEF",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 1024,
                                    Direction = ParameterDirection.Input,
                                    Value = txtMerchBrief.Text
                                };
                            cmd.Parameters.Add(briefParam);

                            var bodyParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHBODY",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Direction = ParameterDirection.Input,
                                    Value = txtMerchBody.Text
                                };
                            cmd.Parameters.Add(bodyParam);

                            var enabledParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHENABLED",
                                    NpgsqlDbType = NpgsqlDbType.Boolean,
                                    Direction = ParameterDirection.Input,
                                    Value = merchEnabledCheckbox.Checked
                                };
                            cmd.Parameters.Add(enabledParam);

                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }

                        if (imageUploadForm.HasFile)
                        {
                            HttpPostedFile file = HttpContext.Current.Request.Files[0];
                            Insert_NewImage(merchId, file);
                        }

                        else
                        {
                            Insert_NewImage(merchId, null);
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

            Reset_Page(string.Empty);
        }

        protected void MerchView_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {
            switch (e.NewMode)
            {
                case DetailsViewMode.Edit:
                    merchView.ChangeMode(DetailsViewMode.Edit);
                    break;
                case DetailsViewMode.Insert:
                    merchView.ChangeMode(DetailsViewMode.Insert);
                    break;
                default:
                    merchView.ChangeMode(DetailsViewMode.ReadOnly);
                    break;
            }

            Load_Forms(merchView.SelectedValue != null ? merchView.SelectedValue.ToString() : string.Empty);
        }

        protected void MerchView_Databound(object sender, EventArgs e)
        {
            if (merchView.CurrentMode == DetailsViewMode.ReadOnly && merchView.Rows.Count > 1)
            {
                var merchEnabledLabel = (Label) merchView.FindControl("merchEnabledLabel");

                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT merchenabled FROM merchandise WHERE merchid= @MERCHID",
                            conn);

                    var idParam = new NpgsqlParameter
                        {
                            ParameterName = "@MERCHID",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 16,
                            Direction = ParameterDirection.Input,
                            Value = FooStringHelper.RemoveInvalidChars(merchView.SelectedValue.ToString())
                        };
                    cmd.Parameters.Add(idParam);

                    bool postEnabled = Convert.ToBoolean(cmd.ExecuteScalar());

                    merchEnabledLabel.Text = postEnabled ? "Yes" : "No";
                }
            }

            else
            {
                var merchEnabledCheckbox = (CheckBox) merchView.FindControl("merchEnabledCheckbox");

                try
                {
                    if (merchView.CurrentMode == DetailsViewMode.Edit)
                    {
                        using (var conn = new NpgsqlConnection())
                        {
                            conn.ConnectionString =
                                ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                            conn.Open();

                            var cmd =
                                new NpgsqlCommand(
                                    "SELECT merchenabled FROM merchandise WHERE merchid= @MERCHID",
                                    conn);

                            var idParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHID",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 16,
                                    Direction = ParameterDirection.Input,
                                    Value = FooStringHelper.RemoveInvalidChars(merchView.SelectedValue.ToString())
                                };
                            cmd.Parameters.Add(idParam);

                            NpgsqlDataReader dr = cmd.ExecuteReader();

                            while (dr.Read())
                            {
                                merchEnabledCheckbox.Checked = Convert.ToBoolean(dr["merchenabled"]);
                            }

                            dr.Close();
                        }
                    }
                }

                catch (Exception ex)
                {
                    FooLogging.WriteLog(ex.ToString());

                    string merchId = merchView.SelectedValue.ToString();

                    if (!FooStringHelper.IsValidAlphanumeric(merchId, 16))
                    {
                        errorLabel.Text = "Invalid request.";
                        Reset_Page(string.Empty);
                        return;
                    }

                    Reset_Page(merchId);
                    errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
                }
            }
        }

        protected void MerchGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string merchId = merchGrid.Rows[merchGrid.SelectedIndex].Cells[0].Text;

                if (!FooStringHelper.IsValidAlphanumeric(merchId, 16))
                {
                    errorLabel.Text = "Invalid request.";
                    Reset_Page(string.Empty);
                    return;
                }

                Load_Forms(merchId);
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }

        protected void Insert_NewImage(string merchId, HttpPostedFile file)
        {
            string fileName = "profile_default.jpg";
            string path = HttpContext.Current.Server.MapPath("~/uploads");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (file != null)
            {
                var uploadCompleted = false;

                byte[] fileBytes = FooFileHelper.GetFileBytesFromHttpStream(file);

                if (FooFileHelper.IsImage(fileBytes) && fileBytes.Length < 2097152)
                {
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
                        File.WriteAllBytes(filePath, fileBytes);
                        uploadCompleted = true;
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

                if (uploadCompleted)
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
                                    "SELECT merchimg FROM merchandise WHERE merchid= @MERCHID",
                                    conn);

                            var idParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHID",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 16,
                                    Direction = ParameterDirection.Input,
                                    Value = merchId
                                };
                            cmd.Parameters.Add(idParam);

                            NpgsqlDataReader dr = cmd.ExecuteReader();
                            string imageFile = string.Empty;

                            while (dr.Read())
                            {
                                imageFile = dr["merchimg"].ToString();
                            }

                            dr.Close();

                            if (imageFile != string.Empty && imageFile != "merch_default.jpg")
                            {
                                string currentFile = Path.Combine(path, imageFile);

                                if (File.Exists(currentFile))
                                {
                                    File.Delete(currentFile);
                                }
                            }
                        }

                        using (var conn = new NpgsqlConnection())
                        {
                            conn.ConnectionString =
                                ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                            conn.Open();

                            var cmd =
                                new NpgsqlCommand(
                                    "UPDATE merchandise SET (merchimg) = (@MERCHIMG) WHERE merchid= @MERCHID",
                                    conn);

                            var idParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHID",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 16,
                                    Direction = ParameterDirection.Input,
                                    Value = merchId
                                };
                            cmd.Parameters.Add(idParam);

                            var imgParam = new NpgsqlParameter
                                {
                                    ParameterName = "@MERCHIMG",
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
                        errorLabel.Text =
                            "Something has gone wrong. A log has been forwarded to the site administrator.";
                    }
                }
            }
        }

        protected void ReviewGrid_Delete(object sender, GridViewDeleteEventArgs e)
        {
            string merchId = FooStringHelper.RemoveInvalidChars(merchView.SelectedValue.ToString());

            if (!FooStringHelper.IsValidAlphanumeric(merchId, 16))
            {
                errorLabel.Text = "Invalid request.";
                Reset_Page(string.Empty);
                return;
            }

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
                                CommandText = "DELETE FROM reviews WHERE reviewid= @REVIEWID",
                                CommandType = CommandType.Text,
                                Connection = conn
                            };

                        var param = new NpgsqlParameter
                            {
                                ParameterName = "@REVIEWID",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 16,
                                Direction = ParameterDirection.Input,
                                Value =
                                    FooStringHelper.RemoveInvalidChars(
                                        reviewGrid.DataKeys[e.RowIndex].Values[0].ToString())
                            };
                        cmd.Parameters.Add(param);

                        cmd.ExecuteNonQuery();
                    }
                }

                else
                {
                    errorLabel.Text = "Invalid request.";
                }
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }

            Reset_Page(merchId);
        }
    }
}