// Copyright (c) 2021 AiCodo.com Corporation. All Rights Reserved.
// Licensed under the MIT License.

namespace AiCodo
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute) : base(execute)
        {
        }

        public RelayCommand(Action<object> execute) : base(execute)
        {
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
            : base(execute, canExecute)
        {
        }
    }

    public class RelayCommand<T> : ICommand
    {
        #region Fields
        private Func<T, bool> _canExecute;
        private Action<T> _execute;
        private Action _action;
        private bool _IsExecuting = false;
        #endregion // Fields

        #region Constructors

        public RelayCommand(Action execute)
        {
            _action = execute;
        }

        public RelayCommand(Action<T> execute)
        {
            this.ResetExecute(execute);
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            ResetActions(execute, canExecute);
        }
        #endregion // Constructors

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : ((!_IsExecuting) && _canExecute(parameter == null ? default(T) : ((T)parameter)));
        }

        public event EventHandler CanExecuteChanged
        {
            add { System.Windows.Input.CommandManager.RequerySuggested += value; }
            remove { System.Windows.Input.CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            try
            {
                if (_IsExecuting)
                    return;
                _IsExecuting = true;
                RaiseCanExecuteChanged();
                if (_execute != null)
                {
                    _execute((T)parameter);
                }
                else if (_action != null)
                {
                    _action();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _IsExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        #endregion

        public RelayCommand<T> ResetExecute(Action<T> execute)
        {
            _execute = execute;
            return this;
        }

        public RelayCommand<T> ResetCanExecute(Func<T, bool> canExecute)
        {
            _canExecute = canExecute;
            return this;
        }

        public void ResetActions(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute != null)
            {
                _execute = execute;
            }
            if (canExecute != null)
            {
                _canExecute = canExecute;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            WpfHelper.SafeRun(() =>
            {
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            });
        }
    }
}