using System.Configuration;
using System.Web;
using System.Web.Security;
using FooBlog.common;
using Newtonsoft.Json;

namespace FooBlog
{
    public class FooSessionHelper
    {
        public static UserObject GetUserObjectFromCookie(HttpContext context)
        {
            HttpCookie cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

                if (ticket != null)
                {
                    if (!string.IsNullOrEmpty(ticket.UserData))
                    {
                        return JsonConvert.DeserializeObject<UserObject>(ticket.UserData);
                    }
                }
            }

            return null;
        }

        public static void AuthenticationCheck(HttpContext context)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                context.Response.Redirect("login.aspx");
            }
        }

        public static string SetToken(HttpContext context)
        {
            string value = FooStringHelper.RandomString(24);
            string encryptedValue = FooCryptHelper.MachineEncrypt(value);
            string cookieName = ConfigurationManager.AppSettings["CSRF Cookie Name"];

            var ck = new HttpCookie(cookieName, encryptedValue)
                {
                    Path = FormsAuthentication.FormsCookiePath
                };

            context.Response.Cookies.Add(ck);

            return value;
        }

        public static bool IsValidRequest(HttpContext context, string formValue)
        {
            string cookieName = ConfigurationManager.AppSettings["CSRF Cookie Name"];
            HttpCookie httpCookie = context.Request.Cookies[cookieName];

            if (httpCookie != null)
            {
                string userToken = FooCryptHelper.MachineDecrypt(httpCookie.Value);

                if (!FooStringHelper.IsValidAlphanumeric(userToken, 24) ||
                    !FooStringHelper.IsValidAlphanumeric(formValue, 24))
                {
                    return false;
                }

                return userToken == formValue;
            }

            return false;
        }
    }
}