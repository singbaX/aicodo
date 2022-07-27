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
using System.Windows.Input;

namespace AiCodo.Wpf.Controls
{
    public class DialogWindow : Window, ICloseable
    {
        static DialogWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogWindow), new FrameworkPropertyMetadata(typeof(DialogWindow)));
        }

        public DialogWindow()
        {
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        #region DialogStyle DependencyProperty
        public static readonly DependencyProperty DialogStyleProperty =
            DependencyProperty.RegisterAttached("DialogStyle", typeof(Style), typeof(DialogWindow),
            new PropertyMetadata(null, OnDialogStyleValueChanged));

        public static Style GetDialogStyle(UIElement obj)
        {
            return (Style)obj.GetValue(DialogStyleProperty);
        }

        public static void SetDialogStyle(UIElement obj, Style value)
        {
            obj.SetValue(DialogStyleProperty, value);
        }

        private static void OnDialogStyleValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null && d is UIElement element)
            {
                var window = element.FindParent<DialogWindow>();
                if (window != null)
                {
                    window.Style = (Style)e.NewValue;
                }
                else if (element is FrameworkElement frameworkElement)
                {
                    frameworkElement.Loaded += FrameworkElement_Loaded;
                }
            }
        }

        private static void FrameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement frameworkElement)
            {
                frameworkElement.Loaded -= FrameworkElement_Loaded;
                var window = frameworkElement.FindParent<DialogWindow>();
                if (window != null)
                {
                    window.Style = GetDialogStyle(frameworkElement);
                }
            }
        }
        #endregion

        #region ShowOKButton DependencyProperty
        public bool ShowOKButton
        {
            get { return (bool)GetValue(ShowOKButtonProperty); }
            set { SetValue(ShowOKButtonProperty, value); }
        }
        public static readonly DependencyProperty ShowOKButtonProperty =
                DependencyProperty.Register("ShowOKButton", typeof(bool), typeof(DialogWindow),
                new PropertyMetadata(true, new PropertyChangedCallback(DialogWindow.OnShowOKButtonPropertyChanged)));

        private static void OnShowOKButtonPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DialogWindow)
            {
                (obj as DialogWindow).OnShowOKButtonValueChanged();
            }
        }

        protected void OnShowOKButtonValueChanged()
        {

        }
        #endregion

        #region ShowCancelButton DependencyProperty
        public bool ShowCancelButton
        {
            get { return (bool)GetValue(ShowCancelButtonProperty); }
            set { SetValue(ShowCancelButtonProperty, value); }
        }
        public static readonly DependencyProperty ShowCancelButtonProperty =
                DependencyProperty.Register("ShowCancelButton", typeof(bool), typeof(DialogWindow),
                new PropertyMetadata(true, new PropertyChangedCallback(DialogWindow.OnShowCancelButtonPropertyChanged)));

        private static void OnShowCancelButtonPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DialogWindow)
            {
                (obj as DialogWindow).OnShowCancelButtonValueChanged();
            }
        }

        protected void OnShowCancelButtonValueChanged()
        {

        }
        #endregion

        #region ShowCloseButton DependencyProperty
        public bool ShowCloseButton
        {
            get { return (bool)GetValue(ShowCloseButtonProperty); }
            set { SetValue(ShowCloseButtonProperty, value); }
        }
        public static readonly DependencyProperty ShowCloseButtonProperty =
                DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(DialogWindow),
                new PropertyMetadata(true, new PropertyChangedCallback(DialogWindow.OnShowCloseButtonPropertyChanged)));

        private static void OnShowCloseButtonPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DialogWindow)
            {
                (obj as DialogWindow).OnShowCloseButtonValueChanged();
            }
        }

        protected void OnShowCloseButtonValueChanged()
        {

        }
        #endregion

        #region ShowButtons DependencyProperty
        public bool ShowButtons
        {
            get { return (bool)GetValue(ShowButtonsProperty); }
            set { SetValue(ShowButtonsProperty, value); }
        }
        public static readonly DependencyProperty ShowButtonsProperty =
                DependencyProperty.Register("ShowButtons", typeof(bool), typeof(DialogWindow),
                new PropertyMetadata(true, new PropertyChangedCallback(DialogWindow.OnShowButtonsPropertyChanged)));

        private static void OnShowButtonsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DialogWindow)
            {
                (obj as DialogWindow).OnShowButtonsValueChanged();
            }
        }

        protected void OnShowButtonsValueChanged()
        {

        }
        #endregion

        #region ButtonsTemplate DependencyProperty
        public DataTemplate ButtonsTemplate
        {
            get { return (DataTemplate)GetValue(ButtonsTemplateProperty); }
            set { SetValue(ButtonsTemplateProperty, value); }
        }
        public static readonly DependencyProperty ButtonsTemplateProperty =
                DependencyProperty.Register("ButtonsTemplate", typeof(DataTemplate), typeof(DialogWindow),
                new PropertyMetadata(null, new PropertyChangedCallback(DialogWindow.OnButtonsTemplatePropertyChanged)));

        private static void OnButtonsTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DialogWindow)
            {
                (obj as DialogWindow).OnButtonsTemplateValueChanged();
            }
        }

        protected void OnButtonsTemplateValueChanged()
        {

        }
        #endregion

        #region OKCommand DependencyProperty
        public ICommand OKCommand
        {
            get { return (ICommand)GetValue(OKCommandProperty); }
            set { SetValue(OKCommandProperty, value); }
        }
        public static readonly DependencyProperty OKCommandProperty =
                DependencyProperty.Register("OKCommand", typeof(ICommand), typeof(DialogWindow),
                new PropertyMetadata(null, new PropertyChangedCallback(DialogWindow.OnOKCommandPropertyChanged)));

        private static void OnOKCommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DialogWindow)
            {
                (obj as DialogWindow).OnOKCommandValueChanged();
            }
        }

        protected void OnOKCommandValueChanged()
        {

        }
        #endregion

        #region CancelCommand DependencyProperty
        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }
        public static readonly DependencyProperty CancelCommandProperty =
                DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(DialogWindow),
                new PropertyMetadata(null, new PropertyChangedCallback(DialogWindow.OnCancelCommandPropertyChanged)));

        private static void OnCancelCommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is DialogWindow)
            {
                (obj as DialogWindow).OnCancelCommandValueChanged();
            }
        }

        protected void OnCancelCommandValueChanged()
        {

        }
        #endregion

        public void RequireClose(object state)
        {
            if(state is bool result)
            {
                this.DialogResult = result;
            }
            else
            {
                this.DialogResult = false;
            }
        }
    }
}
