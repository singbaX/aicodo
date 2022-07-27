// *************************************************************************************************
// 代码文件：FlowContext.cs
// 
// 作    者：SingbaX
// 
// 创建日期：202206
// 
// 功能说明：流程执行上下文
//
// 修改记录：
// *************************************************************************************************

namespace AiCodo.Flow.Configs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    public class FlowContext
    {
        private Dictionary<string, object> _Args { get; set; } = new Dictionary<string, object>();

        private Dictionary<string, object> _Results { get; set; } = new Dictionary<string, object>();

        public string ID { get; set; }

        public string TrackID { get; set; }

        public string FlowID { get; set; }

        public string FlowName { get; set; }

        public string LastErrorCode { get; private set; } = "";

        public FlowContext(Dictionary<string, object> args = null)
        {
            if (args != null)
            {
                foreach (var item in args)
                {
                    _Args[item.Key] = item.Value;
                }
            }
        }

        public FlowContext SetArgs(string name, object value)
        {
            _Args[name] = value;
            return this;
        }

        public IEnumerable<KeyValuePair<string, object>> GetArgs()
        {
            foreach (var item in _Args)
            {
                yield return item;
            }
        }

        public IEnumerable<KeyValuePair<string, object>> GetResults()
        {
            foreach (var item in _Results)
            {
                yield return item;
            }
        }

        public Task<DynamicEntity> ExecuteFlow(FunctionFlowConfig flow)
        {
            FlowID = flow.ID;
            FlowName = flow.Name;

            return Task.Run(() =>
            {                
                var actions = flow.GetActions().ToList();
                var data = ExecuteFlowActions(flow, actions, out var flowArgs);
                if (flow.Results.Count > 0)
                {
                    var exp = ExpressionHelper.GetInterpreter(flowArgs);
                    foreach (var r in flow.Results)
                    {
                        object value = null;
                        if (r.Expression.IsNullOrEmpty())
                        {
                            if(flowArgs.TryGetValue(r.Name,out value))
                            {
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            value = exp.Eval(r.Expression);
                        }
                        data.SetValue(r.Name, value);
                    }
                }
                return data;
            });
        }

        internal DynamicEntity ExecuteFlowActions(FunctionFlowConfig flow, IEnumerable<FlowActionBase> actions, out Dictionary<string, object> flowArgs)
        {
            var result = new DynamicEntity();
            var errorCode = "";
            var errorMessage = "";
            flowArgs = CreateFlowArgs(flow);

            flowArgs["HasError"] = false;
            try
            {
                var actionIndex = 0;
                foreach (var action in actions)
                {
                    var actionStartTime = DateTime.Now;
                    flowArgs["LastErrorCode"] = LastErrorCode;
                    var exp = ExpressionHelper.GetInterpreter(flowArgs);
                    errorCode = "";
                    errorMessage = "";
                    actionIndex++;

                    #region execute action
                    try
                    {
                        var tryCount = 0;
                        var goNext = false;

                        while (!goNext)
                        {
                            tryCount++;
                            if (!action.TryRun(flow, flowArgs, out var actionResult))
                            {
                                continue;
                            }

                            #region 处理返回值
                            LastErrorCode = actionResult.ErrorCode;

                            if (actionResult.IsOK())
                            {
                                ResetResult(result, flowArgs, action, actionResult);
                                goNext = true;
                                break;
                            }
                            else
                            {
                                AddLog($"执行[{action.Name}]出错：{errorCode}-{actionResult.ErrorMessage}");
                                if (flow.ErrorMode == FlowErrorMode.Break)
                                {
                                    errorCode = OnSetError(result, action, actionResult);
                                    goNext = false;
                                    break;
                                }

                                if (flow.ErrorMode == FlowErrorMode.Continue)
                                {
                                    flowArgs["HasError"] = true;
                                    goNext = true;
                                    break;
                                }

                                if (flow.ErrorMode == FlowErrorMode.Retry)
                                {
                                    if (flow.RetryCount > 0 && flow.RetryCount > tryCount)
                                    {
                                        goNext = true;
                                        break;
                                    }
                                    else
                                    {
                                        errorCode = OnSetError(result, action, actionResult);
                                        goNext = false;
                                    }
                                }
                            }
                            #endregion
                        }

                        if (!goNext)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        CheckFileNotFoundException(ex);
                        if (errorCode.IsNullOrEmpty())
                        {
                            errorCode = FunctionResult.UnknowError;
                        }
                        AddLog(ex.ToString(), Category.Exception);
                        throw new FunctionExecuteException(ex.Message, errorCode);
                    }
                    #endregion
                }
            }
            catch (FunctionExecuteException aex)
            {
                errorCode = aex.ErrorCode;
                errorMessage = aex.Message;
            }
            catch (Exception ex)
            {
                errorCode = "";
                errorMessage = ex.Message;
            }

            if (errorCode.IsNotEmpty() && errorCode != FunctionResult.OK)
            {
                result.SetValue("ErrorCode", errorCode);
            }
            if (errorMessage.IsNotEmpty())
            {
                result.SetValue("ErrorMessage", $"[{ID}] " + errorMessage);
            }

            return result;
        }

        private Dictionary<string, object> CreateFlowArgs(FunctionFlowConfig flow)
        {
            var flowArgs = new Dictionary<string, object>();
            _Args.ForEach(a => flowArgs[a.Key] = a.Value);

            var flowParameters = flow.GetParameters().ToList();
            if (flowParameters.Count > 0)
            {
                flowParameters.ForEach(p =>
                {
                    object value = "";
                    if (p.DefaultValue.StartsWith("="))
                    {
                        value = p.DefaultValue.Substring(1).Eval(flowArgs);
                    }
                    else
                    {
                        if (flowArgs.TryGetValue(p.Name, out value))
                        {
                        }
                        else
                        {
                            value = p.DefaultValue;
                        }
                    }
                    flowArgs[p.Name] = p.GetValue(value);
                });
            }

            return flowArgs;
        }

        static void ResetResult(DynamicEntity result, Dictionary<string, object> flowArgs, FlowActionBase action, IFunctionResult functionResult)
        {
            foreach (var r in action.ResultParameters)
            {
                if (!functionResult.TryGetValue(r.Name, out object rvalue))
                {
                    throw new FunctionExecuteException($"算法[{action.Name}]，没有返回值[{r.Name}]", FunctionResult.FlowConfigError);
                }

                if (r.ResultName.IsNotEmpty())
                {
                    result.SetValue(r.ResultName, rvalue);
                }

                if (r.ResetInputName.IsNotEmpty())
                {
                    flowArgs[r.ResetInputName] = rvalue;
                }

                if (rvalue != null && rvalue is IList list)
                {
                }
            }
        }

        private string OnSetError(DynamicEntity result, FlowActionBase action, IFunctionResult actionResult)
        {
            string errorCode = actionResult.ErrorCode;
            result.SetValue("ErrorCode", errorCode);
            result.SetValue("ErrorMessage", actionResult.ErrorMessage);
            return errorCode;
        }

        private void CheckFileNotFoundException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            if (ex is FileNotFoundException fex)
            {
                if (fex.FileName.IsNotEmpty())
                {
                    var names = fex.FileName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (names.Length == 0)
                    {
                        return;
                    }

                    var fileName = $"{names[0]}.dll".FixedAppBasePath();
                    if (fileName.IsFileExists())
                    {
                        try
                        {
                            var asm = Assembly.LoadFrom(fileName);
                        }
                        catch (Exception loadEx)
                        {
                            AddLog(loadEx.ToString());
                        }
                    }
                }
            }
        }

        internal static object GetDefaultValue(ParameterItem p)
        {
            return p.GetValue(p.DefaultValue);
        }

        private void AddLog(string line, Category category = Category.Info)
        {
            this.Log($"[{ID}] {line}", category);
        }
    }
}
