using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using Npgsql;
using NpgsqlTypes;

namespace FooBlog
{
    public partial class view_item : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            string merchId = Request.QueryString["id"];

            if (FooStringHelper.IsValidAlphanumeric(merchId, 16))
            {
                RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
                Load_Forms(merchId);
            }

            else
            {
                errorLabel.Text = "Invalid item.";
            }
        }

        protected void Load_Forms(string merchId)
        {
            try
            {
                bool isValidItem = false;

                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT merchid, merchname, merchprice, merchimg, merchbody FROM merchandise WHERE merchenabled= true AND merchid= @MERCHID LIMIT 1",
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

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        errorLabel.Text = "Invalid item.";
                    }

                    else
                    {
                        merchList.DataSource = ds;
                        merchList.DataBind();
                        errorLabel.Text = "";
                        isValidItem = true;
                    }
                }

                if (!isValidItem) return;

                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT T1.reviewid, T1.reviewtime, T1.reviewbody, T1.merchid, T2.userid, T2.useralias, T2.profileimg FROM reviews AS T1 LEFT OUTER JOIN users AS T2 ON T1.userid = T2.userid WHERE T1.merchid= @MERCHID",
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

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        reviewLabel.Text = "No reviews.";
                    }

                    else
                    {
                        reviewList.DataSource = ds;
                        reviewList.DataBind();
                        reviewLabel.Text = "";
                    }
                }

                if (!User.Identity.IsAuthenticated)
                {
                    reviewText.Visible = false;
                    submitButton.Visible = false;
                    reviewErrorLabel.Text = "You must be logged in to leave a review.";
                }

                reviewPanel.Visible = true;
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text = "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {
            string reviewBody = reviewText.Text;
            string userId = FooSessionHelper.GetUserObjectFromCookie(HttpContext.Current).UserId;
            string merchId = Request.QueryString["id"];

            if (string.IsNullOrEmpty(reviewBody))
            {
                RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
                reviewErrorLabel.Text = "Incomplete input.";
                return;
            }

            if (!FooStringHelper.IsValidAlphanumeric(merchId, 16))
            {
                RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
                reviewErrorLabel.Text = "Invalid input.";
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
                                CommandText =
                                    "INSERT INTO reviews(reviewid, reviewtime, userid, merchid, reviewbody) VALUES (@REVIEWID, @REVIEWTIME, @USERID, @MERCHID, @REVIEWBODY)",
                                CommandType = CommandType.Text,
                                Connection = conn
                            };

                        var idParam = new NpgsqlParameter
                            {
                                ParameterName = "@REVIEWID",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 16,
                                Direction = ParameterDirection.Input,
                                Value = FooStringHelper.RandomString(16)
                            };
                        cmd.Parameters.Add(idParam);

                        var timeParam = new NpgsqlParameter
                            {
                                ParameterName = "@REVIEWTIME",
                                NpgsqlDbType = NpgsqlDbType.Timestamp,
                                Size = 32,
                                Direction = ParameterDirection.Input,
                                Value = DateTime.Now
                            };
                        cmd.Parameters.Add(timeParam);

                        var userParam = new NpgsqlParameter
                            {
                                ParameterName = "@USERID",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 16,
                                Direction = ParameterDirection.Input,
                                Value = FooStringHelper.RemoveInvalidChars(userId)
                            };
                        cmd.Parameters.Add(userParam);

                        var merchParam = new NpgsqlParameter
                            {
                                ParameterName = "@MERCHID",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 16,
                                Direction = ParameterDirection.Input,
                                Value = merchId
                            };
                        cmd.Parameters.Add(merchParam);

                        var bodyParam = new NpgsqlParameter
                            {
                                ParameterName = "@REVIEWBODY",
                                NpgsqlDbType = NpgsqlDbType.Varchar,
                                Size = 1024,
                                Direction = ParameterDirection.Input,
                                Value = reviewBody
                            };
                        cmd.Parameters.Add(bodyParam);

                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        reviewErrorLabel.Text = "";
                        reviewText.Text = "";
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
                reviewErrorLabel.Text =
                    "Something has gone wrong. A log has been forwarded to the site administrator.";
            }

            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
            Load_Forms(merchId);
        }
    }
}