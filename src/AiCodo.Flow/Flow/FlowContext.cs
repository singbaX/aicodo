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
    using System.Collections.Generic;
    using System.Collections;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Threading;
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

        public Task<DynamicEntity> ExecuteFlow(FunctionFlowItem flow)
        {
            FlowID = flow.ID;
            FlowName = flow.Name;
            var flowStartTime = DateTime.Now;

            return Task.Run(() =>
            {
                var result = new DynamicEntity();

                var errorCode = "";
#pragma warning disable CS0219 // 变量“errorMessage”已被赋值，但从未使用过它的值
                var errorMessage = "";
#pragma warning restore CS0219 // 变量“errorMessage”已被赋值，但从未使用过它的值
                var flowArgs = new Dictionary<string, object>();
                _Args.ForEach(a => flowArgs[a.Key] = a.Value);
                flowArgs["HasError"] = false;

                List<string> runTags = new List<string>();
                flowArgs["RunTags"] = runTags;

                try
                {
                    var actions = flow.GetActions().ToList();
                    var actionIndex = 0;
                    foreach (var action in actions)
                    {
                        var actionStartTime = DateTime.Now;
                        flowArgs["LastErrorCode"] = LastErrorCode;
                        var exp = ExpressionHelper.GetInterpreter(flowArgs);
                        errorCode = "";
                        errorMessage = "";
                        actionIndex++;

                        if (action.Wait != null && action.Wait.Condition.IsNotEmpty())
                        {
                            var maxCount = action.Wait.MaxCount > 0 ? action.Wait.MaxCount : 0;
                            var checkMS = action.Wait.CheckMS > 0 ? action.Wait.CheckMS : 100;
                            var checkCount = 0;
                            while (!action.Wait.Condition.Eval(flowArgs).ToBoolean())
                            {
                                checkCount++;
                                if (maxCount > 0 && checkCount >= maxCount)
                                {
                                    this.Log($"等待条件失败，重试次数[{checkCount}]");
                                    break;
                                }
                                Thread.Sleep(checkMS);
                            }
                        }

                        if (action.Condition.IsNotEmpty())
                        {
                            if (!action.Condition.Eval(flowArgs).ToBoolean())
                            {
                                AddLog($"{action.Name} 不满足执行条件，执行跳过");
                                continue;
                            }
                        }

                        if (action.Asserts.Count > 0)
                        {
                            foreach (var assert in action.Asserts)
                            {
                                if (assert.Condition.IsNullOrEmpty())
                                {
                                    continue;
                                }

                                try
                                {
                                    var passed = exp.Eval(assert.Condition).ToBoolean();
                                    if (passed)
                                    {
                                        continue;
                                    }
                                    throw new FunctionExecuteException($"流程[{flow.Name}]节点[{action.Name}] Assert异常：{assert.Error}", FunctionResult.AssertError);
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                        }

                        if (action.FunctionName.IsNullOrEmpty())
                        {
                            throw new FunctionExecuteException($"流程[{flow.Name}]节点[{action.Name}]算法没有设置", FunctionResult.FlowConfigError);
                        }

                        var functionName = action.FunctionName;
                        var algItem = MethodServiceFactory.GetItem(functionName);
                        if (algItem == null)
                        {
                            throw new FunctionExecuteException($"算法[{action.FunctionName}]不存在", FunctionResult.NotFound);
                        }

                        #region execute action
                        try
                        {
                            var args = new Dictionary<string, object>();
                            #region 设置参数
                            foreach (var p in algItem.GetParameters())
                            {
                                try
                                {
                                    object pvalue = null;
                                    var actionParameter = action.Parameters.FirstOrDefault(f => f.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase));
                                    if (actionParameter != null)
                                    {
                                        if (actionParameter.IsInherit || actionParameter.Expression.IsNullOrEmpty())
                                        {
                                            object defaultValue = p.DefaultValue;
                                            pvalue = p.GetValue(defaultValue);
                                        }
                                        else
                                        {
                                            pvalue = p.GetValue(exp.Eval(actionParameter.Expression));
                                        }
                                        args[p.Name] = pvalue;
                                    }
                                    else
                                    {
                                        if (flowArgs.TryGetValue(p.Name, out object pv))
                                        {
                                            args[p.Name] = p.GetValue(pv);
                                        }
                                        else
                                        {
                                            args[p.Name] = GetDefaultValue(p);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    AddLog($"函数[{action.FunctionName}] 设置参数值出错[{p.Name}] ：{ex}");
                                    throw;
                                }
                            }
                            #endregion

                            var tryCount = 0;
                            var goNext = false;

                            while (!goNext)
                            {
                                tryCount++;
                                FunctionResult actionResult = MethodServiceFactory.Run(functionName,args);
                                if (action.RunTag.IsNotEmpty())
                                {
                                    runTags.Add(action.RunTag);
                                }
                                #region 处理返回值
                                var resultData = actionResult.Data;
                                if (resultData == null)
                                {
                                    resultData = new Dictionary<string, object>();
                                }
                                LastErrorCode = actionResult.ErrorCode;

                                if (actionResult.IsOK)
                                {
                                    foreach (var r in action.ResultParameters)
                                    {
                                        if (!resultData.TryGetValue(r.Name, out object rvalue))
                                        {
                                            throw new FunctionExecuteException($"算法[{action.FunctionName}]，没有返回值[{r.Name}]", FunctionResult.FlowConfigError);
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
                                            AddLog($"Output {r.Name}: List Count {list.Count}");
                                        }
                                    }
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
            });
        }

        private void AddTrackArgs(Dictionary<string, object> args)
        {
            if (TrackID.IsNotEmpty())
            {
                args.Add("TrackID", TrackID);
            }
        }

        private string OnSetError(DynamicEntity result, FunctionFlowAction action, FunctionResult actionResult)
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

        private static object GetDefaultValue(ParameterItem p)
        {
            return p.GetValue(p.DefaultValue);
        }

        private void AddLog(string line, Category category = Category.Info)
        {
            this.Log($"[{ID}] {line}", category);
        }
    }
}
