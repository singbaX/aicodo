// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AiCodo
{
    public class AiModule:EntityBase
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

        #region 属性 Icon
        private string _Icon = string.Empty;
        [XmlAttribute("Icon"), DefaultValue("")]
        public string Icon
        {
            get
            {
                return _Icon;
            }
            set
            {
                if (_Icon == value)
                {
                    return;
                }
                _Icon = value;
                RaisePropertyChanged("Icon");
            }
        }
        #endregion

        #region 属性 Url
        private string _Url = string.Empty;
        [XmlAttribute("Url"), DefaultValue("")]
        public string Url
        {
            get
            {
                return _Url;
            }
            set
            {
                if (_Url == value)
                {
                    return;
                }
                _Url = value;
                RaisePropertyChanged("Url");
            }
        }
        #endregion
    }
}
