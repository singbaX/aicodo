// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
namespace AiCodo.Services
{
    using AiCodo.Data;
    using AiCodo.Flow.Configs;
    using System.Threading.Tasks;
    public static class FlowFactory
    {
        static FlowFactory()
        {
            MethodServiceFactory.RegisterService("sql", SqlMethodService.Current);
        }

        public static Task<ServiceResult> Run(string flowName, Dictionary<string, object> args)
        {
            if (TryGetFlow(flowName, out var flow))
            {
                return Run(flow, args);
            }
            throw new Exception($"处理流程不存在[{flowName}]");

            //return RunSql(flowName, args);
        }

        public static Task<ServiceResult> Run(FunctionFlowConfig flow, Dictionary<string, object> args)
        {
            var context = new FlowContext(args);
            return context.ExecuteFlow(flow)
                .ContinueWith<ServiceResult>(t =>
                {
                    SqlConnectionContext? connContext = null;
                    if (context.TryGetArg(SqlMethodService.ConnectionArgName, out var connItem))
                    {
                        connContext = connItem as SqlConnectionContext;
                    }

                    if (t.Exception != null)
                    {
                        if (connContext != null)
                        {
                            connContext.HasError = true;
                            connContext.End();
                        }

                        if (t.Exception.InnerException is FunctionExecuteException fex)
                        {
                            return new ServiceResult { Error = fex.Message, ErrorCode = fex.ErrorCode };
                        }
                        var error = t.Exception.InnerException != null ?
                            t.Exception.InnerException.Message :
                            t.Exception.Message;

                        var result = new ServiceResult { Error = error };
                        return result;
                    }
                    else
                    {
                        var data = t.Result;
                        var errorCode = data.GetString("ErrorCode");
                        var errorMessage = data.GetString("ErrorMessage");
                        var hasError = errorCode.IsNotNullOrEmpty() || errorMessage.IsNotNullOrEmpty();

                        if (connContext != null)
                        {
                            connContext.HasError = hasError;
                            connContext.End();
                        }

                        data.Remove("ErrorCode");
                        data.Remove("ErrorMessage");
                        var result = new ServiceResult { Data = t.Result, Error = errorMessage, ErrorCode = errorCode };

                        return result;
                    }
                });
        }

        private static Task<ServiceResult> RunSql(string serviceName, Dictionary<string, object> args)
        {
            return Task.Run<ServiceResult>(() =>
            {
                ServiceResult result = null;
                try
                {
                    var sqlContext = new SqlRequest
                    {
                        SqlName = serviceName,
                        Parameters = args
                    };
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
                return result;
            });
        }

        public static bool TryGetFlow(string flowName, out FunctionFlowConfig flow)
        {
            var item = FunctionFlowIndex.Current.Items.FirstOrDefault(f => f.Name.Equals(flowName, StringComparison.OrdinalIgnoreCase)
                || f.ID == flowName);
            if (item != null)
            {
                flow = FunctionFlowConfig.Load(item.ID);
                return true;
            }

            flow = null;
            return false;
        }
    }
}
