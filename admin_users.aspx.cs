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
    public partial class admin_users : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FooSessionHelper.AuthenticationCheck(HttpContext.Current);

            if (IsPostBack) return;

            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
            Load_Forms();
            Load_Dropdown();
        }

        protected void Reset_Page()
        {
            userGrid.EditIndex = -1;
            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
            Load_Forms();
            Load_Dropdown();
        }

        protected void Load_Forms()
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
                            "SELECT T1.userid, T1.username, T1.useralias, T1.email, T1.groupid, T2.groupid, T2.groupname FROM users AS T1 LEFT OUTER JOIN groups AS T2 ON T1.groupid = T2.groupid",
                            conn);

                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        if (dr.HasRows)
                        {
                            userGrid.DataSource = dr;
                            userGrid.DataBind();
                        }

                        else
                        {
                            var dt = new DataTable();
                            dt.Columns.Add("userid");
                            dt.Columns.Add("username");
                            dt.Columns.Add("useralias");
                            dt.Columns.Add("email");
                            dt.Columns.Add("groupname");
                            DataRow row = dt.NewRow();
                            row["userid"] = "null";
                            row["username"] = "null";
                            row["useralias"] = "null";
                            row["email"] = "null";
                            row["groupname"] = "null";
                            dt.Rows.Add(row);

                            userGrid.DataSource = dt;
                            userGrid.DataBind();

                            userGrid.Rows[0].Visible = false;
                            userGrid.Rows[0].Controls.Clear();
                        }
                    }

                    dr.Close();
                }
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }

        protected void Load_Dropdown()
        {
            var footerDropDown = (DropDownList) userGrid.FooterRow.FindControl("groupDropdownFooter");

            try
            {
                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT groupid, groupname FROM groups",
                            conn);

                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        footerDropDown.DataSource = dr;
                        footerDropDown.DataValueField = "groupid";
                        footerDropDown.DataTextField = "groupname";
                        footerDropDown.DataBind();
                    }
                }
            }

                // SQL exception.
            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if ((e.Row.RowState & DataControlRowState.Edit) <= 0) return;

            var groupDropDown = (DropDownList) e.Row.FindControl("groupDropdown");

            try
            {
                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    string userId = userGrid.DataKeys[e.Row.RowIndex].Value.ToString();

                    var userIdCmd = new NpgsqlCommand
                        {
                            CommandText = "SELECT groupid FROM users WHERE userID= @USERID",
                            CommandType = CommandType.Text,
                            Connection = conn
                        };

                    var userIdParam = new NpgsqlParameter
                        {
                            ParameterName = "@USERID",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 16,
                            Direction = ParameterDirection.Input,
                            Value = userId
                        };
                    userIdCmd.Parameters.Add(userIdParam);

                    object groupId = userIdCmd.ExecuteScalar();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT groupid, groupname FROM groups",
                            conn);

                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        groupDropDown.DataSource = dr;
                        groupDropDown.DataValueField = "groupid";
                        groupDropDown.DataTextField = "groupname";
                        groupDropDown.DataBind();
                        groupDropDown.Items.FindByValue(groupId.ToString()).Selected = true;
                    }
                }
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }

        protected void GridView_Delete(object sender, GridViewDeleteEventArgs e)
        {
            string userId = userGrid.DataKeys[e.RowIndex].Values[0].ToString();

            if (!FooStringHelper.IsValidAlphanumeric(userId, 16))
            {
                errorLabel.Text = "Invalid request.";
                Reset_Page();
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
                                CommandText = "DELETE FROM users WHERE userid= @USERID",
                                CommandType = CommandType.Text,
                                Connection = conn
                            };

                        var param = new NpgsqlParameter
                            {
                                ParameterName = "@USERID",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 16,
                                Direction = ParameterDirection.Input,
                                Value = userId
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
                                CommandText = "DELETE FROM comments WHERE userid= @USERID",
                                CommandType = CommandType.Text,
                                Connection = conn
                            };

                        var param = new NpgsqlParameter
                            {
                                ParameterName = "@USERID",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 16,
                                Direction = ParameterDirection.Input,
                                Value = userGrid.DataKeys[e.RowIndex].Values[0].ToString()
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

            Reset_Page();
        }

        protected void GridView_Edit(object sender, GridViewEditEventArgs e)
        {
            try
            {
                userGrid.EditIndex = e.NewEditIndex;
                Load_Forms();
                Load_Dropdown();
            }
            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }

        protected void GridView_Cancel(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                Reset_Page();
            }
            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }

        protected void GridView_Update(object sender, GridViewUpdateEventArgs e)
        {
            string userId = userGrid.DataKeys[e.RowIndex].Values[0].ToString();

            if (!FooStringHelper.IsValidAlphanumeric(userId, 16))
            {
                errorLabel.Text = "Invalid request.";
                Reset_Page();
                return;
            }

            var txtUserName = (TextBox) userGrid.Rows[e.RowIndex].FindControl("txtUserName");
            var txtUserAlias = (TextBox) userGrid.Rows[e.RowIndex].FindControl("txtUserAlias");
            var txtEmail = (TextBox) userGrid.Rows[e.RowIndex].FindControl("txtEmail");
            var groupDropdown = (DropDownList) userGrid.Rows[e.RowIndex].FindControl("groupDropdown");

            if (!string.IsNullOrEmpty(txtUserName.Text) && !string.IsNullOrEmpty(txtUserAlias.Text) &&
                !string.IsNullOrEmpty(txtEmail.Text) && FooStringHelper.IsValidEmailAddress(txtEmail.Text))
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
                                        "UPDATE users SET (username, useralias, groupid, email) = (@NAME, @USERALIAS, @GROUP, @EMAIL) WHERE userID= @USERID",
                                    CommandType = CommandType.Text,
                                    Connection = conn
                                };

                            var param = new NpgsqlParameter
                                {
                                    ParameterName = "@USERID",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 16,
                                    Direction = ParameterDirection.Input,
                                    Value = userId
                                };
                            cmd.Parameters.Add(param);

                            var nameParam = new NpgsqlParameter
                                {
                                    ParameterName = "@USERNAME",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 32,
                                    Direction = ParameterDirection.Input,
                                    Value = txtUserName.Text
                                };
                            cmd.Parameters.Add(nameParam);

                            var dispParam = new NpgsqlParameter
                                {
                                    ParameterName = "@USERALIAS",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 32,
                                    Direction = ParameterDirection.Input,
                                    Value = txtUserAlias.Text
                                };
                            cmd.Parameters.Add(dispParam);

                            var emailParam = new NpgsqlParameter
                                {
                                    ParameterName = "@EMAIL",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 64,
                                    Direction = ParameterDirection.Input,
                                    Value = txtEmail.Text
                                };
                            cmd.Parameters.Add(emailParam);

                            var groupParam = new NpgsqlParameter
                                {
                                    ParameterName = "@GROUP",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 16,
                                    Direction = ParameterDirection.Input,
                                    Value = groupDropdown.SelectedValue
                                };
                            cmd.Parameters.Add(groupParam);

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

            Reset_Page();
        }

        protected void GridView_Command(object sender, GridViewCommandEventArgs e)
        {
            string userId = FooStringHelper.RandomString(16);
            var txtUserNameFooter = (TextBox) userGrid.FooterRow.FindControl("txtUserNameFooter");
            var txtUserAliasFooter = (TextBox) userGrid.FooterRow.FindControl("txtUserAliasFooter");
            var txtEmailFooter = (TextBox) userGrid.FooterRow.FindControl("txtEmailFooter");
            var txtUserPasswordFooter = (TextBox) userGrid.FooterRow.FindControl("txtUserPasswordFooter");
            var groupDropdownFooter = (DropDownList) userGrid.FooterRow.FindControl("groupDropdownFooter");

            if (!string.IsNullOrEmpty(txtUserNameFooter.Text) && !string.IsNullOrEmpty(txtUserAliasFooter.Text) &&
                !string.IsNullOrEmpty(txtEmailFooter.Text) && FooStringHelper.IsValidEmailAddress(txtEmailFooter.Text) &&
                !string.IsNullOrEmpty(txtUserPasswordFooter.Text))
            {
                try
                {
                    if (FooSessionHelper.IsValidRequest(HttpContext.Current, RequestToken.Value))
                    {
                        if (e.CommandName.Equals("AddNew"))
                        {
                            using (var conn = new NpgsqlConnection())
                            {
                                conn.ConnectionString =
                                    ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                                conn.Open();

                                var cmd = new NpgsqlCommand
                                    {
                                        CommandText =
                                            "INSERT INTO users(userid,username,useralias,groupid,email,passwordhash,profileimg) VALUES (@USERID,@NAME,@DISP,@GROUP,@EMAIL,@HASH,'profile_default.jpg')",
                                        CommandType = CommandType.Text,
                                        Connection = conn
                                    };

                                var userIdParam = new NpgsqlParameter
                                    {
                                        ParameterName = "@USERID",
                                        NpgsqlDbType = NpgsqlDbType.Varchar,
                                        Size = 16,
                                        Direction = ParameterDirection.Input,
                                        Value = userId
                                    };
                                cmd.Parameters.Add(userIdParam);

                                var nameParam = new NpgsqlParameter
                                    {
                                        ParameterName = "@NAME",
                                        NpgsqlDbType = NpgsqlDbType.Varchar,
                                        Size = 32,
                                        Direction = ParameterDirection.Input,
                                        Value = txtUserNameFooter.Text
                                    };
                                cmd.Parameters.Add(nameParam);

                                var dispParam = new NpgsqlParameter
                                    {
                                        ParameterName = "@DISP",
                                        NpgsqlDbType = NpgsqlDbType.Varchar,
                                        Size = 32,
                                        Direction = ParameterDirection.Input,
                                        Value = txtUserAliasFooter.Text
                                    };
                                cmd.Parameters.Add(dispParam);

                                var groupParam = new NpgsqlParameter
                                    {
                                        ParameterName = "@GROUP",
                                        NpgsqlDbType = NpgsqlDbType.Varchar,
                                        Direction = ParameterDirection.Input,
                                        Value = groupDropdownFooter.SelectedValue
                                    };
                                cmd.Parameters.Add(groupParam);

                                var emailParam = new NpgsqlParameter
                                    {
                                        ParameterName = "@EMAIL",
                                        NpgsqlDbType = NpgsqlDbType.Varchar,
                                        Size = 64,
                                        Direction = ParameterDirection.Input,
                                        Value = txtEmailFooter.Text
                                    };
                                cmd.Parameters.Add(emailParam);

                                var hashParam = new NpgsqlParameter
                                    {
                                        ParameterName = "@HASH",
                                        NpgsqlDbType = NpgsqlDbType.Varchar,
                                        Direction = ParameterDirection.Input,
                                        Value = FooCryptHelper.CreateShaHash(txtUserPasswordFooter.Text)
                                    };
                                cmd.Parameters.Add(hashParam);


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
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }

            Reset_Page();
        }
    }
}