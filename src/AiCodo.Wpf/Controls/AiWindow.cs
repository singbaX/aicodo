using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AiCodo.Wpf.Controls
{ 
    public class AiWindow : Window
    {
        const int TipSecond = 3;

        ObservableCollection<TipItem> _TipItems = new ObservableCollection<TipItem>();
        bool _IsTipChecking = false;
        object _TipCheckingLock = new object();

        #region AIWindow
        static AiWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AiWindow), new FrameworkPropertyMetadata(typeof(AiWindow)));
        }

        public AiWindow() : this(true)
        {
        }

        public AiWindow(bool useEscapeClose)
        {
            this.DefaultStyleKey = typeof(AiWindow);
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
            if (useEscapeClose)
            {
                SystemCommands.CloseWindowCommand.InputGestures.Add(new KeyGesture(Key.Escape));
            }
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }
        #endregion

        #region HeaderBackground DependencyProperty
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }
        public static readonly DependencyProperty HeaderBackgroundProperty =
                DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(AiWindow),
                new PropertyMetadata(null, new PropertyChangedCallback(AiWindow.OnHeaderBackgroundPropertyChanged)));

        private static void OnHeaderBackgroundPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is AiWindow)
            {
                (obj as AiWindow).OnHeaderBackgroundValueChanged();
            }
        }

        protected void OnHeaderBackgroundValueChanged()
        {

        }
        #endregion

        #region ToolContent DependencyProperty
        public object ToolContent
        {
            get { return (object)GetValue(ToolContentProperty); }
            set { SetValue(ToolContentProperty, value); }
        }
        public static readonly DependencyProperty ToolContentProperty =
                DependencyProperty.Register("ToolContent", typeof(object), typeof(AiWindow),
                new PropertyMetadata(null, new PropertyChangedCallback(AiWindow.OnToolContentPropertyChanged)));

        private static void OnToolContentPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is AiWindow)
            {
                (obj as AiWindow).OnToolContentValueChanged();
            }
        }

        protected void OnToolContentValueChanged()
        {

        }
        #endregion

        #region ToolContentTemplate DependencyProperty
        public DataTemplate ToolContentTemplate
        {
            get { return (DataTemplate)GetValue(ToolContentTemplateProperty); }
            set { SetValue(ToolContentTemplateProperty, value); }
        }
        public static readonly DependencyProperty ToolContentTemplateProperty =
                DependencyProperty.Register("ToolContentTemplate", typeof(DataTemplate), typeof(AiWindow),
                new PropertyMetadata(null, new PropertyChangedCallback(AiWindow.OnToolContentTemplatePropertyChanged)));

        private static void OnToolContentTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is AiWindow)
            {
                (obj as AiWindow).OnToolContentTemplateValueChanged();
            }
        }

        protected void OnToolContentTemplateValueChanged()
        {

        }
        #endregion

        #region ShowTool DependencyProperty
        public bool ShowTool
        {
            get { return (bool)GetValue(ShowToolProperty); }
            set { SetValue(ShowToolProperty, value); }
        }
        public static readonly DependencyProperty ShowToolProperty =
                DependencyProperty.Register("ShowTool", typeof(bool), typeof(AiWindow),
                new PropertyMetadata(false, new PropertyChangedCallback(AiWindow.OnShowToolPropertyChanged)));

        private static void OnShowToolPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is AiWindow)
            {
                (obj as AiWindow).OnShowToolValueChanged();
            }
        }

        protected void OnShowToolValueChanged()
        {

        }
        #endregion

        #region TopContent DependencyProperty
        public object TopContent
        {
            get { return (object)GetValue(TopContentProperty); }
            set { SetValue(TopContentProperty, value); }
        }
        public static readonly DependencyProperty TopContentProperty =
                DependencyProperty.Register("TopContent", typeof(object), typeof(AiWindow),
                new PropertyMetadata(null, new PropertyChangedCallback(AiWindow.OnTopContentPropertyChanged)));

        private static void OnTopContentPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is AiWindow)
            {
                (obj as AiWindow).OnTopContentValueChanged();
            }
        }

        protected void OnTopContentValueChanged()
        {

        }
        #endregion

        #region TopContentTemplate DependencyProperty
        public DataTemplate TopContentTemplate
        {
            get { return (DataTemplate)GetValue(TopContentTemplateProperty); }
            set { SetValue(TopContentTemplateProperty, value); }
        }
        public static readonly DependencyProperty TopContentTemplateProperty =
                DependencyProperty.Register("TopContentTemplate", typeof(DataTemplate), typeof(AiWindow),
                new PropertyMetadata(null, new PropertyChangedCallback(AiWindow.OnTopContentTemplatePropertyChanged)));

        private static void OnTopContentTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is AiWindow)
            {
                (obj as AiWindow).OnTopContentTemplateValueChanged();
            }
        }

        protected void OnTopContentTemplateValueChanged()
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
                DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(AiWindow),
                new PropertyMetadata(true, new PropertyChangedCallback(AiWindow.OnShowCloseButtonPropertyChanged)));

        private static void OnShowCloseButtonPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is AiWindow)
            {
                (obj as AiWindow).OnShowCloseButtonValueChanged();
            }
        }

        protected void OnShowCloseButtonValueChanged()
        {

        }
        #endregion

        #region ShowMaximizeButton DependencyProperty
        public bool ShowMaximizeButton
        {
            get { return (bool)GetValue(ShowMaximizeButtonProperty); }
            set { SetValue(ShowMaximizeButtonProperty, value); }
        }
        public static readonly DependencyProperty ShowMaximizeButtonProperty =
                DependencyProperty.Register("ShowMaximizeButton", typeof(bool), typeof(AiWindow),
                new PropertyMetadata(true, new PropertyChangedCallback(AiWindow.OnShowMaximizeButtonPropertyChanged)));

        private static void OnShowMaximizeButtonPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is AiWindow)
            {
                (obj as AiWindow).OnShowMaximizeButtonValueChanged();
            }
        }

        protected void OnShowMaximizeButtonValueChanged()
        {

        }
        #endregion

        #region ShowMinimizeButton DependencyProperty
        public bool ShowMinimizeButton
        {
            get { return (bool)GetValue(ShowMinimizeButtonProperty); }
            set { SetValue(ShowMinimizeButtonProperty, value); }
        }
        public static readonly DependencyProperty ShowMinimizeButtonProperty =
                DependencyProperty.Register("ShowMinimizeButton", typeof(bool), typeof(AiWindow),
                new PropertyMetadata(true, new PropertyChangedCallback(AiWindow.OnShowMinimizeButtonPropertyChanged)));

        private static void OnShowMinimizeButtonPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is AiWindow)
            {
                (obj as AiWindow).OnShowMinimizeButtonValueChanged();
            }
        }

        protected void OnShowMinimizeButtonValueChanged()
        {

        }
        #endregion

        #region IsTipShow DependencyProperty
        public bool IsTipShow
        {
            get { return (bool)GetValue(IsTipShowProperty); }
            private set { SetValue(IsTipShowProperty, value); }
        }
        public static readonly DependencyProperty IsTipShowProperty =
                DependencyProperty.Register("IsTipShow", typeof(bool), typeof(AiWindow),
                new PropertyMetadata(false, new PropertyChangedCallback(AiWindow.OnIsTipShowPropertyChanged)));

        private static void OnIsTipShowPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is AiWindow)
            {
                (obj as AiWindow).OnIsTipShowValueChanged();
            }
        }

        protected void OnIsTipShowValueChanged()
        {

        }
        #endregion

        #region TipItems DependencyProperty
        public IEnumerable TipItems
        {
            get
            {
                return _TipItems;
            }
        }
        #endregion

        #region tips
        public void AddTip(object obj)
        {
            var item = new TipItem { Message = obj, EndTime = DateTime.Now.AddSeconds(TipSecond) };
            RunSafe(() =>
            {
                _TipItems.Add(item);
                BeginCheckTip();
            });
        }

        private void RemoveTip(TipItem item)
        {
            RunSafe(() => _TipItems.Remove(item));
        }

        private void BeginCheckTip()
        {
            if (_IsTipChecking)
            {
                return;
            }
            lock (_TipCheckingLock)
            {
                if (_IsTipChecking)
                {
                    return;
                }
                _IsTipChecking = true;
                IsTipShow = true;
                ManualResetEvent removeDone = new ManualResetEvent(false);
                Threads.StartNew(() =>
                {
                    while (_TipItems.Count > 0)
                    {
                        removeDone.Reset();
                        RunSafe(() =>
                        {
                            if (_TipItems.Count == 0)
                            {
                                return;
                            }

                            _TipItems.Where(t => t.EndTime < DateTime.Now)
                                .ToList()
                                .ForEach(t =>
                                {
                                    _TipItems.Remove(t);
                                });

                            if (_TipItems.Count == 0)
                            {
                                IsTipShow = false;
                            }
                            removeDone.Set();
                        });
                        removeDone.WaitOne(300);

                        if (_TipItems.Count == 0)
                        {
                            lock (_TipCheckingLock)
                            {
                                if (_TipItems.Count == 0)
                                {
                                    _IsTipChecking = false;
                                    break;
                                }
                            }
                        }
                    }
                });
            }
        }
        #endregion

        public void RunSafe(Action action)
        {
            if (this.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                this.Dispatcher.BeginInvoke(action);
            }
        }
    }
    class TipItem
    {
        public object Message { get; set; }

        public DateTime EndTime { get; set; }
    }
}
