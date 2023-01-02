// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
namespace AiCodo.Data
{
    using System.ComponentModel;
    using System.Xml.Serialization;
    [XmlRoot("DataItems")]
    public class DataItems : ConfigFile
    {
        #region 属性 Current 
        private static DataItems _Current = null;
        private static object _LoadLock = new object();
        public static DataItems Current
        {
            get
            {
                if (_Current == null)
                {
                    lock (_LoadLock)
                    {
                        if (_Current == null)
                        {
                            _Current = CreateOrLoad<DataItems>("DataItemsConfig.xml");
                        }
                    }
                }
                return _Current;
            }
        }
        #endregion

        #region 属性 Items
        private CollectionBase<DataItem> _Items = null;
        [XmlElement("Item", typeof(DataItem))]
        public CollectionBase<DataItem> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                _Items = value;
                RaisePropertyChanged("Items");
            }
        }
        #endregion
    }

    public class DataItem : EntityBase
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

        #region 属性 Columns
        private CollectionBase<DataColumn> _Columns = null;
        [XmlElement("Column", typeof(DataColumn))]
        public CollectionBase<DataColumn> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    _Columns = new CollectionBase<DataColumn>();
                }
                return _Columns;
            }
            set
            {
                _Columns = value;
                RaisePropertyChanged("Columns");
            }
        }
        #endregion
    }

    public class DataColumn : EntityBase
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

        #region 属性 Type
        private string _Type = string.Empty;
        [XmlAttribute("Type"), DefaultValue("")]
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                if (_Type == value)
                {
                    return;
                }
                _Type = value;
                RaisePropertyChanged("Type");
            }
        }
        #endregion

        #region 属性 DefaultValue
        private string _DefaultValue = string.Empty;
        [XmlAttribute("DefaultValue"), DefaultValue("")]
        public string DefaultValue
        {
            get
            {
                return _DefaultValue;
            }
            set
            {
                if (_DefaultValue == value)
                {
                    return;
                }
                _DefaultValue = value;
                RaisePropertyChanged("DefaultValue");
            }
        }
        #endregion

    }
}
