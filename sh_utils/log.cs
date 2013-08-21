using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WOCL.Shared.Utils
{
    /// <summary>
    /// Provides logging messages to a file or may also be used as a stub when no file loggin needed 
    /// (posting messages is exeption-safe)
    /// </summary>
    public class LogManager
    {
        /// <summary>
        /// Default LogManager, (=null)
        /// </summary>
        public static LogManager Default;
        public static void Post(string msg, params object[] args)
        {
            if (Default != null)
                Default[DateTime.Now] = string.Format(msg, args);
        }    

        public StreamWriter File;

        public LogManager(string logFileName)
        {
            File = new StreamWriter(logFileName + "_" + DateTime.Now.ToString("ddMMYYhhmmss") + ".log");
        }
        public string this[DateTime time]
        {
            set
            {
                File.WriteLine(time.ToLongTimeString() + " : " + value);
            }
        }
        ~LogManager()
        {
            File.Close();
        }
    }
}
