// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using System;
using System.Windows;
using System.Windows.Controls;

namespace AiCodo
{
    public class FrameControl : ContentControl
    {
        //static FrameControl()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameControl), new FrameworkPropertyMetadata(typeof(FrameControl)));
        //}

        #region Source DependencyProperty
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty =
                DependencyProperty.Register("Source", typeof(string), typeof(FrameControl),
                new PropertyMetadata("", new PropertyChangedCallback(FrameControl.OnSourcePropertyChanged)));

        private static void OnSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is FrameControl)
            {
                (obj as FrameControl).OnSourceValueChanged();
            }
        }

        protected void OnSourceValueChanged()
        {
            ResetContent();
        }
        #endregion

        #region ContentLoader DependencyProperty
        public IContentLoader ContentLoader
        {
            get { return (IContentLoader)GetValue(ContentLoaderProperty); }
            set { SetValue(ContentLoaderProperty, value); }
        }
        public static readonly DependencyProperty ContentLoaderProperty =
                DependencyProperty.Register("ContentLoader", typeof(IContentLoader), typeof(FrameControl),
                new PropertyMetadata(null, new PropertyChangedCallback(FrameControl.OnContentLoaderPropertyChanged)));

        private static void OnContentLoaderPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is FrameControl)
            {
                (obj as FrameControl).OnContentLoaderValueChanged();
            }
        }

        protected void OnContentLoaderValueChanged()
        {
            ResetContent();
        }
        #endregion

        #region IsLoading DependencyProperty
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            private set { SetValue(IsLoadingProperty, value); }
        }
        public static readonly DependencyProperty IsLoadingProperty =
                DependencyProperty.Register("IsLoading", typeof(bool), typeof(FrameControl),
                new PropertyMetadata(false, new PropertyChangedCallback(FrameControl.OnIsLoadingPropertyChanged)));

        private static void OnIsLoadingPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is FrameControl)
            {
                (obj as FrameControl).OnIsLoadingValueChanged();
            }
        }

        protected void OnIsLoadingValueChanged()
        {

        }
        #endregion

        private async void ResetContent()
        {
            if (ContentLoader == null || Source.IsNullOrEmpty())
            {
                return;
            }
            IsLoading = true;
            try
            {
                var content = await ContentLoader.LoadContentAsync(Source);
                this.Content = content;
            }
            catch (Exception ex)
            {
                this.Content = ex.ToString();
            }
            IsLoading = false;
        }
    }
}
