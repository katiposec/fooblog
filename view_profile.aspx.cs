using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using Npgsql;
using NpgsqlTypes;

namespace FooBlog
{
    public partial class view_profile : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string userId = FooStringHelper.RemoveInvalidChars(Request.QueryString["id"]);

            if (FooStringHelper.IsValidAlphanumeric(userId, 16))
            {
                Load_Forms(userId);
            }

            else
            {
                errorLabel.Text = "Invalid user.";
            }
        }

        protected void Load_Forms(string userId)
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
                            "SELECT userid, useralias, city, country, profileimg, profilebody FROM users WHERE userid= @USERID",
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

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        errorLabel.Text = "Invalid user.";
                    }

                    else
                    {
                        userList.DataSource = ds;
                        userList.DataBind();
                        errorLabel.Text = "";
                    }
                }
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text =
                    "Something has gone wrong. A log has been forwarded to the site administrator. Error:<br/>" + ex;
            }
        }
    }
}