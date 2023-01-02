// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
namespace AiCodo.Wpf.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    public class BlockControl : ContentControl
    {
        static BlockControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockControl), new FrameworkPropertyMetadata(typeof(BlockControl)));
        }

        #region ExpanderButtonStyle DependencyProperty
        public Style ExpanderButtonStyle
        {
            get { return (Style)GetValue(ExpanderButtonStyleProperty); }
            set { SetValue(ExpanderButtonStyleProperty, value); }
        }
        public static readonly DependencyProperty ExpanderButtonStyleProperty =
                DependencyProperty.Register("ExpanderButtonStyle", typeof(Style), typeof(BlockControl),
                new PropertyMetadata(null, new PropertyChangedCallback(BlockControl.OnExpanderButtonStylePropertyChanged)));

        private static void OnExpanderButtonStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is BlockControl)
            {
                (obj as BlockControl).OnExpanderButtonStyleValueChanged();
            }
        }

        protected void OnExpanderButtonStyleValueChanged()
        {

        }
        #endregion

        #region Header DependencyProperty
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
                DependencyProperty.Register("Header", typeof(object), typeof(BlockControl),
                new PropertyMetadata(null, new PropertyChangedCallback(BlockControl.OnHeaderPropertyChanged)));

        private static void OnHeaderPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is BlockControl)
            {
                (obj as BlockControl).OnHeaderValueChanged();
            }
        }

        protected void OnHeaderValueChanged()
        {

        }
        #endregion

        #region HeaderTemplate DependencyProperty
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }
        public static readonly DependencyProperty HeaderTemplateProperty =
                DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(BlockControl),
                new PropertyMetadata(null, new PropertyChangedCallback(BlockControl.OnHeaderTemplatePropertyChanged)));

        private static void OnHeaderTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is BlockControl)
            {
                (obj as BlockControl).OnHeaderTemplateValueChanged();
            }
        }

        protected void OnHeaderTemplateValueChanged()
        {

        }
        #endregion

        #region HeaderTextStyle DependencyProperty
        public Style HeaderTextStyle
        {
            get { return (Style)GetValue(HeaderTextStyleProperty); }
            set { SetValue(HeaderTextStyleProperty, value); }
        }
        public static readonly DependencyProperty HeaderTextStyleProperty =
                DependencyProperty.Register("HeaderTextStyle", typeof(Style), typeof(BlockControl),
                new PropertyMetadata(null, new PropertyChangedCallback(BlockControl.OnHeaderTextStylePropertyChanged)));

        private static void OnHeaderTextStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is BlockControl)
            {
                (obj as BlockControl).OnHeaderTextStyleValueChanged();
            }
        }

        protected void OnHeaderTextStyleValueChanged()
        {

        }
        #endregion

        #region ContentBackground DependencyProperty
        public Brush ContentBackground
        {
            get { return (Brush)GetValue(ContentBackgroundProperty); }
            set { SetValue(ContentBackgroundProperty, value); }
        }
        public static readonly DependencyProperty ContentBackgroundProperty =
                DependencyProperty.Register("ContentBackground", typeof(Brush), typeof(BlockControl),
                new PropertyMetadata(null, new PropertyChangedCallback(BlockControl.OnContentBackgroundPropertyChanged)));

        private static void OnContentBackgroundPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is BlockControl)
            {
                (obj as BlockControl).OnContentBackgroundValueChanged();
            }
        }

        protected void OnContentBackgroundValueChanged()
        {

        }
        #endregion

        #region ShowExpander DependencyProperty
        public bool ShowExpander
        {
            get { return (bool)GetValue(ShowExpanderProperty); }
            set { SetValue(ShowExpanderProperty, value); }
        }
        public static readonly DependencyProperty ShowExpanderProperty =
                DependencyProperty.Register("ShowExpander", typeof(bool), typeof(BlockControl),
                new PropertyMetadata(false, new PropertyChangedCallback(BlockControl.OnShowExpanderPropertyChanged)));

        private static void OnShowExpanderPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is BlockControl)
            {
                (obj as BlockControl).OnShowExpanderValueChanged();
            }
        }

        protected void OnShowExpanderValueChanged()
        {

        }
        #endregion

        #region IsExpanded DependencyProperty
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }
        public static readonly DependencyProperty IsExpandedProperty =
                DependencyProperty.Register("IsExpanded", typeof(bool), typeof(BlockControl),
                new PropertyMetadata(true, new PropertyChangedCallback(BlockControl.OnIsExpandedPropertyChanged)));

        private static void OnIsExpandedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is BlockControl)
            {
                (obj as BlockControl).OnIsExpandedValueChanged();
            }
        }

        protected void OnIsExpandedValueChanged()
        {

        }
        #endregion

    }
}
