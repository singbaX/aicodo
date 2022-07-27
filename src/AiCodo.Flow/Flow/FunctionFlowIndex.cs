// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using Newtonsoft.Json;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace AiCodo.Flow.Configs
{
    [XmlRoot("FlowIndex")]
    public class FunctionFlowIndex : ConfigFile
    {
        #region 属性 Current
        private static FunctionFlowIndex _Current = null;
        private static object _LoadLock = new object();
        public static FunctionFlowIndex Current
        {
            get
            {
                if (_Current == null)
                {
                    lock (_LoadLock)
                    {
                        if (_Current == null)
                        {
                            _Current = CreateOrLoad<FunctionFlowIndex>("FunctionFlowIndex.xml");
                        }
                    }
                }
                return _Current;
            }
        }
        #endregion

        #region 属性 RefItems
        private CollectionBase<FunctionFlowRefFileItem> _RefItems = null;
        [XmlElement("RefItem", typeof(FunctionFlowRefFileItem))]
        public CollectionBase<FunctionFlowRefFileItem> RefItems
        {
            get
            {
                if (_RefItems == null)
                {
                    _RefItems = new CollectionBase<FunctionFlowRefFileItem>();
                    _RefItems.CollectionChanged += RefItems_CollectionChanged;
                }
                return _RefItems;
            }
            set
            {
                if (_RefItems != null)
                {
                    _RefItems.CollectionChanged -= RefItems_CollectionChanged;
                    OnRefItemsRemoved(_RefItems);
                }
                _RefItems = value;
                RaisePropertyChanged("RefItems");
                if (_RefItems != null)
                {
                    _RefItems.CollectionChanged += RefItems_CollectionChanged;
                    OnRefItemsAdded(_RefItems);
                }
            }
        }

        private void RefItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    OnRefItemsAdded(e.NewItems);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    OnRefItemsRemoved(e.OldItems);
                    break;
                default:
                    break;
            }
        }
        protected virtual void OnRefItemsAdded(IList newItems)
        {
            foreach (FunctionFlowRefFileItem item in newItems)
            {
                item.ConfigRoot = this;
            }
        }

        protected virtual void OnRefItemsRemoved(IList oldItems)
        {
            foreach (FunctionFlowRefFileItem item in oldItems)
            {
                item.ConfigRoot = null;
            }
        }
        #endregion

        #region 属性 Items
        private CollectionBase<FunctionFlowFileItem> _Items = null;
        [XmlElement("Item", typeof(FunctionFlowFileItem))]
        public CollectionBase<FunctionFlowFileItem> Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new CollectionBase<FunctionFlowFileItem>();
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
            foreach (FunctionFlowFileItem item in newItems)
            {
                item.ConfigRoot = this;
            }
        }

        protected virtual void OnItemsRemoved(IList oldItems)
        {
            foreach (FunctionFlowFileItem item in oldItems)
            {
                item.ConfigRoot = null;
            }
        }
        #endregion

        public string GetNewRefID()
        {
            var id = 0;
            foreach(var rid in RefItems.Select(r => r.ID).Where(r => r.StartsWith("Ref")).Select(s => s.Substring(3)))
            {
                var mid = rid.ToInt32();
                if (mid > id)
                {
                    id = mid;
                }
            }
            id++;
            return $"Ref{id.ToString("d4")}";
        }

        public string GetNewID()
        {
            var id = 0;
            foreach(var rid in Items.Select(r => r.ID).Where(r => r.StartsWith("F")).Select(s => s.Substring(1)))
            {
                var mid = rid.ToInt32();
                if (mid > id)
                {
                    id = mid;
                }
            }
            id++;
            return $"F{id.ToString("d4")}";
        }
    }

    public class FunctionFlowFileItem : FunctionFlowFileItemBase
    {
        #region 属性 FlowConfig
        private FunctionRefFlowConfig _FlowConfig = null;
        [XmlIgnore,JsonIgnore]
        public FunctionRefFlowConfig FlowConfig
        {
            get
            {
                if(_FlowConfig == null)
                {
                    _FlowConfig = FunctionRefFlowConfig.Load(ID);
                }
                return _FlowConfig;
            }
        }
        #endregion
    }
    public class FunctionFlowRefFileItem : FunctionFlowFileItemBase
    {
        #region 属性 FlowConfig
        private FunctionRefFlowConfig _FlowConfig = null;
        [XmlIgnore,JsonIgnore]
        public FunctionRefFlowConfig FlowConfig
        {
            get
            {
                if(_FlowConfig == null)
                {
                    _FlowConfig = FunctionRefFlowConfig.Load(ID);
                }
                return _FlowConfig;
            }
        }
        #endregion
    }

    public class FunctionFlowFileItemBase : ConfigItemBase
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
        [XmlAttribute("Name"), DefaultValue("")]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name == value)
                {
                    return;
                }
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }
        #endregion

        #region 属性 DisplayName
        private string _DisplayName = string.Empty;
        [XmlAttribute("DisplayName"), DefaultValue("")]
        public string DisplayName
        {
            get
            {
                return _DisplayName;
            }
            set
            {
                if (_DisplayName == value)
                {
                    return;
                }
                _DisplayName = value;
                RaisePropertyChanged("DisplayName");
            }
        }
        #endregion

    }
}
