using System;
using System.Globalization;
using System.IO;

namespace FooBlog
{
    public class FooLogging
    {
        public static void WriteLog(string error)
        {
            string strPath = AppDomain.CurrentDomain.BaseDirectory;
            string strLogFilePath = strPath + @"logs\error.log";

            if (error.Contains("Thread was being aborted")) return;
            if (!File.Exists(strLogFilePath))
            {
                File.Create(strLogFilePath).Close();
            }
            using (StreamWriter w = File.AppendText(strLogFilePath))
            {
                w.WriteLine("\r\nLog: ");
                w.WriteLine("{0}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                string err = "Message:" + error;
                w.WriteLine(err);
            }
        }
    }
}