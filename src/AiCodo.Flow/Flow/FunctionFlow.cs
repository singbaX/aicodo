// *************************************************************************************************
// 代码文件：FunctionFlow.cs
// 
// 配置文件：FunctionFlow.xml
// 
// 作    者：SingbaX
// 
// 创建日期：202206
// 
// 功能说明：流程配置文件
//
// 修改记录：
// *************************************************************************************************
namespace AiCodo.Flow.Configs
{
    using DynamicExpresso;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Xml.Serialization;

    public partial class FlowInputParameter : ParameterBase
    {
        #region 属性 DefaultValue
        private string _DefaultValue = string.Empty;
        /// <summary>
        /// 默认值
        /// </summary>
        [XmlAttribute("DefaultValue"), DefaultValue("")]
        public string DefaultValue
        {
            get
            {
                return _DefaultValue;
            }
            set
            {
                _DefaultValue = value;
                RaisePropertyChanged("DefaultValue");
            }
        }
        #endregion

        #region 属性 Ref
        private string _Ref = string.Empty;
        [XmlAttribute("Ref"), DefaultValue("")]
        public string Ref
        {
            get
            {
                return _Ref;
            }
            set
            {
                if (_Ref == value)
                {
                    return;
                }
                _Ref = value;
                RaisePropertyChanged("Ref");
            }
        }
        #endregion

        #region 属性 IsRange
        private bool _IsRange = false;
        [XmlAttribute("IsRange"), DefaultValue(false)]
        public bool IsRange
        {
            get
            {
                return _IsRange;
            }
            set
            {
                if (_IsRange == value)
                {
                    return;
                }
                _IsRange = value;
                RaisePropertyChanged("IsRange");
            }
        }
        #endregion
    }

    public partial class FunctionFlowItemRef : ConfigFile
    {
        #region 属性 Name
        private string _Name = string.Empty;
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }
        #endregion

        #region 属性 Parameters
        private CollectionBase<FlowInputParameter> _Parameters = null;
        [XmlArray("Parameters"), XmlArrayItem("Item", typeof(FlowInputParameter))]
        public CollectionBase<FlowInputParameter> Parameters
        {
            get
            {
                if (_Parameters == null)
                {
                    _Parameters = new CollectionBase<FlowInputParameter>();
                }
                return _Parameters;
            }
            set
            {
                _Parameters = value;
                RaisePropertyChanged("Parameters");
            }
        }
        #endregion

        #region 属性 BeforeActions
        private CollectionBase<FunctionFlowAction> _BeforeActions = null;
        [XmlElement("BeforeAction", typeof(FunctionFlowAction))]
        public CollectionBase<FunctionFlowAction> BeforeActions
        {
            get
            {
                if (_BeforeActions == null)
                {
                    _BeforeActions = new CollectionBase<FunctionFlowAction>();
                }
                return _BeforeActions;
            }
            set
            {
                _BeforeActions = value;
                RaisePropertyChanged("BeforeActions");
            }
        }
        #endregion

