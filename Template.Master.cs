using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using FooBlog.common;
using Npgsql;

namespace FooBlog
{
    public partial class Template : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                anonPanel.Visible = false;
                userPanel.Visible = true;

                UserObject userObj = FooSessionHelper.GetUserObjectFromCookie(HttpContext.Current);
                string adminGroup = ConfigurationManager.AppSettings["Admin Group ID"];
                if (userObj.GroupId == adminGroup)
                {
                    adminPanel.Visible = true;
                }
            }

            Load_Form();
        }

        protected void Load_Form()
        {
            searchText.Text = "";

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
                        if (!dr.HasRows) return;
                        catRepeater.DataSource = dr;
                        catRepeater.DataBind();
                    }
                }
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
            }
        }
    }
}