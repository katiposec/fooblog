using System;
using System.IO;
using System.Web;
using FooBlog.common;
using Newtonsoft.Json;

namespace FooBlog
{
    /// <summary>
    ///     Summary description for reset_handler
    /// </summary>
    public class reset_handler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string userId = FooSessionHelper.GetUserObjectFromCookie(context).UserId;

            string jsonString = new StreamReader(context.Request.InputStream).ReadToEnd();

            var resetObj = JsonConvert.DeserializeObject<ResetObject>(jsonString);

            string password = resetObj.Password.Trim();
            string confirmation = resetObj.Confirmation.Trim();

            if (password != confirmation)
            {
                context.Response.Write("Reset Failed");
            }

            if (!String.IsNullOrEmpty(password))
            {
                bool reset = do_reset.UpdatePassword(userId, password);

                if (reset)
                {
                    string email = FooEmailHelper.GetEmailForAccount(userId);

                    var emailObj = new EmailObject
                        {
                            Body =
                                "Your FooBlog password has been reset. If you did not perform this action, please contact a FooBlog administrator using your registered email account",
                            Subject = "FooBlog Password Reset",
                            ToAddress = email
                        };

                    FooEmailHelper.SendEmail(emailObj);

                    context.Response.Write("Reset OK");
                }

                else
                {
                    context.Response.Write("Reset Failed");
                }
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}