// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AiCodo
{
    public static class ControlService
    {
        internal class ControlItem
        {
            public string Name { get; set; }

            public string GroupName { get; set; }

            public Type ViewType { get; set; }

            public Type ViewModelType { get; set; }

            public bool IsShared { get; set; } = false;

            public object Control { get; set; }
        }

        static Dictionary<string, ControlItem> _Controls = new Dictionary<string, ControlItem>();

        public static void Register<TView>(string groupName, string controlName, bool isShared = false) 
        {
            var name = $"{groupName.Trim().ToLower()}/{controlName.Trim().ToLower()}";
            var item = new ControlItem
            {
                Name = name,
                GroupName = groupName,
                ViewType = typeof(TView),
                IsShared = isShared
            };
            _Controls[name] = item;
        }

        public static void Register<TView, TViewModel>(string groupName, string controlName, bool isShared = false) 
        {
            var name = $"{groupName.Trim().ToLower()}/{controlName.Trim().ToLower()}";
            var item = new ControlItem
            {
                Name = name,
                GroupName = groupName,
                ViewType = typeof(TView),
                ViewModelType = typeof(TViewModel),
                IsShared = isShared
            };
            _Controls[name] = item;
        }

        public static object CreateControl(string groupName, string controlName)
        {
            var name = $"{groupName.Trim().ToLower()}/{controlName.Trim().ToLower()}";
            if (_Controls.TryGetValue(name, out ControlItem item))
            {
                if (item.ViewType == null)
                {
                    return null;
                }
                var view = item.ViewType.Assembly.CreateInstance(item.ViewType.FullName);
                if(item.ViewModelType!=null && view is FrameworkElement control)
                {
                    var viewModel= item.ViewModelType.Assembly.CreateInstance(item.ViewModelType.FullName); 
                    control.DataContext = viewModel;
                }
                return view;
            }
            return null;
        }
    }
}
