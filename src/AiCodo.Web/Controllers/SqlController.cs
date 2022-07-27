// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using AiCodo.Data;
using AiCodo.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace AiCodo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SqlController : ControllerBase
    {
        #region /sql/conn/create
        [HttpPost]
        [Route("/sql/conn/create")]
        public IActionResult CreateConn()
        {
            DynamicEntity data = Request.Body.ReadToEnd();
            if (data == null)
            {
                throw new Exception("参数不正确");
            }
            var name = data.GetString("name");
            if (name.IsNullOrEmpty())
            {
                throw new Exception("缺少必须参数[name]");
            }
            if (SqlData.Current.Connections[name] != null)
            {
                throw new Exception($"连接[{name}]已存在");
            }
            var providerName = data.GetString("provider");
            if (providerName.IsNullOrEmpty())
            {
                throw new Exception("缺少必须参数[provider]");
            }
            var provider = DbProviderFactories.GetProvider(providerName);
            if (provider == null)
            {
                throw new Exception($"没有对应的数据库处理[{providerName}]");
            }
            var connectionString = provider.CreateConnectionString(data);
            var sqlConn = new SqlConnection { Name = name, ProviderName = providerName, ConnectionString = connectionString };
            SqlData.Current.Connections.Add(sqlConn);
            sqlConn.ReloadTables();
            SqlData.Current.Save();

            return Ok();
        }
        #endregion

        #region /sql/conn
        [HttpGet]
        [Route("/sql/conns")]
        public IActionResult GetConns()
        {
            var items = SqlData.Current.Connections
                .Select(c => new ConnItem { Name = c.Name, ProviderName = c.ProviderName })
                .ToList();
            return new JsonResult(items);
        }
        #endregion

        #region /sql/tables
        [HttpGet]
        [Route("/sql/tables")]
        public IActionResult GetTables([FromQuery] string connName)
        {
            var conn = SqlData.Current.Connections[connName];
            if (conn == null)
            {
                throw new System.Exception(string.Format("连接[{0}]不存在", connName));
            }

            var items = conn.Tables
                .Select(c => new TableItem { Name = c.Name, DisplayName = c.DisplayName })
                .ToList();
            return new JsonResult(items);
        }
        #endregion

        #region /sql/schema
        [HttpGet]
        [Route("/sql/columns")]
        public IActionResult GetSchema([FromQuery] string connName, [FromQuery] string tableName)
        {
            var conn = SqlData.Current.Connections[connName];
            if (conn == null)
            {
                throw new System.Exception(string.Format("连接[{0}]不存在", connName));
            }
            var table = conn.Tables[tableName];
            if (table == null)
            {
                throw new System.Exception(string.Format("数据表[{0}]不存在", tableName));
            }

            return new JsonResult(table.Columns.Select(c => new ColumnItem
            {

            }).ToList());
        }
        #endregion
    }
}
