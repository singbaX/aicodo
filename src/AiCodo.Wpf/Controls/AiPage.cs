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
using System.Windows.Controls;

namespace AiCodo.Wpf.Controls
{
    public class AiPage : ContentControl, IView
    {
        static AiPage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AiPage), new FrameworkPropertyMetadata(typeof(AiPage)));
        }

        #region Title DependencyProperty
        public object Title
        {
            get { return (object)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
                DependencyProperty.Register("Title", typeof(object), typeof(AiPage),
                new PropertyMetadata(null, new PropertyChangedCallback(AiPage.OnTitlePropertyChanged)));

        private static void OnTitlePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is AiPage)
            {
                (obj as AiPage).OnTitleValueChanged();
            }
        }

        protected void OnTitleValueChanged()
        {

        }
        #endregion

        #region TitleTemplate DependencyProperty
        public DataTemplate TitleTemplate
        {
            get { return (DataTemplate)GetValue(TitleTemplateProperty); }
            set { SetValue(TitleTemplateProperty, value); }
        }
        public static readonly DependencyProperty TitleTemplateProperty =
                DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(AiPage),
                new PropertyMetadata(null, new PropertyChangedCallback(AiPage.OnTitleTemplatePropertyChanged)));

        private static void OnTitleTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is AiPage)
            {
                (obj as AiPage).OnTitleTemplateValueChanged();
            }
        }

        protected void OnTitleTemplateValueChanged()
        {

        }
        #endregion
    }
}
