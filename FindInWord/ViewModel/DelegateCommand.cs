using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FindInWord.ViewModel
{
    public class DelegateCommand : ICommand
    {
        Action<object> execute;
        Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged//вызывается при изменении состояния команды
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)//хранит логику команды
        {
            if (execute != null)
            {
                execute(parameter);
            }

        }

        public bool CanExecute(object parameter)// вернет true, если команда доступна к использованию, false, если нет
        {
            if (canExecute != null)
                return canExecute(parameter);
            return true;
        }

        public DelegateCommand(Action<object> execute) : this(execute, null) { }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
    }

}

