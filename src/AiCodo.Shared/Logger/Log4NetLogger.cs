// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
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
