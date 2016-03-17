using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using FooBlog.common;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;

namespace FooBlog
{
    public partial class login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("index.aspx");
            }
        }
    }
}