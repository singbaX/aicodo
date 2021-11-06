/* 
 * author      : singba 
 * email       : singba@163.com 
 * version     : 20210831
 * package     : AiCodo
 * license     : MIT
 * description : let me think a while
 */
namespace AiCodo
{
    using System;
    using System.Collections.Concurrent;
    public static class FileLogger
    {
        private static string AppPath = ApplicationConfig.LocalDataFolder;

        static object ErrorLock = new object();

        private static ConcurrentBag<string> _Files = new ConcurrentBag<string>();

        public static void WriteErrorLog(this Exception ex)
        {
            lock (ErrorLock)
            {
                ex.ToString().AppendFileLog(string.Format("log\\error{0}.log", DateTime.Now.Date.ToString("yyyyMMdd")));
            }
        }

        public static void WriteErrorLog(this string message)
        {
            lock (ErrorLock)
            {
                message.AppendFileLog(string.Format("log\\error{0}.log", DateTime.Now.Date.ToString("yyyyMMdd")));
            }
        }

        public static void WriteFileLog(this string text, string filename)
        {
            try
            {
                string path = System.IO.Path.Combine(AppPath, filename.Replace("/", "\\"));
                string dir = path.Substring(0, path.LastIndexOf('\\'));
                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
                lock (path)
                {
                    using (var sw = System.IO.File.Exists(path) ? System.IO.File.AppendText(path) : System.IO.File.CreateText(path))
                    {
                        sw.Write(text);
                        sw.Close();
                    }
                }
            }
            catch
            {
            }
        }

        public static void AppendFileLog(this string text, string filename, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            try
            {
                string path = System.IO.Path.Combine(AppPath, filename.Replace("/", "\\"));
                string dir = path.Substring(0, path.LastIndexOf('\\'));
                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
                using (var sw = System.IO.File.Exists(path) ? System.IO.File.AppendText(path) : System.IO.File.CreateText(path))
                {
                    sw.WriteLine("[{0}]{1}", DateTime.Now.ToString(dateFormat), text);
                    sw.Close();
                }
            }
            catch
            {
            }
        }
    }
}
