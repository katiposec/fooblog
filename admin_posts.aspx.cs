using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;
using NpgsqlTypes;

namespace FooBlog
{
    public partial class admin_posts : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FooSessionHelper.AuthenticationCheck(HttpContext.Current);

            if (IsPostBack) return;

            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
            Load_Forms(-1);
        }

        protected void Reset_Page(int postId)
        {
            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
            categoryGrid.EditIndex = -1;
            postView.ChangeMode(DetailsViewMode.ReadOnly);
            Load_Forms(postId);
        }

        protected void Load_Forms(int postId)
        {
            try
            {
                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd = new NpgsqlCommand();

                    if (postId == -1)
                    {
                        cmd.CommandText =
                            "SELECT T1.postid, T1.catid, T1.posttitle, T1.postbrief, T1.postbody, T1.postenabled, T2.catid, T2.catname FROM posts AS T1 LEFT OUTER JOIN categories AS T2 ON T1.catid = T2.catid ORDER BY T1.postid DESC LIMIT 1";
                        cmd.Connection = conn;
                    }

                    else
                    {
                        cmd.CommandText =
                            "SELECT T1.postid, T1.catid, T1.posttitle, T1.postbrief, T1.postbody, T1.postenabled, T2.catid, T2.catname FROM posts AS T1 LEFT OUTER JOIN categories AS T2 ON T1.catid = T2.catid WHERE T1.postid= @POSTID";
                        cmd.Connection = conn;

                        var idParam = new NpgsqlParameter
                            {
                                ParameterName = "@POSTID",
                                NpgsqlDbType = NpgsqlDbType.Integer,
                                Size = 8,
                                Direction = ParameterDirection.Input,
                                Value = postId
                            };
                        cmd.Parameters.Add(idParam);
                    }

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    postView.DataSource = ds;
                    postView.DataBind();
                }

                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT postid, posttime, posttitle FROM posts",
                            conn);

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    postGrid.DataSource = ds;
                    postGrid.DataBind();
                }

                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT catid, catname FROM categories",
                            conn);

                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            categoryGrid.DataSource = dr;
                            categoryGrid.DataBind();
                        }

                        else
                        {
                            var dt = new DataTable();
                            dt.Columns.Add("catid");
                            dt.Columns.Add("catname");
                            DataRow row = dt.NewRow();
                            row["catid"] = "null";
                            row["catname"] = "null";
                            dt.Rows.Add(row);

                            categoryGrid.DataSource = dt;
                            categoryGrid.DataBind();

                            categoryGrid.Rows[0].Visible = false;
                            categoryGrid.Rows[0].Controls.Clear();
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
                            "SELECT T1.commentid, T1.commenttime, T1.userid, T1.postid, T1.commentbody, T2.userid, T2.useralias, T3.postid, T3.posttitle FROM comments AS T1 LEFT OUTER JOIN users AS T2 ON T1.userid = T2.userid LEFT OUTER JOIN posts AS T3 ON T1.postid = T3.postid",
                            conn);

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    commentGrid.DataSource = ds;
                    commentGrid.DataBind();
                }

                errorLabel.Text = "";
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }

        protected void PostView_ItemDeleting(object sender, DetailsViewDeleteEventArgs e)
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
                                CommandText = "DELETE FROM posts WHERE postid= @POSTID",
                                CommandType = CommandType.Text,
                                Connection = conn
                            };

                        var param = new NpgsqlParameter
                            {
                                ParameterName = "@POSTID",
                                NpgsqlDbType = NpgsqlDbType.Integer,
                                Size = 8,
                                Direction = ParameterDirection.Input,
                                Value = Convert.ToInt32(postView.SelectedValue)
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

            Reset_Page(-1);
        }

        protected void PostView_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            int postId = Convert.ToInt32(postView.SelectedValue);
            var txtPostTitle = (TextBox) postView.FindControl("txtPostTitle");
            var txtPostBrief = (TextBox) postView.FindControl("txtPostBrief");
            var txtPostBody = (TextBox) postView.FindControl("txtPostBody");
            var postEnabledCheckbox = (CheckBox) postView.FindControl("postEnabledCheckbox");
            var catDropdown = (DropDownList) postView.FindControl("catDropdown");

            if (!string.IsNullOrEmpty(txtPostTitle.Text) && !string.IsNullOrEmpty(txtPostBrief.Text) &&
                !string.IsNullOrEmpty(txtPostBody.Text))
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
                                        "UPDATE posts SET (catid, posttitle, postbrief, postbody, postenabled) = (@CATID, @POSTTITLE, @POSTBRIEF, @POSTBODY, @POSTENABLED) WHERE postid= @POSTID",
                                    CommandType = CommandType.Text,
                                    Connection = conn
                                };

                            var idParam = new NpgsqlParameter
                                {
                                    ParameterName = "@POSTID",
                                    NpgsqlDbType = NpgsqlDbType.Integer,
                                    Size = 8,
                                    Direction = ParameterDirection.Input,
                                    Value = postId
                                };
                            cmd.Parameters.Add(idParam);

                            var catParam = new NpgsqlParameter
                                {
                                    ParameterName = "@CATID",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 16,
                                    Direction = ParameterDirection.Input,
                                    Value = catDropdown.SelectedValue
                                };
                            cmd.Parameters.Add(catParam);

                            var titleParam = new NpgsqlParameter
                                {
                                    ParameterName = "@POSTTITLE",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 32,
                                    Direction = ParameterDirection.Input,
                                    Value = txtPostTitle.Text
                                };
                            cmd.Parameters.Add(titleParam);

                            var briefParam = new NpgsqlParameter
                                {
                                    ParameterName = "@POSTBRIEF",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 1024,
                                    Direction = ParameterDirection.Input,
                                    Value = txtPostBrief.Text
                                };
                            cmd.Parameters.Add(briefParam);

                            var bodyParam = new NpgsqlParameter
                                {
                                    ParameterName = "@POSTBODY",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Direction = ParameterDirection.Input,
                                    Value = Server.HtmlDecode(txtPostBody.Text)
                                };
                            cmd.Parameters.Add(bodyParam);

                            var enabledParam = new NpgsqlParameter
                                {
                                    ParameterName = "@POSTENABLED",
                                    NpgsqlDbType = NpgsqlDbType.Boolean,
                                    Direction = ParameterDirection.Input,
                                    Value = postEnabledCheckbox.Checked
                                };
                            cmd.Parameters.Add(enabledParam);

                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
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

            Reset_Page(postId);
        }

        protected void PostView_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            var txtPostTitle = (TextBox) postView.FindControl("txtPostTitle");
            var txtPostBrief = (TextBox) postView.FindControl("txtPostBrief");
            var txtPostBody = (TextBox) postView.FindControl("txtPostBody");
            var postEnabledCheckbox = (CheckBox) postView.FindControl("postEnabledCheckbox");
            var catDropdown = (DropDownList) postView.FindControl("catDropdown");

            if (!string.IsNullOrEmpty(txtPostTitle.Text) && !string.IsNullOrEmpty(txtPostBrief.Text) &&
                !string.IsNullOrEmpty(txtPostBody.Text))
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
                                        "INSERT INTO posts(catid, posttime, posttitle, postbrief, postbody, postenabled) VALUES (@CATID, @POSTTIME, @POSTTITLE, @POSTBRIEF, @POSTBODY, @POSTENABLED)",
                                    CommandType = CommandType.Text,
                                    Connection = conn
                                };

                            var catParam = new NpgsqlParameter
                                {
                                    ParameterName = "@CATID",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 16,
                                    Direction = ParameterDirection.Input,
                                    Value = catDropdown.SelectedValue
                                };
                            cmd.Parameters.Add(catParam);

                            var timeParam = new NpgsqlParameter
                                {
                                    ParameterName = "@POSTTIME",
                                    NpgsqlDbType = NpgsqlDbType.Timestamp,
                                    Size = 32,
                                    Direction = ParameterDirection.Input,
                                    Value = DateTime.Now
                                };
                            cmd.Parameters.Add(timeParam);

                            var titleParam = new NpgsqlParameter
                                {
                                    ParameterName = "@POSTTITLE",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 32,
                                    Direction = ParameterDirection.Input,
                                    Value = txtPostTitle.Text
                                };
                            cmd.Parameters.Add(titleParam);

                            var briefParam = new NpgsqlParameter
                                {
                                    ParameterName = "@POSTBRIEF",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 1024,
                                    Direction = ParameterDirection.Input,
                                    Value = txtPostBrief.Text
                                };
                            cmd.Parameters.Add(briefParam);

                            var bodyParam = new NpgsqlParameter
                                {
                                    ParameterName = "@POSTBODY",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Direction = ParameterDirection.Input,
                                    Value = Server.HtmlDecode(txtPostBody.Text)
                                };
                            cmd.Parameters.Add(bodyParam);

                            var enabledParam = new NpgsqlParameter
                                {
                                    ParameterName = "@POSTENABLED",
                                    NpgsqlDbType = NpgsqlDbType.Boolean,
                                    Direction = ParameterDirection.Input,
                                    Value = postEnabledCheckbox.Checked
                                };
                            cmd.Parameters.Add(enabledParam);

                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
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

            Reset_Page(-1);
        }

        protected void PostView_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {
            switch (e.NewMode)
            {
                case DetailsViewMode.Edit:
                    postView.ChangeMode(DetailsViewMode.Edit);
                    break;
                case DetailsViewMode.Insert:
                    postView.ChangeMode(DetailsViewMode.Insert);
                    break;
                default:
                    postView.ChangeMode(DetailsViewMode.ReadOnly);
                    break;
            }

            if (postView.SelectedValue != null)
            {
                Load_Forms(Convert.ToInt32(postView.SelectedValue));
            }
            else
            {
                Load_Forms(-1);
            }
        }

        protected void PostView_Databound(object sender, EventArgs e)
        {
            if (postView.CurrentMode == DetailsViewMode.ReadOnly && postView.Rows.Count > 1)
            {
                var postEnabledLabel = (Label) postView.FindControl("postEnabledLabel");

                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT postenabled FROM posts WHERE postid= @POSTID",
                            conn);

                    var idParam = new NpgsqlParameter
                        {
                            ParameterName = "@POSTID",
                            NpgsqlDbType = NpgsqlDbType.Integer,
                            Direction = ParameterDirection.Input,
                            Value = Convert.ToInt32(postView.SelectedValue)
                        };
                    cmd.Parameters.Add(idParam);

                    bool postEnabled = Convert.ToBoolean(cmd.ExecuteScalar());

                    postEnabledLabel.Text = postEnabled ? "Yes" : "No";
                }
            }

            else
            {
                var catDropdown = (DropDownList) postView.FindControl("catDropdown");
                var postEnabledCheckbox = (CheckBox) postView.FindControl("postEnabledCheckbox");

                try
                {
                    using (var conn = new NpgsqlConnection())
                    {
                        conn.ConnectionString =
                            ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                        conn.Open();

                        var cmd =
                            new NpgsqlCommand(
                                "SELECT catid, catname FROM categories",
                                conn);

                        using (NpgsqlDataReader dr = cmd.ExecuteReader())
                        {
                            catDropdown.DataSource = dr;
                            catDropdown.DataValueField = "catid";
                            catDropdown.DataTextField = "catname";
                            catDropdown.DataBind();
                        }
                    }

                    if (postView.CurrentMode == DetailsViewMode.Edit)
                    {
                        using (var conn = new NpgsqlConnection())
                        {
                            conn.ConnectionString =
                                ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                            conn.Open();

                            var cmd =
                                new NpgsqlCommand(
                                    "SELECT catid, postenabled FROM posts WHERE postid= @POSTID",
                                    conn);

                            var idParam = new NpgsqlParameter
                                {
                                    ParameterName = "@POSTID",
                                    NpgsqlDbType = NpgsqlDbType.Integer,
                                    Size = 8,
                                    Direction = ParameterDirection.Input,
                                    Value = Convert.ToInt32(postView.SelectedValue)
                                };
                            cmd.Parameters.Add(idParam);

                            NpgsqlDataReader dr = cmd.ExecuteReader();

                            while (dr.Read())
                            {
                                postEnabledCheckbox.Checked = Convert.ToBoolean(dr["postenabled"]);
                                catDropdown.SelectedValue = dr["catid"].ToString();
                            }

                            dr.Close();
                        }
                    }
                }

                catch (Exception ex)
                {
                    Reset_Page(Convert.ToInt32(postView.SelectedValue));
                    FooLogging.WriteLog(ex.ToString());
                    errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
                }
            }
        }

        protected void PostGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int postId = Convert.ToInt32(postGrid.Rows[postGrid.SelectedIndex].Cells[0].Text);

                Load_Forms(postId);
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }

        protected void CategoryGrid_Delete(object sender, GridViewDeleteEventArgs e)
        {
            int postId = Convert.ToInt32(postView.SelectedValue);

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
                                CommandText = "DELETE FROM posts WHERE catid= @CATID",
                                CommandType = CommandType.Text,
                                Connection = conn
                            };

                        var param = new NpgsqlParameter
                            {
                                ParameterName = "@CATID",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 16,
                                Direction = ParameterDirection.Input,
                                Value =
                                    FooStringHelper.RemoveInvalidChars(
                                        categoryGrid.DataKeys[e.RowIndex].Values[0].ToString())
                            };
                        cmd.Parameters.Add(param);

                        cmd.ExecuteNonQuery();
                    }

                    // Define connection string.
                    using (var conn = new NpgsqlConnection())
                    {
                        conn.ConnectionString =
                            ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                        conn.Open();

                        var cmd = new NpgsqlCommand
                            {
                                CommandText = "DELETE FROM categories WHERE catid= @CATID",
                                CommandType = CommandType.Text,
                                Connection = conn
                            };

                        var param = new NpgsqlParameter
                            {
                                ParameterName = "@CATID",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 16,
                                Direction = ParameterDirection.Input,
                                Value =
                                    FooStringHelper.RemoveInvalidChars(
                                        categoryGrid.DataKeys[e.RowIndex].Values[0].ToString())
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

            Reset_Page(postId);
        }

        protected void CategoryGrid_Edit(object sender, GridViewEditEventArgs e)
        {
            try
            {
                categoryGrid.EditIndex = e.NewEditIndex;
                Load_Forms(Convert.ToInt32(postView.SelectedValue));
            }
            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }

        protected void CategoryGrid_Cancel(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                Reset_Page(Convert.ToInt32(postView.SelectedValue));
            }
            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }

        protected void CategoryGrid_Update(object sender, GridViewUpdateEventArgs e)
        {
            int postId = Convert.ToInt32(postView.SelectedValue);

            var txtCatName = (TextBox) categoryGrid.Rows[e.RowIndex].FindControl("txtCatName");

            if (!string.IsNullOrEmpty(txtCatName.Text))
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
                                        "UPDATE categories SET catname= @NAME WHERE catid= @CATID",
                                    CommandType = CommandType.Text,
                                    Connection = conn
                                };

                            var nameParam = new NpgsqlParameter
                                {
                                    ParameterName = "@NAME",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 32,
                                    Direction = ParameterDirection.Input,
                                    Value = txtCatName.Text
                                };
                            cmd.Parameters.Add(nameParam);

                            var idParam = new NpgsqlParameter
                                {
                                    ParameterName = "@CATID",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 16,
                                    Direction = ParameterDirection.Input,
                                    Value =
                                        FooStringHelper.RemoveInvalidChars(
                                            categoryGrid.DataKeys[e.RowIndex].Values[0].ToString())
                                };
                            cmd.Parameters.Add(idParam);

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
            }

            else
            {
                errorLabel.Text = "Incomplete or invalid input.";
            }

            Reset_Page(postId);
        }

        protected void CategoryGrid_Command(object sender, GridViewCommandEventArgs e)
        {
            int postId = Convert.ToInt32(postView.SelectedValue);
            var txtCatNameFooter = (TextBox) categoryGrid.FooterRow.FindControl("txtCatNameFooter");

            if (!string.IsNullOrEmpty(txtCatNameFooter.Text))
            {
                try
                {
                    if (FooSessionHelper.IsValidRequest(HttpContext.Current, RequestToken.Value))
                    {
                        if (e.CommandName.Equals("AddNew"))
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
                                            "INSERT INTO categories(catid, catname) VALUES (@CATID, @NAME)",
                                        CommandType = CommandType.Text,
                                        Connection = conn
                                    };

                                var idParam = new NpgsqlParameter
                                    {
                                        ParameterName = "@CATID",
                                        NpgsqlDbType = NpgsqlDbType.Varchar,
                                        Size = 16,
                                        Direction = ParameterDirection.Input,
                                        Value = FooStringHelper.RandomString(16)
                                    };
                                cmd.Parameters.Add(idParam);

                                var nameParam = new NpgsqlParameter
                                    {
                                        ParameterName = "@NAME",
                                        NpgsqlDbType = NpgsqlDbType.Varchar,
                                        Size = 32,
                                        Direction = ParameterDirection.Input,
                                        Value = txtCatNameFooter.Text
                                    };
                                cmd.Parameters.Add(nameParam);

                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
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
            }

            else
            {
                errorLabel.Text = "Incomplete or invalid input.";
            }

            Reset_Page(postId);
        }

        protected void CommentGrid_Delete(object sender, GridViewDeleteEventArgs e)
        {
            int postId = Convert.ToInt32(postView.SelectedValue);

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
                                CommandText = "DELETE FROM comments WHERE commentid= @COMMENTID",
                                CommandType = CommandType.Text,
                                Connection = conn
                            };

                        var param = new NpgsqlParameter
                            {
                                ParameterName = "@COMMENTID",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 16,
                                Direction = ParameterDirection.Input,
                                Value =
                                    FooStringHelper.RemoveInvalidChars(
                                        commentGrid.DataKeys[e.RowIndex].Values[0].ToString())
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

            Reset_Page(postId);
        }
    }
}