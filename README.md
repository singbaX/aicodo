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
## add config file "sql.xml"
* add file "App_Data\\configs\\sql.xml"
* change parameter value for your own: Server=localhost;Port=12306;Database=demo1;Uid=root;Pwd=sa123456;CharSet=utf8;SslMode=none;
```
<?xml version="1.0" encoding="utf-8" ?>
<SqlData AutoGenerateItems="true">
	<Connections>
		<Connection Name="aicodo" ProviderName="mysql" ConnectionString="Server=localhost;Port=12306;Database=demo1;Uid=root;Pwd=sa123456;CharSet=utf8;SslMode=none;"/>
	</Connections>
</SqlData>
```

## run 
![image](https://github.com/singbaX/aicodo/blob/main/doc/samples/images/pageindex.png)

add "service/sys_user/selectall" in url
![image](https://github.com/singbaX/aicodo/blob/main/doc/samples/images/httpget.png)

## sql.xml auto changed
```
<?xml version="1.0"?>
<SqlData xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" AutoGenerateItems="true">
  <Connections>
    <Connection Name="aicodo" ProviderName="mysql" ConnectionString="Server=localhost;Port=12306;Database=demo1;Uid=root;Pwd=sa123456;CharSet=utf8;SslMode=none;">
      <Table Name="sys_user" Schema="demo1" CodeName="User" Key="ID">
        <Columns>
          <Column Name="ID" DisplayName="自动编号" ColumnOrdinal="1" ColumnType="int(11)" DataType="int" DbType="String" PropertyType="" PropertyName="" Remark="" DefaultValue="" Length="0" NullAble="false" IsAutoIncrement="true" IsReadOnly="true" IsKey="true" IsSystem="false" SystemParameter="" />
          <Column Name="UserName" DisplayName="用户名" ColumnOrdinal="2" ColumnType="varchar(50)" DataType="varchar" DbType="String" PropertyType="" PropertyName="" Remark="" DefaultValue="''" Length="50" NullAble="false" IsAutoIncrement="false" IsReadOnly="false" IsKey="false" IsSystem="false" SystemParameter="" />
          <Column Name="Email" DisplayName="用户邮箱" ColumnOrdinal="3" ColumnType="varchar(200)" DataType="varchar" DbType="String" PropertyType="" PropertyName="" Remark="" DefaultValue="''" Length="200" NullAble="false" IsAutoIncrement="false" IsReadOnly="false" IsKey="false" IsSystem="false" SystemParameter="" />
          <Column Name="Password" DisplayName="密码" Comment="这是一个密码" ColumnOrdinal="4" ColumnType="varchar(200)" DataType="varchar" DbType="String" PropertyType="" PropertyName="" Remark="" DefaultValue="''" Length="200" NullAble="false" IsAutoIncrement="false" IsReadOnly="false" IsKey="false" IsSystem="false" SystemParameter="" />
          <Column Name="CreateUser" DisplayName="创建用户" ColumnOrdinal="5" ColumnType="int(11)" DataType="int" DbType="String" PropertyType="" PropertyName="" Remark="" DefaultValue="" Length="0" NullAble="false" IsAutoIncrement="false" IsReadOnly="false" IsKey="false" IsSystem="false" SystemParameter="" />
          <Column Name="CreateTime" DisplayName="创建时间" ColumnOrdinal="6" ColumnType="datetime" DataType="datetime" DbType="String" PropertyType="" PropertyName="" Remark="" DefaultValue="" Length="0" NullAble="false" IsAutoIncrement="false" IsReadOnly="false" IsKey="false" IsSystem="false" SystemParameter="" />
          <Column Name="UpdateUser" DisplayName="修改用户" ColumnOrdinal="7" ColumnType="int(11)" DataType="int" DbType="String" PropertyType="" PropertyName="" Remark="" DefaultValue="" Length="0" NullAble="false" IsAutoIncrement="false" IsReadOnly="false" IsKey="false" IsSystem="false" SystemParameter="" />
          <Column Name="UpdateTime" DisplayName="修改时间" ColumnOrdinal="8" ColumnType="datetime" DataType="datetime" DbType="String" PropertyType="" PropertyName="" Remark="" DefaultValue="" Length="0" NullAble="false" IsAutoIncrement="false" IsReadOnly="false" IsKey="false" IsSystem="false" SystemParameter="" />
          <Column Name="IsValid" DisplayName="是否有效" ColumnOrdinal="9" ColumnType="bit(1)" DataType="bit" DbType="String" PropertyType="" PropertyName="" Remark="" DefaultValue="" Length="0" NullAble="false" IsAutoIncrement="false" IsReadOnly="false" IsKey="false" IsSystem="false" SystemParameter="" />
        </Columns>
      </Table>
    </Connection>
  </Connections>
  <Groups>
    <Group Name="aicodo">
      <Table Name="sys_user" ConnectionName="aicodo" TableName="sys_user">
        <Sql Name="Insert" ConnectionName="aicodo" SqlType="Scalar" Description="新增" IsGenerate="true">
          <CommandText>
INSERT INTO `sys_user`
(`UserName`,`Email`,`Password`,`CreateUser`,`CreateTime`,`UpdateUser`,`UpdateTime`,`IsValid`)
VALUES(@UserName,@Email,@Password,@CreateUser,@CreateTime,@UpdateUser,@UpdateTime,@IsValid);
SELECT LAST_INSERT_ID() AS `ID`;
</CommandText>
          <Parameters />
        </Sql>
        <Sql Name="Delete" ConnectionName="aicodo" SqlType="Execute" Description="删除" IsGenerate="true">
          <CommandText>
DELETE FROM `sys_user` 
 WHERE `ID`=@ID
</CommandText>
          <Parameters />
        </Sql>
        <Sql Name="Update" ConnectionName="aicodo" SqlType="Execute" Description="更新" IsGenerate="true">
          <CommandText>
UPDATE `sys_user` SET 
`UserName`=@UserName
,`Email`=@Email
,`Password`=@Password
,`CreateUser`=@CreateUser
,`CreateTime`=@CreateTime
,`UpdateUser`=@UpdateUser
,`UpdateTime`=@UpdateTime
,`IsValid`=@IsValid
 WHERE `ID`=@ID
</CommandText>
          <Parameters />
        </Sql>
        <Sql Name="SelectAll" ConnectionName="aicodo" Description="全选" IsGenerate="true">
          <CommandText>
SELECT `ID`,`UserName`,`Email`,`Password`,`CreateUser`,`CreateTime`,`UpdateUser`,`UpdateTime`,`IsValid`
FROM `sys_user` 
</CommandText>
          <Parameters />
        </Sql>
        <Sql Name="SelectByKeys" ConnectionName="aicodo" Description="主键选择" IsGenerate="true">
          <CommandText>
SELECT `ID`,`UserName`,`Email`,`Password`,`CreateUser`,`CreateTime`,`UpdateUser`,`UpdateTime`,`IsValid`
FROM `sys_user` 
WHERE `ID`=@ID
</CommandText>
          <Parameters />
        </Sql>
        <Sql Name="Count" ConnectionName="aicodo" SqlType="Scalar" Description="记录数" IsGenerate="true">
          <CommandText>
SELECT count(*) FROM `sys_user` 
</CommandText>
          <Parameters />
        </Sql>
      </Table>
    </Group>
  </Groups>
</SqlData>
```
