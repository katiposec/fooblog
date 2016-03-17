using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using Npgsql;
using NpgsqlTypes;

namespace FooBlog
{
    public partial class search : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string searchTerm = Request.Form["ctl00$searchText"];

            if (!string.IsNullOrEmpty(searchTerm))
            {
                Load_Forms(searchTerm);
            }

            else
            {
                errorLabel.Text = "Invalid search.";
            }
        }

        protected void Load_Forms(string searchTerm)
        {
            errorLabel.Text = "";
            termLabel.Text = String.Format("Showing Results For: {0}", searchTerm);
            searchPanel.Visible = true;

            try
            {
                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT T1.postid, T1.posttime, T1.catid, T1.posttitle, T1.postbrief, T1.postenabled, T2.catid, T2.catname FROM posts AS T1 LEFT OUTER JOIN categories AS T2 ON T1.catid = T2.catid WHERE T1.postenabled= true AND LOWER(T1.posttitle) LIKE '%" +
                            searchTerm.ToLower() + "%' ORDER BY T1.posttime",
                            conn);

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        postErrorLabel.Text = "There are no posts.";
                    }

                    else
                    {
                        postList.DataSource = ds;
                        postList.DataBind();
                        postErrorLabel.Text = "";
                    }
                }

                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT merchid, merchname, merchprice, merchbrief FROM merchandise WHERE merchenabled= true AND LOWER(merchname) LIKE @MERCHNAME ORDER BY merchname",
                            conn);

                    var merchParam = new NpgsqlParameter
                        {
                            ParameterName = "@MERCHNAME",
                            NpgsqlDbType = NpgsqlDbType.Varchar,
                            Size = 32,
                            Direction = ParameterDirection.Input,
                            Value = string.Format("%{0}%", searchTerm.ToLower())
                        };
                    cmd.Parameters.Add(merchParam);

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        merchErrorLabel.Text = "There are no items.";
                    }

                    else
                    {
                        merchList.DataSource = ds;
                        merchList.DataBind();
                        merchErrorLabel.Text = "";
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