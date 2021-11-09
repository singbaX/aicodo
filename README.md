# aicodo

## mysql使用场景
如果您想要建立一个基于Mysql数据库的后台系统，下面使用AiCodo的方案步骤

* Create New Project:[asp.net core web app]
 ![image](https://github.com/singbaX/aicodo/blob/main/doc/samples/images/createproj.png)
 
* Add Nuget Lib:AiCodo.Data.MySql(include "AiCodo"、"AiCodo.Data")
![image](https://github.com/singbaX/aicodo/blob/main/doc/samples/images/addnuget.png)

* edit Startup
** add mysql provider
```
        public void ConfigureServices(IServiceCollection services)
        {          
            //Step01:set db provider(AiCodo)
            DbProviderFactories.SetFactory("mysql", MySqlProvider.Instance);

            services.AddRazorPages();
        }
```
** set base config path
```
            //Step02:set config files path(AiCodo)
            ApplicationConfig.LocalDataFolder = System.IO.Path.Combine(env.ContentRootPath, "App_Data");
            ApplicationConfig.LocalConfigFolder = "configs".FixedAppDataPath();
```
** add controller support
```
            app.UseEndpoints(endpoints =>
            {
                //Step03:add controller
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();

                endpoints.MapRazorPages();
            });
```
** Startup.cs
```
using AiCodo.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCodo.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {          
            //Step01:set db provider(AiCodo)
            DbProviderFactories.SetFactory("mysql", MySqlProvider.Instance);

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Step02:set config files path(AiCodo)
            ApplicationConfig.LocalDataFolder = System.IO.Path.Combine(env.ContentRootPath, "App_Data");
            ApplicationConfig.LocalConfigFolder = "configs".FixedAppDataPath();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //Step03:add controller
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();

                endpoints.MapRazorPages();
            });
        }
    }
}

```
