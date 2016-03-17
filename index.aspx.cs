using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using Npgsql;

namespace FooBlog
{
    public partial class index : Page
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
                            "SELECT T1.postid, T1.posttime, T1.catid, T1.posttitle, T1.postbrief, T1.postenabled, T2.catid, T2.catname FROM posts AS T1 LEFT OUTER JOIN categories AS T2 ON T1.catid = T2.catid WHERE T1.postenabled= true ORDER BY T1.posttime",
                            conn);

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        errorLabel.Text = "There are no posts.";
                    }

                    else
                    {
                        postList.DataSource = ds;
                        postList.DataBind();
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