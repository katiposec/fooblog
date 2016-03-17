using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using Npgsql;

namespace FooBlog
{
    public partial class merchandise : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Load_Form();
        }

        protected void Load_Form()
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
                            "SELECT merchid, merchname, merchprice, merchbrief FROM merchandise WHERE merchenabled= true ORDER BY merchname",
                            conn);

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        errorLabel.Text = "There are no items.";
                    }

                    else
                    {
                        merchList.DataSource = ds;
                        merchList.DataBind();
                        errorLabel.Text = "";
                    }
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