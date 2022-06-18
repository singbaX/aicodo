﻿// *************************************************************************************************
// 代码文件：FunctionConfig.cs
// 
// 配置文件：FunctionConfig.xml
// 
// 作    者：SingbaX
// 
// 创建日期：202206
// 
// 功能说明：算法定义配置文件的对应
//
// 修改记录：
// *************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace AiCodo.Flow.Configs
{
    [XmlRoot("FunctionConfig")]
    public partial class FunctionConfig : ConfigFile
    {
        const string _ConfigFile = "FunctionConfig.xml";

        static readonly List<ParameterTypeDefine> _StaticTypes = new List<ParameterTypeDefine>
        {
            new ParameterTypeDefine { Name = "Bool",    Type = ParameterType.Bool },
            new ParameterTypeDefine { Name = "Int",     Type = ParameterType.Int },
            new ParameterTypeDefine { Name = "Long",    Type = ParameterType.Long },
            new ParameterTypeDefine { Name = "Single",  Type = ParameterType.Single },
            new ParameterTypeDefine { Name = "Double",  Type = ParameterType.Double },
            new ParameterTypeDefine { Name = "String",  Type = ParameterType.String },
            new ParameterTypeDefine { Name = "FilePath",Type = ParameterType.FilePath },
            new ParameterTypeDefine { Name = "Image",   Type = ParameterType.Image },
            new ParameterTypeDefine { Name = "Object",  Type = ParameterType.Object },
            new ParameterTypeDefine { Name = "List",    Type = ParameterType.List },
        };

        static object _LoadLock = new object();

        public static void Reload()
        {
            var config = _ConfigFile.LoadXDoc<FunctionConfig>();
            if (config != null)
            {
                Current = config;
            }
            else
            {
                Current = new FunctionConfig();
            }
        }

        #region 属性 ImageFunctionVersion
        private string _ImageFunctionVersion = string.Empty;
        [XmlAttribute("ImageFunctionVersion"), DefaultValue("")]
        public string ImageFunctionVersion
        {
            get
            {
                return _ImageFunctionVersion;
            }
            set
            {
                if (_ImageFunctionVersion == value)
                {
                    return;
                }
                _ImageFunctionVersion = value;
                RaisePropertyChanged("ImageFunctionVersion");
            }
        }
        #endregion

        #region 属性 ConfigFileName
        private string _ConfigFileName = string.Empty;
        [XmlIgnore]
        public string ConfigFileName
        {
            get
            {
                if (_Current == null)
                {
                    lock (_LoadLock)
                    {
                        if (_Current == null)
                        {
                            Reload();
                        }
                    }
                }
                return _ConfigFileName;
            }
            set
            {
                _ConfigFileName = value;
                RaisePropertyChanged("ConfigFileName");
            }
        }
        #endregion

        #region 属性 CommonMethods
        private CollectionBase<MethodItem> _CommonMethods = null;
        [XmlArray("CommonMethods"), XmlArrayItem("Method", typeof(MethodItem))]
        public CollectionBase<MethodItem> CommonMethods
        {
            get
            {
                if (_CommonMethods == null)
                {
                    _CommonMethods = new CollectionBase<MethodItem>();
                    _CommonMethods.CollectionChanged += CommonMethods_CollectionChanged;
                }
                return _CommonMethods;
            }
            set
            {
                if (_CommonMethods != null)
                {
                    _CommonMethods.CollectionChanged -= CommonMethods_CollectionChanged;
                    OnCommonMethodsRemoved(_CommonMethods);
                }
                _CommonMethods = value;
                RaisePropertyChanged("CommonMethods");
                if (_CommonMethods != null)
                {
                    _CommonMethods.CollectionChanged += CommonMethods_CollectionChanged;
                    OnCommonMethodsAdded(_CommonMethods);
                }
            }
        }

        private void CommonMethods_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    OnCommonMethodsAdded(e.NewItems);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    OnCommonMethodsRemoved(e.OldItems);
                    break;
                default:
                    break;
            }
        }
        protected virtual void OnCommonMethodsAdded(IList newItems)
        {
            foreach (MethodItem item in newItems)
            {
                item.ConfigRoot = this;
            }
        }

        protected virtual void OnCommonMethodsRemoved(IList oldItems)
        {
            foreach (MethodItem item in oldItems)
            {
                item.ConfigRoot = null;
            }
        }
        #endregion

        #region 属性 Items
        private CollectionBase<FunctionItemConfig> _Items = null;
        [XmlElement("Item", typeof(FunctionItemConfig))]
        public CollectionBase<FunctionItemConfig> Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new CollectionBase<FunctionItemConfig>();
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
            foreach (FunctionItemBase item in newItems)
            {
                item.ConfigRoot = this;
            }
        }

        protected virtual void OnItemsRemoved(IList oldItems)
        {
            foreach (FunctionItemBase item in oldItems)
            {
                item.ConfigRoot = null;
            }
        }
        #endregion

        #region 属性 Types
        private CollectionBase<OptionType> _Types = null;
        [XmlArray("Types")]
        [XmlArrayItem("Option", typeof(OptionType))]
        public CollectionBase<OptionType> Types
        {
            get
            {
                if (_Types == null)
                {
                    _Types = new CollectionBase<OptionType>();
                    _Types.CollectionChanged += Types_CollectionChanged;
                }
                return _Types;
            }
            set
            {
                if (_Types != null)
                {
                    _Types.CollectionChanged -= Types_CollectionChanged;
                    OnTypesRemoved(_Types);
                }
                _Types = value;
                RaisePropertyChanged("Types");
                if (_Types != null)
                {
                    _Types.CollectionChanged += Types_CollectionChanged;
                    OnTypesAdded(_Types);
                }
                RaisePropertyChanged("AllTypes");
            }
        }

        private void Types_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    OnTypesAdded(e.NewItems);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    OnTypesRemoved(e.OldItems);
                    break;
                default:
                    break;
            }
            RaisePropertyChanged("AllTypes");
        }
        protected virtual void OnTypesAdded(IList newItems)
        {
            foreach (OptionType item in newItems)
            {
                item.ConfigRoot = this;
            }
        }

        protected virtual void OnTypesRemoved(IList oldItems)
        {
            foreach (OptionType item in oldItems)
            {
                item.ConfigRoot = null;
            }
        }
        #endregion

        #region 属性 AllNumberTypes 
        public IEnumerable<ParameterTypeDefine> AllNumberTypes
        {
            get
            {
                return _StaticTypes.Where(t => t.Type == ParameterType.Int
                    || t.Type == ParameterType.Double
                    || t.Type == ParameterType.Long);
            }
        }
        #endregion

        #region 属性 AllTypes
        [XmlIgnore]
        public IEnumerable<ParameterTypeDefine> AllTypes
        {
            get
            {
                return GetAllTypes();
            }
        }

        private IEnumerable<ParameterTypeDefine> GetAllTypes()
        {
            foreach (var item in _StaticTypes)
            {
                yield return item;
            }

            foreach (var item in Types)
            {
                yield return new ParameterTypeDefine
                {
                    Name = item.Name,
                    Type = ParameterType.Option,
                    OptionType = item
                };
            }
        }
        #endregion

        #region 属性 Current
        private static FunctionConfig _Current = null;
        public static FunctionConfig Current
        {
            get
            {
                return _Current;
            }
            set
            {
                _Current = value;
            }
        }
        #endregion

        public FunctionItemBase GetItem(string name)
        {
            return Items.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public ParameterTypeDefine GetType(string name)
        {
            return AllTypes.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }

    public class ParameterTypeDefine : ConfigItemBase
    {
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
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }
        #endregion

        #region 属性 Type
        private ParameterType _Type = ParameterType.String;
        public ParameterType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
                RaisePropertyChanged("Type");
            }
        }
        #endregion

        #region 属性 OptionType
        private OptionType _OptionType = null;
        public OptionType OptionType
        {
            get
            {
                return _OptionType;
            }
            set
            {
                _OptionType = value;
                RaisePropertyChanged("OptionType");
            }
        }
        #endregion
    }

    /// <summary>
    /// 算法类型，每种算法返回值类型相同
    /// </summary>
    public enum FunctionType
    {
        ImageMerge, //图片合并
        DropEdge,   //去边界
        CountCell,  //计数
        WellImage, //整孔图片处理
        Tool,//工具类
    }

    public partial class FunctionItemBase : ConfigItemBase, IFunctionItem
    {
        #region 属性 Name
        private string _Name = string.Empty;
        /// <summary>
        /// 算法名称（一旦使用不能修改）
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

        #region 属性 DisplayName
        private string _DisplayName = string.Empty;
        /// <summary>
        /// 显示名称（界面显示名称）
        /// </summary>
        [XmlAttribute("DisplayName")]
        public string DisplayName
        {
            get
            {
                return _DisplayName;
            }
            set
            {
                _DisplayName = value;
                RaisePropertyChanged("DisplayName");
            }
        }
        #endregion

        #region 属性 Parameters
        private CollectionBase<ParameterItem> _Parameters = null;
        [XmlElement("Input", typeof(ParameterItem))]
        public CollectionBase<ParameterItem> Parameters
        {
            get
            {
                if (_Parameters == null)
                {
                    _Parameters = new CollectionBase<ParameterItem>();
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
            foreach (ParameterItem item in newItems)
            {
                item.ConfigRoot = this.ConfigRoot;
            }
        }

        protected virtual void OnParametersRemoved(IList oldItems)
        {
            foreach (ParameterItem item in oldItems)
            {
                item.ConfigRoot = null;
            }
        }
        #endregion

        #region 属性 ResultParameters
        private CollectionBase<ResultParameter> _ResultParameters = null;
        [XmlElement("Output", typeof(ResultParameter))]
        public CollectionBase<ResultParameter> ResultParameters
        {
            get
            {
                if (_ResultParameters == null)
                {
                    _ResultParameters = new CollectionBase<ResultParameter>();
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
            foreach (ResultParameter item in newItems)
            {
                item.ConfigRoot = this.ConfigRoot;
            }
        }

        protected virtual void OnResultParametersRemoved(IList oldItems)
        {
            foreach (ResultParameter item in oldItems)
            {
                item.ConfigRoot = null;
            }
        }
        #endregion

        #region 属性 Tags
        private string _Tags = string.Empty;
        [XmlAttribute("Tags"), DefaultValue("")]
        public string Tags
        {
            get
            {
                return _Tags;
            }
            set
            {
                _Tags = value;
                RaisePropertyChanged("Tags");
            }
        }
        #endregion

        #region 属性 IsCmd
        private bool _IsCmd = false;
        [XmlIgnore]
        public bool IsCmd
        {
            get
            {
                return _IsCmd;
            }
            protected set
            {
                _IsCmd = value;
                RaisePropertyChanged("IsCmd");
            }
        }
        #endregion

        protected override void OnConfigRootChanged()
        {
            base.OnConfigRootChanged();
            Parameters.ForEach(p => p.ConfigRoot = this.ConfigRoot);
            ResultParameters.ForEach(p => p.ConfigRoot = this.ConfigRoot);
        }

        public IEnumerable<ParameterItem> GetParameters()
        {
            return Parameters.ToList();
        }

        public IEnumerable<ResultParameter> GetResultParameter()
        {
            return ResultParameters.ToList();
        }
    }
    public partial class FunctionItemConfig : FunctionItemBase
    {
        #region 属性 Location
        private FunctionItemLocation _Location = null;
        [XmlElement("Location")]
        public FunctionItemLocation Location
        {
            get
            {
                if (_Location == null)
                {
                    _Location = new FunctionItemLocation
                    {
                        ConfigRoot = this.ConfigRoot
                    };
                }
                return _Location;
            }
            set
            {
                if (_Location != null)
                {
                    _Location.ConfigRoot = null;
                }
                _Location = value;
                RaisePropertyChanged("Location");
                if (_Location != null)
                {
                    _Location.ConfigRoot = this.ConfigRoot;
                }
            }
        }
        #endregion

        protected override void OnConfigRootChanged()
        {
            base.OnConfigRootChanged();
            Location.ConfigRoot = this.ConfigRoot;
        }
    }

    public partial class FunctionItemLocation : ConfigItemBase
    {
        #region 属性 AsmName
        private string _AsmName = string.Empty;
        [XmlAttribute("AsmName"), DefaultValue("")]
        public string AsmName
        {
            get
            {
                return _AsmName;
            }
            set
            {
                if (_AsmName == value)
                {
                    return;
                }
                _AsmName = value;
                RaisePropertyChanged("AsmName");
            }
        }
        #endregion

        #region 属性 FileName
        private string _FileName = string.Empty;
        [XmlAttribute("FileName"), DefaultValue("")]
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                if (_FileName == value)
                {
                    return;
                }
                _FileName = value;
                RaisePropertyChanged("FileName");
            }
        }
        #endregion

        #region 属性 ClassName
        private string _ClassName = string.Empty;
        [XmlAttribute("ClassName"), DefaultValue("")]
        public string ClassName
        {
            get
            {
                return _ClassName;
            }
            set
            {
                if (_ClassName == value)
                {
                    return;
                }
                _ClassName = value;
                RaisePropertyChanged("ClassName");
            }
        }
        #endregion

        #region 属性 MethodName
        private string _MethodName = string.Empty;
        [XmlAttribute("MethodName"), DefaultValue("")]
        public string MethodName
        {
            get
            {
                return _MethodName;
            }
            set
            {
                if (_MethodName == value)
                {
                    return;
                }
                _MethodName = value;
                RaisePropertyChanged("MethodName");
            }
        }
        #endregion

        #region 属性 UseFormatResult
        private bool _UseFormatResult = true;
        [XmlAttribute("UseFormatResult"), DefaultValue(true)]
        public bool UseFormatResult
        {
            get
            {
                return _UseFormatResult;
            }
            set
            {
                if (_UseFormatResult == value)
                {
                    return;
                }
                _UseFormatResult = value;
                RaisePropertyChanged("UseFormatResult");
            }
        }
        #endregion

        public bool IsEmptyConfig()
        {
            return MethodName.IsNullOrEmpty() || ClassName.IsNullOrEmpty() || AsmName.IsNullOrEmpty();
        }
    }

    public partial class MethodItem : FunctionItemLocation
    {
        #region 属性 Name
        private string _Name = string.Empty;
        /// <summary>
        /// 参数名称（一旦使用不能修改）
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

        #region 属性 DisplayName
        private string _DisplayName = string.Empty;
        /// <summary>
        /// 显示名称（界面显示名称）
        /// </summary>
        [XmlAttribute("DisplayName")]
        public string DisplayName
        {
            get
            {
                return _DisplayName;
            }
            set
            {
                _DisplayName = value;
                RaisePropertyChanged("DisplayName");
            }
        }
        #endregion
    }

    public partial class OptionType : ConfigItemBase
    {
        #region 属性 Name
        private string _Name = string.Empty;
        /// <summary>
        /// 参数名称（一旦使用不能修改）
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

        #region 属性 DisplayName
        private string _DisplayName = string.Empty;
        /// <summary>
        /// 显示名称（界面显示名称）
        /// </summary>
        [XmlAttribute("DisplayName")]
        public string DisplayName
        {
            get
            {
                return _DisplayName;
            }
            set
            {
                _DisplayName = value;
                RaisePropertyChanged("DisplayName");
            }
        }
        #endregion

        #region 属性 Items
        private OptionItemCollection _Items = null;
        [XmlElement("Item")]
        public OptionItemCollection Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new OptionItemCollection();
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
            foreach (OptionItem item in newItems)
            {
                item.ConfigRoot = this.ConfigRoot;
            }
        }

        protected virtual void OnItemsRemoved(IList oldItems)
        {
            foreach (OptionItem item in oldItems)
            {
                item.ConfigRoot = null;
            }
        }
        #endregion

        protected override void OnConfigRootChanged()
        {
            base.OnConfigRootChanged();
            Items.ForEach(p => p.ConfigRoot = this.ConfigRoot);
        }
    }

    public partial class OptionItemCollection : CollectionBase<OptionItem>
    {
        public OptionItem this[string name]
        {
            get
            {
                return Items.FirstOrDefault(f => f.Name.Equals(name));
            }
        }
    }

    public partial class OptionItem : ConfigItemBase
    {
        #region 属性 Name
        private string _Name = string.Empty;
        /// <summary>
        /// 参数名称（一旦使用不能修改）
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

        #region 属性 Value
        private string _Value = string.Empty;
        [XmlAttribute("Value"), DefaultValue("")]
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

        #region 属性 IsEnabled
        private bool _IsEnabled = true;
        /// <summary>
        /// 是否有效
        /// </summary>
        [XmlAttribute("IsEnabled"), DefaultValue(true)]
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                _IsEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }
        #endregion
    }

    public enum ParameterType
    {
        None,
        Option,
        Bool,
        Int,
        Long,
        Single,
        Double,
        String,
        Object,
        List,
        Image,//图片路径
        FilePath,//文件路径
    }

    public partial class ResultParameter : ParameterBase
    {

    }

    public partial class ParameterItem : ParameterBase
    {
        #region 属性 Max
        private string _Max = string.Empty;
        /// <summary>
        /// 最大值
        /// </summary>
        [XmlAttribute("Max"), DefaultValue("")]
        public string Max
        {
            get
            {
                return _Max;
            }
            set
            {
                _Max = value;
                RaisePropertyChanged("Max");
            }
        }
        #endregion

        #region 属性 Min
        private string _Min = string.Empty;
        /// <summary>
        /// 最小值
        /// </summary>
        [XmlAttribute("Min"), DefaultValue("")]
        public string Min
        {
            get
            {
                return _Min;
            }
            set
            {
                _Min = value;
                RaisePropertyChanged("Min");
            }
        }
        #endregion

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
    }

    public partial class ParameterBase : ConfigItemBase
    {
        #region 属性 Name
        private string _Name = string.Empty;
        /// <summary>
        /// 参数名称（一旦使用不能修改）
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

        #region 属性 DisplayName
        private string _DisplayName = string.Empty;
        /// <summary>
        /// 显示名称（界面显示名称）
        /// </summary>
        [XmlAttribute("DisplayName")]
        public string DisplayName
        {
            get
            {
                if (_DisplayName.IsNullOrEmpty())
                {
                    return Name;
                }
                return _DisplayName;
            }
            set
            {
                _DisplayName = value;
                RaisePropertyChanged("DisplayName");
            }
        }
        #endregion

        #region 属性 Type
        private ParameterType _Type = ParameterType.None;
        /// <summary>
        /// 参数类型：int,string,image,imagelist
        /// </summary>
        [XmlIgnore]
        public ParameterType Type
        {
            get
            {
                if (_Type == ParameterType.None)
                {
                    _Type = GetParameterType();
                }
                return _Type;
            }
            set
            {
                _Type = value;
                RaisePropertyChanged("Type");
            }
        }
        #endregion

        #region 属性 TypeName
        private string _TypeName = string.Empty;
        [XmlAttribute("TypeName"), DefaultValue("")]
        public string TypeName
        {
            get
            {
                return _TypeName;
            }
            set
            {
                _TypeName = value;
                RaisePropertyChanged("TypeName");
                Type = GetParameterType();
            }
        }

        private ParameterType GetParameterType()
        {
            return GetParameterTypeOfName(TypeName);
        }

        public ParameterType GetParameterTypeOfName(string typeName)
        {
            var type = ParameterType.None;
            if (TypeName.IsNotEmpty() && ConfigRoot != null)
            {
                if (ConfigRoot is FunctionConfig config)
                {
                }
                else
                {
                    config = FunctionConfig.Current;
                }
                var typeItem = config.GetType(typeName);
                if (typeItem != null)
                {
                    type = typeItem.Type;
                }
            }

            return type;
        }
        #endregion

        #region 属性 IsEnabled
        private bool _IsEnabled = true;
        /// <summary>
        /// 是否有效
        /// </summary>
        [XmlAttribute("IsEnabled"), DefaultValue(true)]
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                _IsEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }
        #endregion

        public object GetValue(object value)
        {
            try
            {
                switch (Type)
                {
                    case ParameterType.Bool:
                        return value.ToBoolean();
                    case ParameterType.Int:
                        return value.ToInt32();
                    case ParameterType.Long:
                        return value.ToInt64();
                    case ParameterType.Double:
                        return value.ToDouble();
                    case ParameterType.Single:
                        return (Single)value.ToDouble();
                    case ParameterType.Object:
                        return value;
                    case ParameterType.List:
                        if (value is IList ilist)
                        {
                            return ilist;
                        }
                        return value;
                    case ParameterType.FilePath:
                        if (value is string file)
                        {
                            if (file.IsNullOrEmpty())
                            {
                                return "";
                            }

                            var dir = Path.GetDirectoryName(file);
                            dir.CreateFolderIfNotExists();
                            return file;
                        }
                        return value.ToString();
                    case ParameterType.Option:
                    case ParameterType.String:
                    case ParameterType.Image:
                        return value == null ? "" : value.ToString();
                    default:
                        return value;
                }
            }
            catch (Exception ex)
            {
                this.Log($"转换值出错：{Type} {value}", Category.Exception);
                throw;
            }
        }
    }

}
