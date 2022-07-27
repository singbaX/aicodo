// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using AiCodo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace AiCodo.Web.Controllers
{
    [Authorize]
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

            #region 从请求对象获取执行参数
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
