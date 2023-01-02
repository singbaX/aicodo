// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AiCodo;
using Module = Autofac.Module;
using Autofac.Core;

namespace AiCodo
{
    public static class ModuleLocator
    {
        public static IContainer Container { get; private set; }

        public static ContainerBuilder Builder { get; private set; }

        public static IViewService ViewService { get; private set; }

        static ModuleLocator()
        {
            var builder = new ContainerBuilder();
            Builder = builder;
        }

        public static void StartAutofac<TWindow>(this Application app) where TWindow : Window
        {
            Builder.RegisterType<TWindow>().AsSelf();
            Builder.RegisterType<ViewService>().As<IViewService>().SingleInstance();
            var container = Builder.Build();
            Container = container;
            ViewService = Container.Resolve<IViewService>();
            ViewService.SetContainer(Container);

            var scope = container.BeginLifetimeScope();
            ServiceLocator.Current.SetContext(scope);
            var window = scope.Resolve<TWindow>();
            window.Show();
        }

        public static void Register<TModule>() where TModule : IModule, new()
        {
            Builder.RegisterModule<TModule>();
            Builder.RegisterViews(typeof(TModule).Assembly);
        }

        public static ContainerBuilder RegisterViews(this ContainerBuilder builder, Assembly assembly)
        {
            builder.RegisterAssemblyTypes(assembly)
                .Where(t =>
                {
                    var attr = t.GetCustomAttribute(typeof(ViewExportAttribute));
                    return attr != null && ((ViewExportAttribute)attr).IsShared;
                })
                .Named<IView>(t => ((ViewExportAttribute)t.GetCustomAttribute(typeof(ViewExportAttribute))).Name)
                .SingleInstance();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t =>
                {
                    var attr = t.GetCustomAttribute(typeof(ViewExportAttribute));
                    return attr != null && ((ViewExportAttribute)attr).IsShared==false;
                })
                .Named<IView>(t => ((ViewExportAttribute)t.GetCustomAttribute(typeof(ViewExportAttribute))).Name);

            builder.RegisterAssemblyTypes(assembly)
                .Where(t =>
                {
                    var attr = t.GetCustomAttribute(typeof(ViewModelExportAttribute));
                    return attr != null && ((ViewModelExportAttribute)attr).IsShared;
                })
                .Named<IViewModel>(t => ((ViewModelExportAttribute)t.GetCustomAttribute(typeof(ViewModelExportAttribute))).Name)
                .SingleInstance();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t =>
                {
                    var attr = t.GetCustomAttribute(typeof(ViewModelExportAttribute));
                    return attr != null && ((ViewModelExportAttribute)attr).IsShared==false;
                })
                .Named<IViewModel>(t => ((ViewModelExportAttribute)t.GetCustomAttribute(typeof(ViewModelExportAttribute))).Name);

            return builder;
        }
    }
}
