// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
namespace AiCodo.Wpf.Controls
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    public class ItemsCommandControl : Control
    {
        static ItemsCommandControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemsCommandControl), new FrameworkPropertyMetadata(typeof(ItemsCommandControl)));
        }

        #region ItemType DependencyProperty
        public Type ItemType
        {
            get { return (Type)GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }
        public static readonly DependencyProperty ItemTypeProperty =
                DependencyProperty.Register("ItemType", typeof(Type), typeof(ItemsCommandControl),
                new PropertyMetadata(null, new PropertyChangedCallback(ItemsCommandControl.OnItemTypePropertyChanged)));

        private static void OnItemTypePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ItemsCommandControl)
            {
                (obj as ItemsCommandControl).OnItemTypeValueChanged();
            }
        }

        protected void OnItemTypeValueChanged()
        {

        }
        #endregion

        #region Items DependencyProperty
        public IList Items
        {
            get { return (IList)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
        public static readonly DependencyProperty ItemsProperty =
                DependencyProperty.Register("Items", typeof(IList), typeof(ItemsCommandControl),
                new PropertyMetadata(null, new PropertyChangedCallback(ItemsCommandControl.OnItemsPropertyChanged)));

        private static void OnItemsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ItemsCommandControl)
            {
                (obj as ItemsCommandControl).OnItemsValueChanged();
            }
        }

        protected void OnItemsValueChanged()
        {

        }
        #endregion

        #region SelectedItem DependencyProperty
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemProperty =
                DependencyProperty.Register("SelectedItem", typeof(object), typeof(ItemsCommandControl),
                new PropertyMetadata(null, new PropertyChangedCallback(ItemsCommandControl.OnSelectedItemPropertyChanged)));

        private static void OnSelectedItemPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ItemsCommandControl)
            {
                (obj as ItemsCommandControl).OnSelectedItemValueChanged();
            }
        }

        protected void OnSelectedItemValueChanged()
        {

        }
        #endregion

        #region AddItemCommand
        private RelayCommand _AddItemCommand = null;
        public RelayCommand AddItemCommand
        {
            get
            {
                if (_AddItemCommand == null)
                {
                    _AddItemCommand = new RelayCommand(OnAddItem, CanAddItem);
                }
                return _AddItemCommand;
            }
        }

        private bool CanAddItem(object parameter)
        {
            return ItemType != null && Items != null;
        }

        private void OnAddItem(object parameter)
        {
            if (Items == null || ItemType == null)
            {
                return;
            }

            var item = ItemType.Assembly.CreateInstance(ItemType.FullName);
            if (parameter != null)
            {
                var index = Items.IndexOf(parameter);
                if (index >= 0)
                {
                    Items.Insert(index + 1, parameter);
                    return;
                }
            }
            Items.Add(item);
        }
        #endregion

        #region RemoveItemCommand
        private RelayCommand _RemoveItemCommand = null;
        public RelayCommand RemoveItemCommand
        {
            get
            {
                if (_RemoveItemCommand == null)
                {
                    _RemoveItemCommand = new RelayCommand(OnRemoveItem, CanRemoveItem);
                }
                return _RemoveItemCommand;
            }
        }

        private bool CanRemoveItem(object parameter)
        {
            return ItemType != null && Items != null && parameter != null;
        }

        private void OnRemoveItem(object parameter)
        {
            if (Items == null || ItemType == null || parameter == null)
            {
                return;
            }
            var index = Items.IndexOf(parameter);
            if (index >= 0)
            {
                Items.Remove(parameter);
                return;
            }
        }
        #endregion

        #region MoveUpCommand
        private RelayCommand _MoveUpCommand = null;
        public RelayCommand MoveUpCommand
        {
            get
            {
                if (_MoveUpCommand == null)
                {
                    _MoveUpCommand = new RelayCommand(OnMoveUp, CanMoveUp);
                }
                return _MoveUpCommand;
            }
        }

        private bool CanMoveUp(object parameter)
        {
            return ItemType != null && Items != null && parameter != null;
        }

        private void OnMoveUp(object parameter)
        {
            if (Items == null || ItemType == null || parameter == null)
            {
                return;
            }
            var index = Items.IndexOf(parameter);
            if (index > 0)
            {
                Items.Remove(parameter);
                Items.Insert(index - 1, parameter);
                return;
            }
        }
        #endregion

        #region MoveDownCommand
        private RelayCommand _MoveDownCommand = null;
        public RelayCommand MoveDownCommand
        {
            get
            {
                if (_MoveDownCommand == null)
                {
                    _MoveDownCommand = new RelayCommand(OnMoveDown, CanMoveDown);
                }
                return _MoveDownCommand;
            }
        }

        private bool CanMoveDown(object parameter)
        {
            return ItemType != null && Items != null && parameter != null;
        }

        private void OnMoveDown(object parameter)
        {
            if (Items == null || ItemType == null || parameter == null)
            {
                return;
            }
            var index = Items.IndexOf(parameter);
            if (index >= 0 && index < (Items.Count - 1))
            {
                Items.Remove(parameter);
                Items.Insert(index + 1, parameter);
                return;
            }
        }
        #endregion

    }
}
