using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AiCodo.Wpf.Controls
{
    public class AiPage : TemplatedControl,IView
    {
        //static AiPage()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(AiPage), new FrameworkPropertyMetadata(typeof(AiPage)));
        //}

        #region Title DependencyProperty
        public object Title
        {
            get { return (object)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly StyledProperty<object> TitleProperty =
            StyledElementExtensions
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
