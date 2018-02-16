using System;
using System.Windows.Input;

namespace TestPubSub
{
    class ActionCommand : ICommand
    {
        public Func<object, bool> CanExecuteFunction { get; set; }
        public Action<object> ExecuteAction { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => CanExecuteFunction != null ? CanExecuteFunction(parameter) : false;
        public void Execute(object parameter) => ExecuteAction(parameter);
    }
}