using AiCodo.Wpf.Controls;
using Autofac;
using Autofac.Core.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AiCodo
{
    public class ViewService : IViewService
    {
        public IContainer Container { get; private set; }

        public ViewService()
        {

        }

        public void SetContainer(IContainer container)
        {
            Container = container;
        }

        public async void ShowDialog(string dialogName, Dictionary<string, object> args)
        {
            if (Container == null)
            {
                return;
            }
            var view = Container.ResolveNamed<IView>(dialogName);
            if (view is FrameworkElement element)
            {
                var vm = await CreateViewModel(dialogName, args);
                if (vm != null)
                {
                    element.DataContext = vm;
                    vm.View = view;
                }
            }

            if (view is Window window)
            {
                if (window.Owner == null)
                {
                    window.Owner = WpfHelper.MainWindow;
                }
                window.ShowDialog();
                return;
            }

            var dialog = new DialogWindow();
            dialog.Content = view;
            if (dialog.Owner == null)
            {
                dialog.Owner = WpfHelper.MainWindow;
            }
            dialog.SetBinding(Window.DataContextProperty, new Binding("DataContext") { Source = view });
            dialog.ShowDialog();
        }

        public async Task<IView> CreateView(string name, Dictionary<string, object> args)
        {
            if (Container == null)
            {
                return null;
            }
            var view = Container.ResolveNamed<IView>(name);
            if (view is FrameworkElement element)
            {
                var vm = await CreateViewModel(name, args);
                if (vm != null)
                {
                    element.DataContext = vm;
                    vm.View = view;
                }
            }
            return view;
        }

        private async Task<IViewModel> CreateViewModel(string name, Dictionary<string, object> args)
        {
            return await Task.Run(() =>
            {
                IViewModel vm = null;
                try
                {
                    vm = Container.ResolveNamed<IViewModel>(name);
                    if (vm != null)
                    {
                        vm.OnOpened(args);
                    }
                }
                catch (ComponentNotRegisteredException notReg)
                {
                    this.Log($"ViewModel [{name}] not registered.");
                }
                catch
                {
                }
                return vm;
            });
        }
    }
}