        #region 属性 AfterActions
        private CollectionBase<FunctionFlowAction> _AfterActions = null;
        [XmlElement("AfterAction", typeof(FunctionFlowAction))]
        public CollectionBase<FunctionFlowAction> AfterActions
        {
            get
            {
                if (_AfterActions == null)
                {
                    _AfterActions = new CollectionBase<FunctionFlowAction>();
                }
                return _AfterActions;
            }
            set
            {
                _AfterActions = value;
                RaisePropertyChanged("AfterActions");
            }
        }
        #endregion
    }

    public partial class FunctionFlowItem : FlowItemBase
    {
        #region 属性 RefItem
        private string _RefItem = string.Empty;
        [XmlAttribute("RefItem"), DefaultValue("")]
        public string RefItem
        {
            get
            {
                return _RefItem;
            }
            set
            {
                if (_RefItem == value)
                {
                    return;
                }
                _RefItem = value;
                RaisePropertyChanged("RefItem");
            }
        }
        #endregion

        #region 属性 Parameters
        private CollectionBase<FlowInputParameter> _Parameters = null;
        [XmlArray("Parameters"), XmlArrayItem("Item", typeof(FlowInputParameter))]
        public CollectionBase<FlowInputParameter> Parameters
        {
            get
            {
                if (_Parameters == null)
                {
                    _Parameters = new CollectionBase<FlowInputParameter>();
                    _Parameters.CollectionChanged += Parameters_CollectionChanged;
                }
                return _Parameters;
            }
            set
            {
                if (_Parameters != null)
                {
                    _Parameters.CollectionChanged -= Parameters_CollectionChanged;
                    OnParametersRemoved(_Parameters);
                }
                _Parameters = value;
                RaisePropertyChanged("Parameters");
                if (_Parameters != null)
                {
                    _Parameters.CollectionChanged += Parameters_CollectionChanged;
                    OnParametersAdded(_Parameters);
                }
            }
        }

        private void Parameters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    OnParametersAdded(e.NewItems);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    OnParametersRemoved(e.OldItems);
                    break;
                default:
                    break;
            }
        }
        protected virtual void OnParametersAdded(IList newItems)
        {
            foreach (FlowInputParameter item in newItems)
            {
                item.ConfigRoot = this;
            }
        }

        protected virtual void OnParametersRemoved(IList oldItems)
        {
            foreach (FlowInputParameter item in oldItems)
            {
                item.ConfigRoot = null;
            }
        }
        #endregion

        #region 属性 Actions
        private CollectionBase<FlowActionBase> _Actions = null;
        [XmlElement("Action", typeof(FunctionFlowAction))]
        [XmlElement("Flow", typeof(SubFlowAction))]
        [XmlElement("Switch", typeof(SwitchAction))]
        [XmlElement("ForEach", typeof(ForEachAction))]
        [XmlElement("While", typeof(WhileAction))]
        public CollectionBase<FlowActionBase> Actions
        {
            get
            {
                if (_Actions == null)
                {
                    _Actions = new CollectionBase<FlowActionBase>();
                    _Actions.CollectionChanged += Actions_CollectionChanged;
                }
                return _Actions;
            }
            set
            {
                if (_Actions != null)
                {
                    _Actions.CollectionChanged -= Actions_CollectionChanged;
                    OnActionsRemoved(_Actions);
                }
                _Actions = value;
                RaisePropertyChanged("Actions");
                if (_Actions != null)
                {
                    _Actions.CollectionChanged += Actions_CollectionChanged;
                    OnActionsAdded(_Actions);
                }
            }
        }

        private void Actions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    OnActionsAdded(e.NewItems);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    OnActionsRemoved(e.OldItems);
                    break;
                default:
                    break;
            }
        }
        protected virtual void OnActionsAdded(IList newItems)
        {
            foreach (FlowActionBase item in newItems)
            {
                item.ConfigRoot = this;
            }
        }

        protected virtual void OnActionsRemoved(IList oldItems)
        {
            foreach (FlowActionBase item in oldItems)
            {
                item.ConfigRoot = null;
            }
        }
        #endregion

        #region 属性 RefFlowItems
        public virtual IEnumerable<FunctionFlowItemRef> GetRefFlowItems()
        {
            yield break;
        }
        #endregion

        #region 属性 Results
        private CollectionBase<FlowResultParameter> _Results = null;
        [XmlArray("Results"), XmlArrayItem("Item", typeof(FlowResultParameter))]
        public CollectionBase<FlowResultParameter> Results
        {
            get
            {
                if (_Results == null)
                {
                    _Results = new CollectionBase<FlowResultParameter>();
                    _Results.CollectionChanged += Results_CollectionChanged;
                }
                return _Results;
            }
            set
            {
                if (_Results != null)
                {
                    _Results.CollectionChanged -= Results_CollectionChanged;
                    OnResultsRemoved(_Results);
                }
                _Results = value;
                RaisePropertyChanged("Results");
                if (_Results != null)
                {
                    _Results.CollectionChanged += Results_CollectionChanged;
                    OnResultsAdded(_Results);
                }
            }
        }

        private void Results_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    OnResultsAdded(e.NewItems);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    OnResultsRemoved(e.OldItems);
                    break;
                default:
                    break;
            }
        }
        protected virtual void OnResultsAdded(IList newItems)
        {
            foreach (FlowResultParameter item in newItems)
            {
                item.ConfigRoot = this;
            }
        }

        protected virtual void OnResultsRemoved(IList oldItems)
        {
            foreach (FlowResultParameter item in oldItems)
            {
                item.ConfigRoot = null;
            }
        }
        #endregion

        public string GetNextID()
        {
            if (Actions.Count > 0)
            {
                var id = 0;
                foreach (var action in Actions)
                {
                    if (action.ID.IsNotEmpty() && action.ID.StartsWith("A"))
                    {
                        var num = action.ID.Substring(1).ToInt32();
                        if (num > id)
                        {
                            id = num;
                        }
                    }
                }
                return $"A{(id + 1).ToString("d2")}";
            }

            return $"A01";
        }

        public virtual IEnumerable<FlowInputParameter> GetParameters()
        {
            foreach (var refItem in GetRefFlowItems())
            {
                foreach (var p in refItem.Parameters)
                {
                    yield return p;
                }
            }
            foreach (var p in Parameters)
            {
                yield return p;
            }
        }

        public virtual IEnumerable<FlowActionBase> GetActions()
        {
            foreach (var refItem in GetRefFlowItems())
            {
                foreach (var action in refItem.BeforeActions)
                {
                    yield return action;
                }
            }

            foreach (var action in Actions)
            {
                yield return action;
            }

            foreach (var refItem in GetRefFlowItems())
            {
                foreach (var action in refItem.AfterActions)
                {
                    yield return action;
                }
            }
        }
    }

    public partial class FlowItemBase : ConfigFile
    {
        #region 属性 ID
        private string _ID = string.Empty;
        [XmlAttribute("ID")]
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
                RaisePropertyChanged("ID");
            }
        }
        #endregion

        #region 属性 Name
        private string _Name = string.Empty;
        /// <summary>
        /// 算法名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }
        #endregion

        #region 属性 ErrorMode
        private FlowErrorMode _ErrorMode = FlowErrorMode.Break;
        [XmlAttribute("ErrorMode"), DefaultValue(typeof(FlowErrorMode), "Break")]
        public FlowErrorMode ErrorMode
        {
            get
            {
                return _ErrorMode;
            }
            set
            {
                _ErrorMode = value;
                RaisePropertyChanged("ErrorMode");
            }
        }
        #endregion

        #region 属性 RetryCount
        private int _RetryCount = 0;
        [XmlAttribute("RetryCount"), DefaultValue(0)]
        public int RetryCount
        {
            get
            {
                return _RetryCount;
            }
            set
            {
                if (_RetryCount == value)
                {
                    return;
                }
                _RetryCount = value;
                RaisePropertyChanged("RetryCount");
            }
        }
        #endregion

        #region 属性 Description
        private string _Description = string.Empty;
        [XmlElement("Description")]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
                RaisePropertyChanged("Description");
            }
        }
        #endregion
    }

    [Flags]
    public enum FlowErrorMode
    {
        /// <summary>
        /// 重试
        /// </summary>
        Retry = 1,
        /// <summary>
        /// 继续
        /// </summary>
        Continue = 2,
        /// <summary>
        /// 中断
        /// </summary>
        Break = 4,
        /// <summary>
        /// 重试后仍然出错继续
        /// </summary>
        RetryContinue = 3,
        /// <summary>
        /// 重试后仍然出错退出
        /// </summary>
        RetryBreak = 5
    }

    public partial class FunctionAssert : ConfigItemBase
    {
        #region 属性 Condition
        private string _Condition = string.Empty;
        [XmlAttribute("Condition"), DefaultValue("")]
        public string Condition
        {
            get
            {
                return _Condition;
            }
            set
            {
                if (_Condition == value)
                {
                    return;
                }
                _Condition = value;
                RaisePropertyChanged("Condition");
            }
        }
        #endregion

        #region 属性 Error
        private string _Error = string.Empty;
        [XmlAttribute("Error"), DefaultValue("")]
        public string Error
        {
            get
            {
                return _Error;
            }
            set
            {
                if (_Error == value)
                {
                    return;
                }
                _Error = value;
                RaisePropertyChanged("Error");
            }
        }
        #endregion
    }

    public class FunctionActionWaitItem : ConfigItemBase
    {
        #region 属性 Condition
        private string _Condition = string.Empty;
        [XmlElement("Condition"), DefaultValue("")]
        public string Condition
        {
            get
            {
                return _Condition;
            }
            set
            {
                _Condition = value;
                RaisePropertyChanged("Condition");
            }
        }
        #endregion

        #region 属性 CheckMS
        private int _CheckMS = 0;
        [XmlAttribute("CheckMS"), DefaultValue(0)]
        public int CheckMS
        {
            get
            {
                return _CheckMS;
            }
            set
            {
                if (_CheckMS == value)
                {
                    return;
                }
                _CheckMS = value;
                RaisePropertyChanged("CheckMS");
            }
        }
        #endregion

        #region 属性 MaxCount
        private int _MaxCount = 0;
        [XmlAttribute("MaxCount"), DefaultValue(0)]
        public int MaxCount
        {
            get
            {
                return _MaxCount;
            }
            set
            {
                if (_MaxCount == value)
                {
                    return;
                }
                _MaxCount = value;
                RaisePropertyChanged("MaxCount");
            }
        }
        #endregion
    }

    public partial class FlowActionBase : ConfigItemBase
    {
        #region 属性 ID
        private string _ID = string.Empty;
        [XmlAttribute("ID"), DefaultValue("")]
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (_ID == value)
                {
                    return;
                }
                _ID = value;
                RaisePropertyChanged("ID");
            }
        }
        #endregion

        #region 属性 Name
        private string _Name = string.Empty;
        /// <summary>
        /// 算法名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }
        #endregion

        #region 属性 IgnoreErrors
        private string _IgnoreErrors = string.Empty;
        [XmlAttribute("IgnoreErrors"), DefaultValue("")]
        public string IgnoreErrors
        {
            get
            {
                return _IgnoreErrors;
            }
            set
            {
                if (_IgnoreErrors == value)
                {
                    return;
                }
                _IgnoreErrors = value;
                RaisePropertyChanged("IgnoreErrors");
            }
        }
        #endregion

        #region 属性 Wait
        private FunctionActionWaitItem _Wait = null;
        [XmlElement("Wait")]
        public FunctionActionWaitItem Wait
        {
            get
            {
                return _Wait;
            }
            set
            {
                _Wait = value;
                RaisePropertyChanged("Wait");
            }
        }
        #endregion

        #region 属性 Condition
        private string _Condition = string.Empty;
        [XmlElement("Condition"), DefaultValue("")]
        public string Condition
        {
            get
            {
                return _Condition;
            }
            set
            {
                _Condition = value;
                RaisePropertyChanged("Condition");
            }
        }
        #endregion

        #region 属性 Asserts
        private CollectionBase<FunctionAssert> _Asserts = null;
        [XmlElement("Assert")]
        public CollectionBase<FunctionAssert> Asserts
        {
            get
            {
                if (_Asserts == null)
                {
                    _Asserts = new CollectionBase<FunctionAssert>();
                }
                return _Asserts;
            }
            set
            {
                _Asserts = value;
                RaisePropertyChanged("Asserts");
            }
        }
        #endregion

        #region 属性 Parameters
        private CollectionBase<ActionInputParameter> _Parameters = null;
        [XmlElement("Input", typeof(ActionInputParameter))]
        public CollectionBase<ActionInputParameter> Parameters
        {
            get
            {
                if (_Parameters == null)
                {
                    _Parameters = new CollectionBase<ActionInputParameter>();
                    _Parameters.CollectionChanged += Parameters_CollectionChanged;
                }
                return _Parameters;
            }
            set
            {
                if (_Parameters != null)
                {
                    _Parameters.CollectionChanged -= Parameters_CollectionChanged;
                    OnParametersRemoved(_Parameters);
                }
                _Parameters = value;
                RaisePropertyChanged("Parameters");
                if (_Parameters != null)
                {
                    _Parameters.CollectionChanged += Parameters_CollectionChanged;
                    OnParametersAdded(_Parameters);
                }
            }
        }

        private void Parameters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    OnParametersAdded(e.NewItems);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    OnParametersRemoved(e.OldItems);
                    break;
                default:
                    break;
            }
        }
        protected virtual void OnParametersAdded(IList newItems)
        {
            foreach (ActionInputParameter item in newItems)
            {
                item.FlowAction = this;
                item.ConfigRoot = this.ConfigRoot;
            }
        }

        protected virtual void OnParametersRemoved(IList oldItems)
        {
            foreach (ActionInputParameter item in oldItems)
            {
                item.FlowAction = null;
                item.ConfigRoot = null;
            }
        }
        #endregion

        #region 属性 ResultParameters
        private CollectionBase<ActionOutputParameter> _ResultParameters = null;
        [XmlElement("Output", typeof(ActionOutputParameter))]
        public CollectionBase<ActionOutputParameter> ResultParameters
        {
            get
            {
                if (_ResultParameters == null)
                {
                    _ResultParameters = new CollectionBase<ActionOutputParameter>();
                    _ResultParameters.CollectionChanged += ResultParameters_CollectionChanged;
                }
                return _ResultParameters;
            }
            set
            {
                if (_ResultParameters != null)
                {
                    _ResultParameters.CollectionChanged -= ResultParameters_CollectionChanged;
                    OnResultParametersRemoved(_ResultParameters);
                }
                _ResultParameters = value;
                RaisePropertyChanged("ResultParameters");
                if (_ResultParameters != null)
                {
                    _ResultParameters.CollectionChanged += ResultParameters_CollectionChanged;
                    OnResultParametersAdded(_ResultParameters);
                }
            }
        }

        private void ResultParameters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    OnResultParametersAdded(e.NewItems);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    OnResultParametersRemoved(e.OldItems);
                    break;
                default:
                    break;
            }
        }
        protected virtual void OnResultParametersAdded(IList newItems)
        {
            foreach (ActionOutputParameter item in newItems)
            {
                item.FlowAction = this;
                item.ConfigRoot = this.ConfigRoot;
            }
        }

        protected virtual void OnResultParametersRemoved(IList oldItems)
        {
            foreach (ActionOutputParameter item in oldItems)
            {
                item.FlowAction = null;
                item.ConfigRoot = null;
            }
        }
        #endregion

        protected override void OnConfigRootChanged()
        {
            base.OnConfigRootChanged();
            Parameters.ForEach(p => p.ConfigRoot = this.ConfigRoot);
            ResultParameters.ForEach(p => p.ConfigRoot = this.ConfigRoot);
        }

        public virtual bool TryRun(FunctionFlowItem flow, Dictionary<string, object> flowArgs, out IFunctionResult actionResult)
        {
            actionResult = null;
            return false;
        }

        protected Dictionary<string, object> CreateFunctionArgs(Dictionary<string, object> flowArgs, Interpreter exp, IFunctionItem algItem)
        {
            #region 准备参数
            var args = new Dictionary<string, object>();
            foreach (var p in algItem.GetParameters())
            {
                try
                {
                    object pvalue = null;
                    var actionParameter = Parameters.FirstOrDefault(f => f.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase));
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
                            args[p.Name] = p.GetValue(p.DefaultValue);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Log($"函数[{Name}] 设置参数值出错[{p.Name}] ：{ex}");
                    throw;
                }
            }
            #endregion
            return args;
        }

        protected void CheckAssert(FunctionFlowItem flow, Interpreter exp)
        {
            if (Asserts.Count > 0)
            {
                foreach (var assert in Asserts)
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
                        throw new FunctionExecuteException($"流程[{flow.Name}]节点[{Name}] Assert异常：{assert.Error}", FunctionResult.AssertError);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        protected void CheckWait(Dictionary<string, object> flowArgs)
        {
            #region wait
            if (Wait != null && Wait.Condition.IsNotEmpty())
            {
                var maxCount = Wait.MaxCount > 0 ? Wait.MaxCount : 0;
                var checkMS = Wait.CheckMS > 0 ? Wait.CheckMS : 100;
                var checkCount = 0;
                while (!Wait.Condition.Eval(flowArgs).ToBoolean())
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
            #endregion
        }
    }

    public partial class FunctionFlowAction : FlowActionBase
    {
        #region 属性 FunctionName
        private string _FunctionName = string.Empty;
        [XmlAttribute("FunctionName")]
        public string FunctionName
        {
            get
            {
                return _FunctionName;
            }
            set
            {
                _FunctionName = value;
                RaisePropertyChanged("FunctionName");
                RaisePropertyChanged("FunctionItem");
            }
        }
        #endregion

        #region 属性 FunctionItem
        [XmlIgnore]
        public FunctionItemBase FunctionItem
        {
            get
            {
                if (FunctionName.IsNullOrEmpty())
                {
                    return null;
                }
                var item = FunctionConfig.Current.GetItem(FunctionName);
                return item;
            }
        }
        #endregion

        public override bool TryRun(FunctionFlowItem flow, Dictionary<string, object> flowArgs, out IFunctionResult actionResult)
        {
            var exp = ExpressionHelper.GetInterpreter(flowArgs);
            actionResult = null;

            CheckWait(flowArgs);
            if (Condition.IsNotEmpty())
            {
                if (exp.Eval(Condition).ToBoolean())
                {
                    this.Log($"{Name} 不满足执行条件，执行跳过");
                    return false;
                }
            }

            CheckAssert(flow, exp);

            if (FunctionName.IsNullOrEmpty())
            {
                throw new FunctionExecuteException($"流程[{flow.Name}]节点[{Name}]算法没有设置", FunctionResult.FlowConfigError);
            }

            var functionName = FunctionName;
            var algItem = MethodServiceFactory.GetItem(functionName);
            if (algItem == null)
            {
                throw new FunctionExecuteException($"算法[{FunctionName}]不存在", FunctionResult.NotFound);
            }

            Dictionary<string, object> args = CreateFunctionArgs(flowArgs, exp, algItem);
            try
            {
                actionResult = MethodServiceFactory.Run(functionName, args);
            }
            catch (Exception ex)
            {
                ex.WriteErrorLog();
                actionResult = new FunctionResult { ErrorCode = FunctionResult.UnknowError, ErrorMessage = ex.Message };
            }
            return true;
        }
    }

    public partial class ActionInputParameter : ParameterBase
    {
        #region 属性 Expression
        private string _Expression = string.Empty;
        [XmlAttribute("Expression")]
        public string Expression
        {
            get
            {
                return _Expression;
            }
            set
            {
                _Expression = value;
                RaisePropertyChanged("Expression");
            }
        }
        #endregion 

        #region 属性 IsInherit
        private bool _IsInherit = true;
        [XmlAttribute("IsInherit"), DefaultValue(true)]
        public bool IsInherit
        {
            get
            {
                return _IsInherit;
            }
            set
            {
                if (_IsInherit == value)
                {
                    return;
                }
                _IsInherit = value;
                RaisePropertyChanged("IsInherit");
            }
        }
        #endregion

        #region 属性 FlowAction
        private FlowActionBase _FlowAction = null;
        [XmlIgnore]
        public FlowActionBase FlowAction
        {
            get
            {
                return _FlowAction;
            }
            internal set
            {
                _FlowAction = value;
                RaisePropertyChanged("FlowAction");
            }
        }
        #endregion
    }

    public partial class FlowResultParameter : ParameterBase
    {
        #region 属性 Expression
        private string _Expression = string.Empty;
        [XmlAttribute("Expression")]
        public string Expression
        {
            get
            {
                return _Expression;
            }
            set
            {
                _Expression = value;
                RaisePropertyChanged("Expression");
            }
        }
        #endregion
    }


    public partial class ActionOutputParameter : ParameterBase
    {
        #region 属性 ResetInputName
        private string _ResetInputName = string.Empty;
        [XmlAttribute("ResetInputName"), DefaultValue("")]
        public string ResetInputName
        {
            get
            {
                return _ResetInputName;
            }
            set
            {
                _ResetInputName = value;
                RaisePropertyChanged("ResetInputName");
            }
        }
        #endregion

        #region 属性 ResultName
        private string _ResultName = string.Empty;
        [XmlAttribute("ResultName"), DefaultValue("")]
        public string ResultName
        {
            get
            {
                return _ResultName;
            }
            set
            {
                if (_ResultName == value)
                {
                    return;
                }
                _ResultName = value;
                RaisePropertyChanged("ResultName");
            }
        }
        #endregion

        #region 属性 FlowAction
        private FlowActionBase _FlowAction = null;
        [XmlIgnore]
        public FlowActionBase FlowAction
        {
            get
            {
                return _FlowAction;
            }
            internal set
            {
                _FlowAction = value;
                RaisePropertyChanged("FlowAction");
            }
        }
        #endregion
    }

    public partial class FlowParameter : ParameterBase
    {
        #region 属性 Value
        private string _Value = string.Empty;
        [XmlAttribute("Value")]
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                RaisePropertyChanged("Value");
            }
        }
        #endregion
    }

    public static class FunctionResultHelper
    {
        public static bool IsOK(this IFunctionResult result)
        {
            return (result.ErrorCode.IsNullOrEmpty() || result.ErrorCode == FunctionResult.OK) && result.ErrorMessage.IsNullOrEmpty();
        }
    }

    /// <summary>
    /// 算法统一返回的结果
    /// </summary>
    public class FunctionResult : IFunctionResult
    {
        public const string OK = "0";

        //方法不存在
        public const string NotFound = "1";

        //其它未知错误
        public const string UnknowError = "2";

        //断言错误
        public const string AssertError = "3";

        //在执行函数内部发送的错误
        public const string MethodInnerError = "4";

        //流程配置错误
        public const string FlowConfigError = "10";

        //针对某个具体算法，可以定义多个错误码
        public string ErrorCode { get; set; } = "0";

        public string ErrorMessage { get; set; } = "";

        //返回值，没有则保留默认值
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        public static bool IsOKCode(string errorCode)
        {
            return errorCode.IsNullOrEmpty() || errorCode == OK;
        }

        public FunctionResult()
        {

        }

        public FunctionResult(Dictionary<string, object> data)
        {
            if (data != null)
            {
                data.ForEach(d => Data[d.Key] = d.Value);
            }
        }

        public bool TryGetValue(string name, out object value)
        {
            return Data.TryGetValue(name, out value);
        }

        public void SetValue(string name, object value)
        {
            Data[name] = value;
        }

        public FunctionResult SetData(string name, object value)
        {
            SetValue(name, value);
            return this;
        }
    }

    public class FunctionExecuteException : Exception
    {
        public string ErrorCode { get; set; } = FunctionResult.UnknowError;

        public FunctionExecuteException(string message) : base(message)
        {

        }
        public FunctionExecuteException(string message, string code) : base(message)
        {
            ErrorCode = code;
        }
    }

    /// <summary>
    /// 异常
    /// </summary>
    public class MethodNotFoundException : Exception
    {
        public string MethodName { get; set; }
    }

}
