using System;
using System.Windows.Input;

namespace Plainion.Windows.Mvvm
{
    /// <summary>
    /// Simple "delegate" based command implementation. 
    /// If you need a more complete implementation pls use the one from Prism.
    /// </summary>
    public class DelegateCommand<T> : ICommand
    {
        private Action<T> myExec;
        private Func<T, bool> myCanExec;

        public DelegateCommand(Action<T> exec)
            : this(exec, arg => true)
        {
        }

        public DelegateCommand(Action<T> exec, Func<T, bool> canExec)
        {
            Contract.RequiresNotNull(exec, "exec");
            Contract.RequiresNotNull(canExec, "canExec");

            myExec = exec;
            myCanExec = canExec;
        }

        public bool CanExecute(object parameter)
        {
            return myCanExec((T)parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            myExec((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Simple "delegate" based command implementation. 
    /// If you need a more complete implementation pls use the one from Prism.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Action myDelegate;
        private readonly Func<bool> myCanExecuteDelegate;

        public DelegateCommand(Action execute)
            : this(execute, () => true)
        {
        }

        public DelegateCommand(Action exec, Func<bool> canExec)
        {
            Contract.RequiresNotNull(exec, "exec");
            Contract.RequiresNotNull(canExec, "canExec");

            myDelegate = exec;
            myCanExecuteDelegate = canExec;
        }

        public bool CanExecute(object parameter)
        {
            return myCanExecuteDelegate();
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public void Execute(object parameter)
        {
            myDelegate();
        }
    }
}
