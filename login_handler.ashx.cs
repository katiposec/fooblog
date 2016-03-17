using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using FooBlog.common;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;

namespace FooBlog
{
    /// <summary>
    /// Summary description for login_handler
    /// </summary>
    public class login_handler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var isValidLogin = DoLogin(username, password);

                if (isValidLogin)
                {
                    UserObject userObj = GetUserObjectByName(username);
                    string userString = JsonConvert.SerializeObject(userObj);

                    // MAKE TIMEOUT VARIABLE BASED ON APPSETTING.
                    var ticket = new FormsAuthenticationTicket(1, username, DateTime.Now,
                                                               DateTime.Now.AddMinutes(30),
                                                               false, userString,
                                                               FormsAuthentication.FormsCookiePath);
                    string cookieString = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieString);
                    context.Response.Cookies.Add(cookie);
                    context.Response.Write("OK");
                }

                else
                {
                    context.Response.Write("Login Failed");
                }
            }

            else
            {
                context.Response.Write("Login Invalid");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public static bool DoLogin(string username, string pass)
        {
            try
            {
                using (var conn = new NpgsqlConnection())
                {
                    // App-DB connection.
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();
                    var cmd = new NpgsqlCommand
                    {
                        CommandText =
                                "SELECT passwordhash FROM users WHERE username= @USERNAME",
                        CommandType = CommandType.Text,
                        Connection = conn
                    };

                    var nameParam = new NpgsqlParameter
                    {
                        ParameterName = "@USERNAME",
                        NpgsqlDbType = NpgsqlDbType.Varchar,
                        Size = 32,
                        Direction = ParameterDirection.Input,
                        Value = username
                    };
                    cmd.Parameters.Add(nameParam);

                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    string result = string.Empty;

                    while (dr.Read())
                    {
                        result = dr["passwordhash"].ToString();
                    }

                    dr.Close();

                    if (!string.IsNullOrEmpty(result))
                    {
                        string hash = FooCryptHelper.CreateShaHash(pass);
                        if (hash == result)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                return false;
            }
        }

        public static UserObject GetUserObjectByName(string username)
        {
            try
            {
                using (var conn = new NpgsqlConnection())
                {
                    // App-DB connection.
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["fooPostgreSQL"].ConnectionString;
                    conn.Open();
                    var cmd = new NpgsqlCommand
                    {
                        CommandText =
                                "SELECT userid, useralias, groupid FROM users WHERE username= @USERNAME",
                        CommandType = CommandType.Text,
                        Connection = conn
                    };

                    var nameParam = new NpgsqlParameter
                    {
                        ParameterName = "@USERNAME",
                        NpgsqlDbType = NpgsqlDbType.Varchar,
                        Size = 32,
                        Direction = ParameterDirection.Input,
                        Value = username
                    };
                    cmd.Parameters.Add(nameParam);

                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    string userAlias = string.Empty;
                    string userId = string.Empty;
                    string groupId = string.Empty;

                    while (dr.Read())
                    {
                        userAlias = dr["useralias"].ToString();
                        userId = dr["userid"].ToString();
                        groupId = dr["groupid"].ToString();
                    }

                    dr.Close();

                    if (!string.IsNullOrEmpty(userAlias) && !string.IsNullOrEmpty(userId) &&
                        !string.IsNullOrEmpty(groupId))
                    {
                        var userObj = new UserObject
                        {
                            Username = username,
                            UserAlias = userAlias,
                            UserId = userId,
                            GroupId = groupId
                        };

                        return userObj;
                    }

                    return null;
                }
            }

            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                return null;
            }
        }
    }
}