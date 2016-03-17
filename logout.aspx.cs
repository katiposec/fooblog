using System;
using System.Web.Security;
using System.Web.UI;

namespace FooBlog
{
    public partial class logout : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }

            Response.Redirect("index.aspx");
        }
    }
}