// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
namespace AiCodo.Services
{
    using Autofac;
    using System;
    public class ServiceLocator
    {
        IComponentContext _Context = null;

        #region 属性 Current 
        private static ServiceLocator _Current = new ServiceLocator();
        public static ServiceLocator Current
        {
            get
            {
                return _Current;
            }
        }
        #endregion

        public IComponentContext Context
        {
            get
            {
                if (_Context == null)
                {
                    throw new InvalidOperationException("没有设置autofac的scope");
                }
                return _Context;
            }
        }

        public void SetContext(IComponentContext componentContext)
        {
            _Context = componentContext;
        }
        public TService Get<TService>()
            where TService : notnull
        {
            return Context.Resolve<TService>();
        }

        public object Get(Type serviceType)
        {
            return Context.Resolve(serviceType);
        }

        public TService GetNamed<TService>(string serviceName)
            where TService : notnull
        {
            return Context.ResolveNamed<TService>(serviceName);
        }

        public object GetNamed(string serviceName, Type serviceType)
        {
            return Context.ResolveNamed(serviceName, serviceType);
        }

        public bool TryGetNamed<TService>(string serviceName, out TService service)
            where TService : notnull
        {
            try
            {
                service = Context.ResolveNamed<TService>(serviceName);
                return service != null;
            }
            catch (Exception ex)
            {
                ex.WriteErrorLog();
                this.Log($"[{serviceName}] not found.");
            }
            service = default;
            return false;
        }
    }
}
