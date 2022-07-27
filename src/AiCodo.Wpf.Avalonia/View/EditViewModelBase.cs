// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
namespace AiCodo
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;

    public class EditViewModelBase : ViewModelBase, IEditableObject, INotifyDataErrorInfo
    {
        private Dictionary<string, List<DataErrorValidate>> ValidateItems = new Dictionary<string, List<DataErrorValidate>>();
        public override void Dispose()
        {
            ValidateItems.Clear();
            errors.Clear();
        }

        protected override void RaisePropertyChanged(string name)
        {
            base.RaisePropertyChanged(name);
            CheckError(name);
        }

        #region INotifyDataErrorInfo错误处理接口实现
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        //存放错误信息，一个Property可能对应多个错误信息        
        private Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        public virtual IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                foreach (var err in errors)
                {
                    foreach (var item in err.Value)
                    {
                        yield return item;
                    }
                }
            }
            else
            {
                if (errors.ContainsKey(propertyName))
                {
                    var errItems = errors[propertyName];
                    foreach (var item in errItems)
                    {
                        yield return item;
                    }
                }
            }
        }

        public bool HasErrors
        {
            get
            {
                return errors.Count > 0;
            }
        }

        protected virtual void SetErrors(string propertyName, List<string> propertyErrors)
        {
            errors.Remove(propertyName);
            if (propertyErrors != null && propertyErrors.Count > 0)
            {
                errors.Add(propertyName, propertyErrors);
            }
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected virtual void ClearErrors(string propertyName = "")
        {
            if (propertyName.IsNullOrEmpty())
            {
                errors.Clear();
                return;
            }
            errors.Remove(propertyName);
        }

        protected virtual bool CheckError(string name = "")
        {
            if (!IsEditing)
            {
                return false;
            }

            if (name.IsNullOrEmpty())
            {
                ClearErrors();
                foreach (var item in ValidateItems)
                {
                    var vitems = item.Value;
                    string error = GetError(vitems);
                    if (error.Length > 0)
                    {
                        SetErrors(item.Key, new List<string> { error });
                    }
                }
                return errors.Count > 0;
            }

            List<DataErrorValidate> items = null;
            if (ValidateItems.TryGetValue(name, out items))
            {
                ClearErrors(name);
                string error = GetError(items);
                if (error.Length > 0)
                {
                    SetErrors(name, new List<string> { error });
                    return true;
                }

                var list = items.Where((v) => (v is DataErrorAsyncValidate)).ToList();
                if (list.Count > 0)
                {
                    CheckAsyncError(name, list);
                }
            }
            return false;
        }
        #endregion

        #region validate 
        protected virtual void AddValidate(string name, DataErrorValidate item)
        {
            lock (ValidateItems)
            {
                List<DataErrorValidate> items = null;
                if (ValidateItems.TryGetValue(name, out items))
                {
                    items.Add(item);
                }
                else
                {
                    items = new List<DataErrorValidate>();
                    items.Add(item);
                    ValidateItems[name] = items;
                }
            }
        }

        public void AddAsyncValidate(string name, Action<Action<bool>> action, string error)
        {
            DataErrorValidate item = new DataErrorAsyncValidate(action, error);
            AddValidate(name, item);
        }

        public void AddValidate(string name, Func<bool> funcIsValidate, string error)
        {
            DataErrorValidate item = new DataErrorValidate(funcIsValidate, error);
            AddValidate(name, item);
        }
        private void CheckAsyncError(string name, IEnumerable<DataErrorValidate> items)
        {
            ClearErrors(name);
            foreach (var item in items)
            {
                var vitem = (DataErrorAsyncValidate)item;
                vitem.AsyncAction((result) =>
                {
                    if (result)
                    {
                        SetErrors(name, new List<string> { vitem.Error });
                    }
                });
            }
        }

        private string GetError(List<DataErrorValidate> items)
        {
            foreach (var item in items.Where((v) => !(v is DataErrorAsyncValidate)))
            {
                if (item.FuncIsValidate())
                {
                    return item.Error;
                }
            }
            return "";
        }
        #endregion

        #region 属性 IsEditing
        private bool _IsEditing = false;
        public bool IsEditing
        {
            get
            {
                return _IsEditing;
            }
            private set
            {
                if (_IsEditing == value)
                {
                    return;
                }
                _IsEditing = value;
                RaisePropertyChanged("IsEditing");
                if (_IsEditing)
                {
                    CheckError();
                }
                else
                {
                    ClearErrors();
                }
            }
        }
        #endregion

        #region IEditableObject
        public virtual void BeginEdit()
        {
            IsEditing = true;
        }

        public virtual void CancelEdit()
        {
            IsEditing = false;
        }

        public virtual void EndEdit()
        {
            IsEditing = false;
        }

        System.Collections.IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) ||
            !this.errors.ContainsKey(propertyName))
            {
                return null;
            }

            return this.errors[propertyName];/*.Where(e => (e is ErrorMessage));*/
        }
        #endregion
    }

    public class ViewModelBase : EntityBase, IDisposable, IViewModel
    {
        public IView View { get; set; }

        public virtual void Dispose()
        {
        }

        public virtual void OnOpened(Dictionary<string, object> args)
        {

        }

        #region OpenDialogCommand
        private RelayCommand _OpenDialogCommand = null;
        public RelayCommand OpenDialogCommand
        {
            get
            {
                if (_OpenDialogCommand == null)
                {
                    _OpenDialogCommand = new RelayCommand(OnOpenDialog, CanOpenDialog);
                }
                return _OpenDialogCommand;
            }
        }

        private bool CanOpenDialog(object parameter)
        {
            return parameter != null;
        }

        private void OnOpenDialog(object parameter)
        {
            ModuleLocator.ViewService.ShowDialog(parameter.ToString(), null);
        }
        #endregion

        protected virtual void CloseView(object result)
        {
            if (View != null && View is ICloseable view)
            {
                view.RequireClose(result);
            }
        }
    }

}
