using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.SqlTypes;

namespace CellCounter.WebHost
{
    public class HostService
    {
        public static IHostBuilder CreateHostBuilder(params string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (args != null && args.Length > 0)
                    {
                        webBuilder.UseUrls(args);
                    }
                    else
                    {
                        webBuilder.UseUrls("http://localhost:18080");
                    }
                    webBuilder.UseStartup<Startup>();
                });
    }
}
