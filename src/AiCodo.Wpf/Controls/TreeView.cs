// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
namespace AiCodo.Wpf.Controls
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using TreeViewItem = System.Windows.Controls.TreeViewItem;
    /// <summary>
    /// 封装绑定选定项
    /// </summary>
    public class TreeView : System.Windows.Controls.TreeView
    {
        //static TreeView()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(AI.Controls.TreeView), new PropertyMetadata(typeof(AI.Controls.TreeView)));
        //}

        #region SelectedItemBinding DependencyProperty
        /// <summary>
        /// 选择项的绑定值
        /// </summary>
        public object SelectedItemBinding
        {
            get { return (object)GetValue(SelectedItemBindingProperty); }
            set { SetValue(SelectedItemBindingProperty, value); }
        }
        /// <summary>
        /// 选择项的绑定
        /// </summary>
        public static readonly DependencyProperty SelectedItemBindingProperty =
                DependencyProperty.Register("SelectedItemBinding", typeof(object), typeof(TreeView),
                new PropertyMetadata(null, new PropertyChangedCallback(TreeView.OnSelectedItemBindingPropertyChanged)));

        private static void OnSelectedItemBindingPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TreeView)
            {
                (obj as TreeView).OnSelectedItemBindingValueChanged();
            }
        }

        /// <summary>
        /// 选定改变
        /// </summary>
        protected void OnSelectedItemBindingValueChanged()
        {
            if (SelectedItemBinding != this.SelectedItem)
            {
                if (SelectedItemBinding == null)
                {
                    ClearSelects();
                    return;
                }
                var treeItem = this.ContainerFromItem(SelectedItemBinding);
                if (treeItem != null)
                {
                    treeItem.IsSelected = true;
                }
            }
        }

        private void ClearSelects()
        {
            var oldSelectedItem = this.ContainerFromItem(this.SelectedItem);
            if (oldSelectedItem != null)
            {
                oldSelectedItem.IsSelected = false;
            }
        }
        #endregion

        #region SelectedItemCommand DependencyProperty
        public ICommand SelectedItemCommand
        {
            get { return (ICommand)GetValue(SelectedItemCommandProperty); }
            set { SetValue(SelectedItemCommandProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemCommandProperty =
                DependencyProperty.Register("SelectedItemCommand", typeof(ICommand), typeof(TreeView),
                new PropertyMetadata(null, new PropertyChangedCallback(TreeView.OnSelectedItemCommandPropertyChanged)));

        private static void OnSelectedItemCommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TreeView)
            {
                (obj as TreeView).OnSelectedItemCommandValueChanged();
            }
        }

        protected void OnSelectedItemCommandValueChanged()
        {

        }
        #endregion

        #region SelectedItemCommandParameter DependencyProperty
        public object SelectedItemCommandParameter
        {
            get { return (object)GetValue(SelectedItemCommandParameterProperty); }
            set { SetValue(SelectedItemCommandParameterProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemCommandParameterProperty =
                DependencyProperty.Register("SelectedItemCommandParameter", typeof(object), typeof(TreeView),
                new PropertyMetadata(null, new PropertyChangedCallback(TreeView.OnSelectedItemCommandParameterPropertyChanged)));

        private static void OnSelectedItemCommandParameterPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TreeView)
            {
                (obj as TreeView).OnSelectedItemCommandParameterValueChanged();
            }
        }

        protected void OnSelectedItemCommandParameterValueChanged()
        {

        }
        #endregion

        #region ItemHoverBackground DependencyProperty
        public Brush ItemHoverBackground
        {
            get { return (Brush)GetValue(ItemHoverBackgroundProperty); }
            set { SetValue(ItemHoverBackgroundProperty, value); }
        }
        public static readonly DependencyProperty ItemHoverBackgroundProperty =
                DependencyProperty.Register("ItemHoverBackground", typeof(Brush), typeof(TreeView),
                new PropertyMetadata(null, new PropertyChangedCallback(TreeView.OnItemHoverBackgroundPropertyChanged)));

        private static void OnItemHoverBackgroundPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TreeView)
            {
                (obj as TreeView).OnItemHoverBackgroundValueChanged();
            }
        }

        protected void OnItemHoverBackgroundValueChanged()
        {

        }
        #endregion

        #region ItemHoverForeground DependencyProperty
        public Brush ItemHoverForeground
        {
            get { return (Brush)GetValue(ItemHoverForegroundProperty); }
            set { SetValue(ItemHoverForegroundProperty, value); }
        }
        public static readonly DependencyProperty ItemHoverForegroundProperty =
                DependencyProperty.Register("ItemHoverForeground", typeof(Brush), typeof(TreeView),
                new PropertyMetadata(null, new PropertyChangedCallback(TreeView.OnItemHoverForegroundPropertyChanged)));

        private static void OnItemHoverForegroundPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TreeView)
            {
                (obj as TreeView).OnItemHoverForegroundValueChanged();
            }
        }

        protected void OnItemHoverForegroundValueChanged()
        {

        }
        #endregion

        #region ItemSelectedBackground DependencyProperty
        public Brush ItemSelectedBackground
        {
            get { return (Brush)GetValue(ItemSelectedBackgroundProperty); }
            set { SetValue(ItemSelectedBackgroundProperty, value); }
        }
        public static readonly DependencyProperty ItemSelectedBackgroundProperty =
                DependencyProperty.Register("ItemSelectedBackground", typeof(Brush), typeof(TreeView),
                new PropertyMetadata(null, new PropertyChangedCallback(TreeView.OnItemSelectedBackgroundPropertyChanged)));

        private static void OnItemSelectedBackgroundPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TreeView)
            {
                (obj as TreeView).OnItemSelectedBackgroundValueChanged();
            }
        }

        protected void OnItemSelectedBackgroundValueChanged()
        {

        }
        #endregion

        #region ItemSelectedForeground DependencyProperty
        public Brush ItemSelectedForeground
        {
            get { return (Brush)GetValue(ItemSelectedForegroundProperty); }
            set { SetValue(ItemSelectedForegroundProperty, value); }
        }
        public static readonly DependencyProperty ItemSelectedForegroundProperty =
                DependencyProperty.Register("ItemSelectedForeground", typeof(Brush), typeof(TreeView),
                new PropertyMetadata(null, new PropertyChangedCallback(TreeView.OnItemSelectedForegroundPropertyChanged)));

        private static void OnItemSelectedForegroundPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TreeView)
            {
                (obj as TreeView).OnItemSelectedForegroundValueChanged();
            }
        }

        protected void OnItemSelectedForegroundValueChanged()
        {

        }
        #endregion

        #region selecteditem changed
        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (SelectedItemBinding != null && item == SelectedItemBinding && element is TreeViewItem treeItem)
            {
                SelectItem(treeItem);
            }
        }

        /// <summary>
        /// 重写
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectedItemChanged(System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            base.OnSelectedItemChanged(e);
            if (SelectedItemBinding == SelectedItem)
            {
                return;
            }
            this.SelectedItemBinding = this.SelectedItem;
            ResetSelectedExpanded();

            if (SelectedItemCommand != null && SelectedItemCommand.CanExecute(SelectedItemCommandParameter))
            {
                SelectedItemCommand.Execute(SelectedItemCommandParameter);
            }
        }
        #endregion 

        private void ResetSelectedExpanded()
        {
            if (SelectedItem == null)
            {
                return;
            }
            var treeItem = this.ContainerFromItem(SelectedItem);
            SelectItem(treeItem);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            var element = e.OriginalSource;
            if (element != null && element is DependencyObject)
            {
                var treeItem = GetParent<TreeViewItem>(element as DependencyObject);
                if (treeItem != null)
                {
                    SelectItem(treeItem);
                    return;
                }
            }

            if (SelectedItem != null)
            {
                var treeItem = this.ContainerFromItem(SelectedItem);
                if (treeItem != null)
                {
                    treeItem.IsSelected = false;
                }
            }
        }

        private void SelectItem(TreeViewItem treeItem)
        {
            if (treeItem == null)
            {
                return;
            }
            if (!treeItem.IsSelected)
            {
                treeItem.IsSelected = true;
                if (!treeItem.IsExpanded)
                {
                    treeItem.IsExpanded = true;
                }
            }
        }

        static T GetParent<T>(DependencyObject obj)
            where T : FrameworkElement
        {
            DependencyObject parent = null;
            try
            {
                parent = VisualTreeHelper.GetParent(obj);
            }
            catch
            {
            }

            while (parent != null)
            {
                if (parent is T)
                {
                    return (T)parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }


        /*	
        public TreeView()
        {
            this.Drop += TreeView_Drop;
            this.AddHandler(TreeViewItem.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown));
        }

        void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(!AllowDrop)
            {
                return;
            }
            DependencyObject uie = this.InputHitTest(e.GetPosition(this)) as DependencyObject;
            System.Diagnostics.Debug.Write(e.Source.GetType().ToString());

            if (uie is System.Windows.Controls.TextBlock && e.Source is TreeViewItem)
            { 
                TreeViewItem lvi = e.Source as TreeViewItem; 
                DragDrop.DoDragDrop(this, lvi, DragDropEffects.Move);
                e.Handled = true; 
            } 
        }

        void TreeView_Drop(object sender, DragEventArgs e)
        {
            if(!AllowDrop)
            {
                return;
            }
            TreeViewItem tvi = e.Source as TreeViewItem;
            TreeViewItem obj = e.Data.GetData(typeof(TreeViewItem)) as TreeViewItem;
            if ((obj.Parent as TreeViewItem) != null)
            {
                (obj.Parent as TreeViewItem).Items.Remove(obj); 
            } 
            else
            {
                this.Items.Remove(obj); 
            } 
            tvi.Items.Insert(0, obj); 
            e.Handled = true;
        }*/
    }

    public static class TreeViewExtensions
    {
        public static TreeViewItem ContainerFromItem(this TreeView treeView, object item)
        {
            if (item == null)
            {
                return null;
            }
            TreeViewItem containerThatMightContainItem = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(item);
            if (containerThatMightContainItem != null)
                return containerThatMightContainItem;
            else
                return ContainerFromItem(treeView.ItemContainerGenerator, treeView.Items, item);
        }

        private static TreeViewItem ContainerFromItem(System.Windows.Controls.ItemContainerGenerator parentItemContainerGenerator, System.Windows.Controls.ItemCollection itemCollection, object item)
        {
            foreach (object curChildItem in itemCollection)
            {
                TreeViewItem parentContainer = (TreeViewItem)parentItemContainerGenerator.ContainerFromItem(curChildItem);
                if (parentContainer == null)
                {
                    continue;
                }

                TreeViewItem containerThatMightContainItem = (TreeViewItem)parentContainer.ItemContainerGenerator.ContainerFromItem(item);
                if (containerThatMightContainItem != null)
                    return containerThatMightContainItem;
                TreeViewItem recursionResult = ContainerFromItem(parentContainer.ItemContainerGenerator, parentContainer.Items, item);
                if (recursionResult != null)
                    return recursionResult;
            }
            return null;
        }

        public static object ItemFromContainer(this TreeView treeView, TreeViewItem container)
        {
            if (container == null)
            {
                return null;
            }
            TreeViewItem itemThatMightBelongToContainer = (TreeViewItem)treeView.ItemContainerGenerator.ItemFromContainer(container);
            if (itemThatMightBelongToContainer != null)
                return itemThatMightBelongToContainer;
            else
                return ItemFromContainer(treeView.ItemContainerGenerator, treeView.Items, container);
        }

        private static object ItemFromContainer(System.Windows.Controls.ItemContainerGenerator parentItemContainerGenerator, System.Windows.Controls.ItemCollection itemCollection, TreeViewItem container)
        {
            foreach (object curChildItem in itemCollection)
            {
                TreeViewItem parentContainer = (TreeViewItem)parentItemContainerGenerator.ContainerFromItem(curChildItem);
                if (parentContainer == null)
                {
                    continue;
                }
                TreeViewItem itemThatMightBelongToContainer = (TreeViewItem)parentContainer.ItemContainerGenerator.ItemFromContainer(container);
                if (itemThatMightBelongToContainer != null)
                    return itemThatMightBelongToContainer;
                TreeViewItem recursionResult = ItemFromContainer(parentContainer.ItemContainerGenerator, parentContainer.Items, container) as TreeViewItem;
                if (recursionResult != null)
                    return recursionResult;
            }
            return null;
        }
    }
}
