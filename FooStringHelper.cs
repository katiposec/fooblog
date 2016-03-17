using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace FooBlog
{
    public class FooStringHelper
    {
        // STRING GENERATION

        public static string RandomString(int length)
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            if (length < 0) throw new ArgumentOutOfRangeException("length", @"length cannot be less than zero.");
            if (string.IsNullOrEmpty(allowedChars)) throw new ArgumentException("allowedChars may not be empty.");

            const int byteSize = 0x100;
            char[] allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length)
                throw new ArgumentException(String.Format("allowedChars may contain no more than {0} characters.",
                                                          byteSize));

            using (var rng = new RNGCryptoServiceProvider())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < length)
                {
                    rng.GetBytes(buf);
                    for (int i = 0; i < buf.Length && result.Length < length; ++i)
                    {
                        int outOfRangeStart = byteSize - (byteSize%allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i]) continue;
                        result.Append(allowedCharSet[buf[i]%allowedCharSet.Length]);
                    }
                }
                return result.ToString();
            }
        }

        public static string RandomFileName(string fileName)
        {
            string fileBase = Path.GetFileNameWithoutExtension(fileName);
            string ext = Path.GetExtension(fileName);
            return String.Format("{0}_{1}{2}", fileBase, RandomString(8), ext);
        }

        // VALIDATION

        public static bool IsValidEmailAddress(string emailaddress)
        {
            try
            {
                var m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool IsValidPrice(string input)
        {
            float num;
            return float.TryParse(input, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out num);
        }

        public static string RemoveInvalidChars(string input)
        {
            var r = new Regex("[^-A-Za-z0-9+/=]|=[^=]|={3,}$");
            return r.Replace(input, "");
        }

        public static string DateTimeToString(DateTime input)
        {
            try
            {
                return input.ToString("d/M/yyyy @ h:mmtt");
            }
            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                return string.Empty;
            }
        }

        public static bool IsValidAlphanumeric(string input, int length)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var r = new Regex(String.Format("^[a-zA-Z0-9]{{{0}}}$", length.ToString()));

                if (r.IsMatch(input))
                {
                    return true;
                }
            }

            return false;
        }

        // URL's
        public static string MakeImageUrl(string fileName)
        {
            string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                             HttpContext.Current.Request.ApplicationPath;

            string resetRequest = baseUrl.EndsWith("/")
                                      ? string.Format("uploads/{0}", fileName)
                                      : string.Format("/uploads/{0}", fileName);

            return baseUrl + resetRequest;
        }

        public static string MakeResetUrl(string resetId, string token)
        {
            string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                             HttpContext.Current.Request.ApplicationPath;

            string resetRequest = baseUrl.EndsWith("/")
                                      ? string.Format("do_reset.aspx?id={0}&token={1}", resetId, token)
                                      : string.Format("/do_reset.aspx?id={0}&token={1}", resetId, token);

            return baseUrl + resetRequest;
        }
    }
}