// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AiCodo
{
    public class ContentLoader : EntityBase, IContentLoader, IDisposable
    {
        private object _LoadLock = new object();
        IViewService _ViewService = null;

        public ContentLoader()
        {
            _ViewService = ModuleLocator.ViewService;
        }

        #region 属性 IsLoading
        private bool _IsLoading = false;
        public bool IsLoading
        {
            get
            {
                return _IsLoading;
            }
            internal set
            {
                _IsLoading = value;
                RaisePropertyChanged(() => IsLoading);
            }
        }
        #endregion

        public async Task<object> LoadContentAsync(string url)
        {
            IsLoading = true;
            var page = await LoadPage(url);
            IsLoading = false;
            return page;
        }

        public async Task<IView> LoadPage(string url)
        {
            var pageName = url.TrimStart('/');

            if (string.IsNullOrEmpty(pageName))
                return null;
            int index = pageName.IndexOf('?');
            string contractName = string.Empty;
            string parameters = index < 0 ? "" : pageName.Substring(index + 1);
            if (index > 0)
            {
                contractName = pageName.Substring(0, index);
            }
            else
            {
                contractName = pageName;
            }
            try
            {
                Dictionary<string, object> args = new Dictionary<string, object>();
                parameters.ToDictionary('&','=').ForEach(p => args[p.Key] = p.Value);
                var page = await _ViewService.CreateView(contractName, args);
                return page;
            }
            catch //(Exception ex)
            {
                throw;
            }
        }

        private static Dictionary<string, string> GetRequest(string parameters)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Clear();
            var items = parameters.Split('&');
            foreach (var item in items)
            {
                var nm = item.Split('=');
                if (nm.Length > 1)
                {
                    request[nm[0]] = nm[1];
                }
            }
            return request;
        }

        public void Dispose()
        {
        }
    }
}
