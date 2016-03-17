using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using Npgsql;
using NpgsqlTypes;

namespace FooBlog
{
    public partial class view_category : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string catId = Request.QueryString["id"];

            if (FooStringHelper.IsValidAlphanumeric(catId, 16))
            {
                Load_Forms(catId);
            }

            else
            {
                errorLabel.Text = "Invalid category.";
            }
        }

        protected void Load_Forms(string catId)
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
                            "SELECT catname FROM categories WHERE catid= @CATID ORDER BY catname",
                            conn);

                    var catParam = new NpgsqlParameter
                        {
                            ParameterName = "@CATID",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 32,
                            Direction = ParameterDirection.Input,
                            Value = catId
                        };
                    cmd.Parameters.Add(catParam);

                    object catName = cmd.ExecuteScalar();

                    if (catName != null)
                    {
                        catLabel.Text = catName.ToString();
                    }
                }

                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT T1.postid, T1.posttime, T1.catid AS queryid, T1.posttitle, T1.postbrief, T2.catid, T2.catname FROM posts AS T1 LEFT OUTER JOIN categories AS T2 ON T1.catid = T2.catid WHERE T2.catid= @CATID AND postenabled= true ORDER BY T1.posttime",
                            conn);

                    var catParam = new NpgsqlParameter
                        {
                            ParameterName = "@CATID",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 32,
                            Direction = ParameterDirection.Input,
                            Value = catId
                        };
                    cmd.Parameters.Add(catParam);

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        errorLabel.Text = "Empty category.";
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