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
using DynamicExpresso;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace AiCodo.Flow.Configs
{
    public abstract class SubFlowActionBase : FlowActionBase
    {
        protected Dictionary<string, object> CreateSubFlowArgs(Dictionary<string, object> flowArgs, Interpreter exp)
        {
            #region 准备参数
            var args = new Dictionary<string, object>();
            foreach (var p in Parameters)
            {
                object pvalue = null;
                if (p.IsInherit || p.Expression.IsNullOrEmpty())
                {
                    if (flowArgs.TryGetValue(p.Name, out object fvalue))
                    {
                        pvalue = p.GetValue(fvalue);
                    }
                }
                else
                {
                    pvalue = p.GetValue(exp.Eval(p.Expression));
                }
                args[p.Name] = pvalue;
            }
            #endregion
            return args;
        }

        protected void ResetResult(FunctionResult result, DynamicEntity data)
        {
            data.ForEach(p =>
            {
                result.SetValue(p.Key, p.Value);
            });
        }
    }

    public class SubFlowAction : SubFlowActionBase
    {
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
                item.ConfigRoot = this.ConfigRoot;
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

            Dictionary<string, object> subFlowArgs = CreateSubFlowArgs(flowArgs, exp);
            var subContext = new FlowContext(flowArgs);

            try
            {
                var result = subContext.ExecuteFlowActions(flow, Actions);
                var functionResult = new FunctionResult();
                ResetResult(functionResult, result);
                actionResult = functionResult;
            }
            catch (Exception ex)
            {
                ex.WriteErrorLog();
                actionResult = new FunctionResult { ErrorCode = FunctionResult.UnknowError, ErrorMessage = ex.Message };
            }
            return true;
        }
    }

    public class SwitchAction : SubFlowActionBase
    {
        #region 属性 Items
        private CollectionBase<SwitchActionItem> _Items = null;
        [XmlElement("Case", typeof(SwitchActionItem))]
        public CollectionBase<SwitchActionItem> Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new CollectionBase<SwitchActionItem>();
                    _Items.CollectionChanged += Items_CollectionChanged;
                }
                return _Items;
            }
            set
            {
                if (_Items != null)
                {
                    _Items.CollectionChanged -= Items_CollectionChanged;
                    OnItemsRemoved(_Items);
                }
                _Items = value;
                RaisePropertyChanged("Items");
                if (_Items != null)
                {
                    _Items.CollectionChanged += Items_CollectionChanged;
                    OnItemsAdded(_Items);
                }
            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    OnItemsAdded(e.NewItems);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    OnItemsRemoved(e.OldItems);
                    break;
                default:
                    break;
            }
        }
        protected virtual void OnItemsAdded(IList newItems)
        {
            foreach (SwitchActionItem item in newItems)
            {
                item.Switch = this;
                item.ConfigRoot = this.ConfigRoot;
            }
        }

        protected virtual void OnItemsRemoved(IList oldItems)
        {
            foreach (SwitchActionItem item in oldItems)
            {
                item.Switch = null;
                item.ConfigRoot = null;
            }
        }
        #endregion

        #region 属性 DefaultItem
        private SwitchActionItem _DefaultItem = null;
        [XmlElement("Default")]
        public SwitchActionItem DefaultItem
        {
            get
            {
                return _DefaultItem;
            }
            set
            {
                _DefaultItem = value;
                RaisePropertyChanged("DefaultItem");
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

            SwitchActionItem switchItem = Items.FirstOrDefault(f => exp.Eval(f.Condition).ToBoolean());
            if (switchItem == null)
            {
                switchItem = DefaultItem;
            }
            if (switchItem == null)
            {
                actionResult = new FunctionResult { ErrorCode = FunctionResult.UnknowError, ErrorMessage = $"没有符合条件的执行节点" };
                return false;
            }
            else
            {
                Dictionary<string, object> subFlowArgs = CreateSubFlowArgs(flowArgs, exp);
                var subContext = new FlowContext(flowArgs);
                try
                {
                    var result = subContext.ExecuteFlowActions(flow, switchItem.Actions);
                    var functionResult = new FunctionResult();
                    ResetResult(functionResult, result);
                    actionResult = functionResult;
                }
                catch (Exception ex)
                {
                    ex.WriteErrorLog();
                    actionResult = new FunctionResult { ErrorCode = FunctionResult.UnknowError, ErrorMessage = ex.Message };
                }

            }
            return true;
        }
    }

    public class SwitchActionItem : ConfigItemBase
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
                _Condition = value;
                RaisePropertyChanged(() => Condition);
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
                item.ConfigRoot = this.ConfigRoot;
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

        #region 属性 Switch
        private SwitchAction _Switch = null;
        [XmlIgnore, JsonIgnore]
        public SwitchAction Switch
        {
            get
            {
                return _Switch;
            }
            internal set
            {
                _Switch = value;
                RaisePropertyChanged("Switch");
            }
        }
        #endregion
    }

    public class ForEachAction : SubFlowAction
    {
        #region 属性 ItemsSource
        private string _ItemsSource = string.Empty;
        [XmlAttribute("ItemsSource"), DefaultValue("")]
        public string ItemsSource
        {
            get
            {
                return _ItemsSource;
            }
            set
            {
                if (_ItemsSource == value)
                {
                    return;
                }
                _ItemsSource = value;
                RaisePropertyChanged("ItemsSource");
            }
        }
        #endregion

        #region 属性 ItemName
        private string _ItemName = string.Empty;
        [XmlAttribute("ItemName"), DefaultValue("")]
        public string ItemName
        {
            get
            {
                return _ItemName;
            }
            set
            {
                if (_ItemName == value)
                {
                    return;
                }
                _ItemName = value;
                RaisePropertyChanged("ItemName");
            }
        }
        #endregion

        #region 属性 ItemIndexName
        private string _ItemIndexName = string.Empty;
        [XmlAttribute("ItemIndexName"), DefaultValue("")]
        public string ItemIndexName
        {
            get
            {
                return _ItemIndexName;
            }
            set
            {
                if (_ItemIndexName == value)
                {
                    return;
                }
                _ItemIndexName = value;
                RaisePropertyChanged("ItemIndexName");
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

            var items = exp.Eval(ItemsSource);
            if (items != null && items is IEnumerable itemsSource)
            {
                var resultList = new List<DynamicEntity>();
                Dictionary<string, object> subFlowArgs = CreateSubFlowArgs(flowArgs, exp);
                var subContext = new FlowContext(flowArgs);
                var itemName = ItemName.IsNullOrEmpty() ? "Item" : ItemName;
                var itemIndexName = ItemIndexName.IsNullOrEmpty() ? "ItemIndex" : ItemIndexName;
                var index = 0;
                foreach (var source in itemsSource)
                {
                    subContext.SetArgs(itemName, source);
                    subContext.SetArgs(itemIndexName, index);
                    var result = subContext.ExecuteFlowActions(flow, Actions);
                    resultList.Add(result);
                    index++;
                }
                actionResult = new FunctionResult();
                actionResult.SetValue("ResultList", resultList);
            }
            else
            {
                actionResult = new FunctionResult { ErrorCode = FunctionResult.FlowConfigError, ErrorMessage = $"数据源为空或设置错误" };
            }
            return true;
        }
    }

    public class WhileAction : SubFlowAction
    {
        #region 属性 Loop
        private string _Loop = string.Empty;
        [XmlAttribute("Loop"), DefaultValue("")]
        public string Loop
        {
            get
            {
                return _Loop;
            }
            set
            {
                if (_Loop == value)
                {
                    return;
                }
                _Loop = value;
                RaisePropertyChanged("Loop");
            }
        }
        #endregion

        #region 属性 IndexName
        private string _IndexName = string.Empty;
        [XmlAttribute("IndexName"), DefaultValue("")]
        public string IndexName
        {
            get
            {
                return _IndexName;
            }
            set
            {
                if (_IndexName == value)
                {
                    return;
                }
                _IndexName = value;
                RaisePropertyChanged("IndexName");
            }
        }
        #endregion

        #region 属性 SleepSecond
        private int _SleepSecond = 0;
        [XmlAttribute("SleepSecond"), DefaultValue(0)]
        public int SleepSecond
        {
            get
            {
                return _SleepSecond;
            }
            set
            {
                if (_SleepSecond == value)
                {
                    return;
                }
                _SleepSecond = value;
                RaisePropertyChanged("SleepSecond");
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

            if (Loop.IsNullOrEmpty())
            {
                throw new ArgumentException("循环条件没有设置", "Loop");
            }

            var resultList = new List<DynamicEntity>();
            Dictionary<string, object> subFlowArgs = CreateSubFlowArgs(flowArgs, exp);
            var subContext = new FlowContext(flowArgs);
            var indexName = IndexName.IsNotEmpty() ? IndexName : "index";
            var index = 0;
            subFlowArgs[indexName] = index;
            while (Loop.Eval(subFlowArgs).ToBoolean())
            {
                subContext.SetArgs(indexName, index);
                var result = subContext.ExecuteFlowActions(flow, Actions);
                resultList.Add(result);
                if (SleepSecond > 0)
                {
                    Thread.Sleep(SleepSecond * 1000);
                }
                index++;
                subFlowArgs[indexName] = index;
            }
            actionResult = new FunctionResult();
            actionResult.SetValue("ResultList", resultList);
            return true;
        }
    }
}
