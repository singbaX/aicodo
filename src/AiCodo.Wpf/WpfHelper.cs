// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
namespace AiCodo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;
    public static class WpfHelper
    {
        public static Dispatcher MainDispatcher { get; set; }
        public static Window MainWindow { get; set; }

        private static bool _IsClosed = false;

        private static List<Action> _WaitingActions
            = new List<Action>();

        private static List<Func<object>> _WaitingFunctions
            = new List<Func<object>>();

        private static object _WaitingLock =
            new object();

        public static bool UseMock { get; set; } = false;

        #region 属性 IsClosing
        private static bool _IsClosing = false;
        public static bool IsClosing
        {
            get
            {
                return _IsClosing;
            }
            set
            {
                _IsClosing = value;
            }
        }
        #endregion

        public static bool IsStart()
        {
            return MainDispatcher != null && _IsClosed == false;
        }

        public static void Close(int exitCode = 0)
        {
            if (_IsClosed)
            {
                return;
            }
            _IsClosed = true;
            try
            {
                Process.GetCurrentProcess().Close();
                System.Windows.Application.Current.Shutdown(exitCode);
            }
            catch
            {
            }
        }

        public static void ShowMessageBox(this string message)
        {
            SafeRun(() =>
            {
                System.Windows.MessageBox.Show(message);
            });
        }

        public static void SafeRun(this Action action)
        {
            if (MainDispatcher == null)
            {
                action();
                return;
            }

            if (!MainDispatcher.CheckAccess())
            {
                MainDispatcher.BeginInvoke(action);
                return;
            }
            action();
        }

        public static bool TryGetResource(this ResourceDictionary d, string key, out object value)
        {
            if (d.Contains(key))
            {
                value = d[key];
                return true;
            }
            foreach (var m in d.MergedDictionaries)
            {
                if (TryGetResource(m, key, out value))
                {
                    return true;
                }
            }
            value = null;
            return false;
        }

        public static T FindChild<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            if (obj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }
                    T childItem = FindChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        public static IEnumerable<T> FindAllChild<T>(this DependencyObject obj)
        {
            if (obj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    object child = VisualTreeHelper.GetChild(obj, i);
                    if (child == null)
                    {
                        continue;
                    }
                    if (child is T)
                    {
                        yield return (T)child;
                    }
                    if (child is DependencyObject)
                    {
                        foreach (var item in (child as DependencyObject).FindAllChild<T>())
                        {
                            yield return item;
                        }
                    }
                }
            }
            yield break;
        }

        public static DependencyObject GetParent(this DependencyObject obj)
        {
            return VisualTreeHelper.GetParent(obj);
        }

        public static T FindParent<T>(this DependencyObject obj)
        {
            var p = VisualTreeHelper.GetParent(obj);
            if (p == null)
            {
                return default(T);
            }
            if (p is T tt)
            {
                return tt;
            }
            return FindParent<T>(p);
        }

        public static bool IsChildOf(this DependencyObject obj, DependencyObject parent)
        {
            return FindParent(obj, p => p == parent) != null;
        }

        public static DependencyObject FindParent(this DependencyObject obj, Func<DependencyObject, bool> find)
        {
            var p = VisualTreeHelper.GetParent(obj);
            while (p != null && find(p) == false)
            {
                p = VisualTreeHelper.GetParent(obj);
            }
            return p;
        }

        public static bool IsInDesignMode()
        {
            return MainDispatcher == null;
        }

        public static bool IsInDesignMode(this DependencyObject control)
        {
            return System.ComponentModel.DesignerProperties.GetIsInDesignMode(control);
        }


        public static bool IsModal(this Window window)
        {
            return System.Windows.Interop.ComponentDispatcher.IsThreadModal;
        }

        public static void InvokeFocus(this UIElement element)
        {
            if (!element.Focus())
            {
                element.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(delegate ()
                {
                    element.Focus();
                }));
            }
        }

        public static void MoveToOwnerCenter(this Window window)
        {
            var owner = window.Owner == null ? System.Windows.Application.Current.MainWindow : window.Owner;
            if (owner != null)
            {
                double top = owner.Top + ((owner.Height - window.ActualHeight) / 2);
                double left = owner.Left + ((owner.Width - window.ActualWidth) / 2);

                window.Top = top < 0 ? 0 : top;
                window.Left = left < 0 ? 0 : left;
            }
        }
    }
}
