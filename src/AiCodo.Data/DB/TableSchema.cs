﻿/* 
 * author      : singba 
 * email       : singba@163.com 
 * version     : 20210831
 * package     : AiCodo
 * license     : MIT
 * description : let me think a while
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AiCodo.Data
{
    public class TableSchema : Entity
    {
        #region 属性 Name
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return GetFieldValue<string>("Name", string.Empty);
            }
            set
            {
                SetFieldValue("Name", value);
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

        #region 属性 Schema
        [XmlAttribute("Schema")]
        public string Schema
        {
            get
            {
                return GetFieldValue<string>("Schema", string.Empty);
            }
            set
            {
                SetFieldValue("Schema", value);
            }
        }
        #endregion

        #region 属性 CodeName
        [XmlAttribute("CodeName")]
        public string CodeName
        {
            get
            {
                return GetFieldValue<string>("CodeName", string.Empty);
            }
            set
            {
                SetFieldValue("CodeName", value);
            }
        }
        #endregion

        #region 属性 Columns
        private ColumnCollection _Columns = null;
        [XmlArray("Columns"), XmlArrayItem("Column", typeof(Column))]
        public ColumnCollection Columns
        {
            get
            {
                if (_Columns == null)
                {
                    _Columns = new ColumnCollection();
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

        #region 属性 Key
        [XmlAttribute("Key")]
        public string Key
        {
            get
            {
                return GetFieldValue<string>("Key", string.Empty);
            }
            set
            {
                SetFieldValue("Key", value);
            }
        }
        #endregion

        #region 属性 IsValid
        private bool _IsValid = true;
        [XmlAttribute("IsValid"), DefaultValue(true)]
        public bool IsValid
        {
            get
            {
                return _IsValid;
            }
            set
            {
                if (_IsValid == value)
                {
                    return;
                }
                _IsValid = value;
                RaisePropertyChanged("IsValid");
            }
        }
        #endregion

        #region 属性 Connection
        private SqlConnection _Connection = null;
        [XmlIgnore, JsonIgnore]
        public SqlConnection Connection
        {
            get
            {
                return _Connection;
            }
            set
            {
                _Connection = value;
                RaisePropertyChanged("Connection");
            }
        }
        #endregion

        public bool HasAutoIncrementColumn()
        {
            return Columns.FirstOrDefault(c => c.IsAutoIncrement) != null;
        }

        public Column GetAutoIncrementColumn()
        {
            return Columns.FirstOrDefault(c => c.IsAutoIncrement);
        }

        public IDbProvider GetProvider()
        {
            var conn = this.Connection;
            if (conn == null)
            {
                return null;
            }
            return DbProviderFactories.GetProvider(conn.ProviderName);
        }
    }
}