# aicodo

# mysql使用场景
如果您想要建立一个基于Mysql数据库的后台系统，下面使用AiCodo的方案步骤

## Create New Project:[asp.net core web app]
 ![image](https://github.com/singbaX/aicodo/blob/main/doc/samples/images/createproj.png)
 
## Add Nuget Lib:AiCodo.Data.MySql(include "AiCodo"、"AiCodo.Data")
![image](https://github.com/singbaX/aicodo/blob/main/doc/samples/images/addnuget.png)

## edit Startup
### add mysql provider
```
        public void ConfigureServices(IServiceCollection services)
        {          
            //Step01:set db provider(AiCodo)
            DbProviderFactories.SetFactory("mysql", MySqlProvider.Instance);

            services.AddRazorPages();
        }
```
### set base config path
```
            //Step02:set config files path(AiCodo)
            ApplicationConfig.LocalDataFolder = System.IO.Path.Combine(env.ContentRootPath, "App_Data");
            ApplicationConfig.LocalConfigFolder = "configs".FixedAppDataPath();
```
### add controller support
```
            app.UseEndpoints(endpoints =>
            {
                //Step03:add controller
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();

                endpoints.MapRazorPages();
            });
```
### Startup.cs
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

## add controller [ServiceController]
![image](https://github.com/singbaX/aicodo/blob/main/doc/samples/images/addcontroller.png)
### ServiceController.cs
```
using AiCodo.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace AiCodo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        [HttpGet]
        [Route("/service/{table}/{sqlName}")]
        public IActionResult Get(string table, string sqlName)
        {
            return RunSqlService($"{table}.{sqlName}");
        }

        [HttpPost]
        [Route("/service/{table}/{sqlName}")]
        public IActionResult Post(string table, string sqlName)
        {
            return RunSqlService($"{table}.{sqlName}");
        }

        private IActionResult RunSqlService(string serviceName)
        {
            ServiceResult result = null;
            try
            {
                var sqlContext = CreateSqlContext(serviceName);
                var data = sqlContext.Execute();

                result = new ServiceResult
                {
                    Data = data
                };
            }
            catch (Exception ex)
            {
                result = new ServiceResult
                {
                    Error = ex.Message
                };
            }
            return new ContentResult()
            {
                Content = result.ToJson(),
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        private SqlRequest CreateSqlContext(string sqlName)
        {
            var parameters = new Dictionary<string, object>();

            #region 从请求对象获取执行参数 Get parameters from request object
            if (Request != null)
            {
                Request.Query
                    .ForEach(p => parameters[p.Key] = p.Value);

                if (Request.Body != null)
                {
                    DynamicEntity data = Request.Body.ReadToEnd();
                    if (data != null)
                    {
                        data.ForEach(p => parameters[p.Key] = p.Value);
                    }
                }
            }
            #endregion

            return new SqlRequest
            {
                SqlName = sqlName,
                Parameters = parameters
            };
        }
    }
}
```

## create database "demo1" (mysql)
### add table "sys_user"
```
CREATE TABLE `sys_user` (
	`ID` INT(11) NOT NULL AUTO_INCREMENT COMMENT '自动编号',
	`UserName` VARCHAR(50) NOT NULL DEFAULT '' COMMENT '用户名' COLLATE 'utf8_general_ci',
	`Email` VARCHAR(200) NOT NULL DEFAULT '' COMMENT '用户邮箱' COLLATE 'utf8_general_ci',
	`Password` VARCHAR(200) NOT NULL DEFAULT '' COMMENT '密码\r\n这是一个密码' COLLATE 'utf8_general_ci',
	`CreateUser` INT(11) NOT NULL COMMENT '创建用户',
	`CreateTime` DATETIME NOT NULL COMMENT '创建时间',
	`UpdateUser` INT(11) NOT NULL COMMENT '修改用户',
	`UpdateTime` DATETIME NOT NULL COMMENT '修改时间',
	`IsValid` BIT(1) NOT NULL COMMENT '是否有效',
	PRIMARY KEY (`ID`) USING BTREE
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
```
## 

