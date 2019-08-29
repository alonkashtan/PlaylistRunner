using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ak.oss.PlaylistRunner.View
{
    public class DelegatingCommand : ICommand
    {
        Action<object> _execute;
        Predicate<object> _canExecute;

        public DelegatingCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, null);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
