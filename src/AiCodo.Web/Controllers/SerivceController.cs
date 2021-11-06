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
