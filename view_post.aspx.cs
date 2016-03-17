using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using Npgsql;
using NpgsqlTypes;

namespace FooBlog
{
    public partial class view_post : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
            Load_Forms();
        }

        protected void Load_Forms()
        {
            try
            {
                bool isValidPost = false;

                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT T1.postid, T1.posttime, T1.catid, T1.posttitle, T1.postbody, T2.catid, T2.catname FROM posts AS T1 LEFT OUTER JOIN categories AS T2 ON T1.catid = T2.catid WHERE postid='" +
                            Request.QueryString["id"] + "'",
                            conn);

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        errorLabel.Text = "Invalid post.";
                    }

                    else
                    {
                        postList.DataSource = ds;
                        postList.DataBind();
                        errorLabel.Text = "";
                        isValidPost = true;
                    }
                }

                if (!isValidPost) return;

                using (var conn = new NpgsqlConnection())
                {
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();

                    var cmd =
                        new NpgsqlCommand(
                            "SELECT T1.commentid, T1.commenttime, T1.commentbody, T1.postid, T2.userid, T2.useralias, T2.profileimg FROM comments AS T1 LEFT OUTER JOIN users AS T2 ON T1.userid = T2.userid WHERE T1.postid='" +
                            Request.QueryString["id"] + "'",
                            conn);

                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        commentLabel.Text = "No comments.";
                    }

                    else
                    {
                        commentList.DataSource = ds;
                        commentList.DataBind();
                        commentLabel.Text = "";
                    }
                }

                if (!User.Identity.IsAuthenticated)
                {
                    commentText.Visible = false;
                    submitButton.Visible = false;
                    commentErrorLabel.Text = "You must be logged in to leave a comment.";
                }

                commentPanel.Visible = true;
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                errorLabel.Text =
                    "Something has gone wrong. A log has been forwarded to the site administrator.";
            }
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {
            string commentBody = commentText.Text;
            string userId = FooSessionHelper.GetUserObjectFromCookie(HttpContext.Current).UserId;
            string postId = Request.QueryString["id"];

            if (!string.IsNullOrEmpty(commentBody))
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
                                        "INSERT INTO comments(commentid, commenttime, userid, postid, commentbody) VALUES (@COMMENTID, @COMMENTTIME, @USERID, @POSTID, @COMMENTBODY)",
                                    CommandType = CommandType.Text,
                                    Connection = conn
                                };

                            var idParam = new NpgsqlParameter
                                {
                                    ParameterName = "@COMMENTID",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 16,
                                    Direction = ParameterDirection.Input,
                                    Value = FooStringHelper.RandomString(16)
                                };
                            cmd.Parameters.Add(idParam);

                            var timeParam = new NpgsqlParameter
                                {
                                    ParameterName = "@COMMENTTIME",
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

                            var postParam = new NpgsqlParameter
                                {
                                    ParameterName = "@POSTID",
                                    NpgsqlDbType = NpgsqlDbType.Integer,
                                    Size = 16,
                                    Direction = ParameterDirection.Input,
                                    Value = FooStringHelper.RemoveInvalidChars(postId)
                                };
                            cmd.Parameters.Add(postParam);

                            var bodyParam = new NpgsqlParameter
                                {
                                    ParameterName = "@COMMENTBODY",
                                    NpgsqlDbType = NpgsqlDbType.Varchar,
                                    Size = 1024,
                                    Direction = ParameterDirection.Input,
                                    Value = commentBody
                                };
                            cmd.Parameters.Add(bodyParam);

                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            commentText.Text = "";
                            commentErrorLabel.Text = "";
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
                    commentErrorLabel.Text =
                        "Something has gone wrong. A log has been forwarded to the site administrator.";
                }

                Load_Forms();
            }

            else
            {
                commentErrorLabel.Text = "Incomplete input.";
            }

            RequestToken.Value = FooSessionHelper.SetToken(HttpContext.Current);
        }
    }
}