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
    using log4net;
    using log4net.Config;
    using log4net.Repository;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;

    public static class Log4NetLogger
    {
        const string ConfigFile = "log4net.config";

        static log4net.ILog _Log = null;
        static Log4NetLogger()
        {
            var configFile = ConfigFile.FixedAppBasePath();
            ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
            if (configFile.IsNotEmpty() && System.IO.File.Exists(configFile))
            {
                Console.WriteLine($"load {ConfigFile}");
                try
                {
                    XmlConfigurator.Configure(repository, new FileInfo(configFile));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"load log4net.config error:\r\n{ex.ToString()}");
                }
            }
            _Log = LogManager.GetLogger(repository.Name, "NETCorelog4net");
        }

        //static Log4netLogger()
        //{
        //    try
        //    {
        //        log4net.Config.XmlConfigurator.Configure();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("初始化Log4Net配置出错：" + ex.Message);
        //    }
        //}

        public static void Logger(object sender, string message, Category category)
        {
            log4net.ILog logger = null;
            if (sender == null)
            {
                logger = log4net.LogManager.GetLogger("Common");
            }
            else
            {
                if (sender is string)
                {
                    logger = log4net.LogManager.GetLogger(sender.ToString());
                }
                else
                {
                    logger = log4net.LogManager.GetLogger(sender.GetType());
                }
            }
            switch (category)
            {
                case Category.Debug:
                    logger.Debug(message);
                    break;
                case Category.Exception:
                    logger.Error(message);
                    break;
                case Category.Fatal:
                    logger.Fatal(message);
                    break;
                case Category.Info:
                    logger.Info(message);
                    break;
                case Category.Warn:
                    logger.Warn(message);
                    break;
            }
        }

        public static void Log_Info(string message)
        {
            var log = GetLogger();
            log.Info(message);
        }
        public static void Log_Info(string message, Exception ex)
        {
            var log = GetLogger();
            log.Info(message, ex);
        }

        public static void Log_Debug(string message)
        {
            var log = GetLogger();
            log.Debug(message);
        }
        public static void Log_Debug(string message, Exception ex)
        {
            var log = GetLogger();
            log.Debug(message, ex);
        }

        public static void Log_Warn(string message)
        {
            var log = GetLogger();
            log.Warn(message);
        }
        public static void Log_Warn(string message, Exception ex)
        {
            var log = GetLogger();
            log.Warn(message, ex);
        }

        public static void Log_Error(string message)
        {
            var log = GetLogger();
            log.Error(message);
        }
        public static void Log_Error(string message, Exception ex)
        {
            var log = GetLogger();
            log.Error(message, ex);
        }

        public static void Log_Fatal(string message)
        {
            var log = GetLogger();
            log.Fatal(message);
        }
        public static void Log_Fatal(string message, Exception ex)
        {
            var log = GetLogger();
            log.Fatal(message, ex);
        }

        private static log4net.ILog GetLogger()
        {
            log4net.ILog log = log4net.LogManager.GetLogger(GetCallerType(3));
            return log;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Type GetCallerType(int skip = 2)
        {
            return new StackFrame(skip, false).GetMethod().DeclaringType;
        }
    }
}
