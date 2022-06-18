﻿using Autofac;
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
        
        public static void StartAutofac<TWindow>(this Application app) where TWindow:Window
        {
            Builder.RegisterType<TWindow>().AsSelf();
            Builder.RegisterType<ViewService>().As<IViewService>().SingleInstance();
            var container = Builder.Build();
            Container = container;
            ViewService = Container.Resolve<IViewService>();
            ViewService.SetContainer(Container);

            using (var scope = container.BeginLifetimeScope())
            { 
                var window = scope.Resolve<TWindow>();
                window.Show();
            }
        }

        public static void Register<TModule>() where TModule : IModule, new ()
        {
            Builder.RegisterModule<TModule>();
            Builder.RegisterViews(typeof(TModule).Assembly);
        }

        public static ContainerBuilder RegisterViews(this ContainerBuilder builder, Assembly assembly)
        {
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.GetCustomAttribute(typeof(ViewExportAttribute)) != null)
                .Named<IView>(t => ((ViewExportAttribute)t.GetCustomAttribute(typeof(ViewExportAttribute))).Name);

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.GetCustomAttribute(typeof(ViewModelExportAttribute)) != null)
                .Named<IViewModel>(t => ((ViewModelExportAttribute)t.GetCustomAttribute(typeof(ViewModelExportAttribute))).Name);

            return builder;
        }
    }
}