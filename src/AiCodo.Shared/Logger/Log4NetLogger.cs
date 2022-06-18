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

        public static void Logger(object sender, string message, Category category)
        {
            log4net.ILog logger = _Log;
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
    }
}
