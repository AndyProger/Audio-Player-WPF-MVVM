using System;
using System.Windows;
using System.Reflection;
using System.Windows.Input;

namespace Player.Services
{
    /// <summary>
    /// Сервис создания модальных окон
    /// </summary>
    public class OpenWindowCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            TypeInfo p = (TypeInfo)parameter;

            return p.BaseType == typeof(Window);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("TargetWindowType");

            TypeInfo p = (TypeInfo)parameter;
            Type t = parameter.GetType();

            if (p.BaseType != typeof(Window))
                throw new InvalidOperationException("parameter is not a Window type");

            Window wnd = Activator.CreateInstance(p) as Window;

            OpenWindow(wnd);
        }

        protected virtual void OpenWindow(Window wnd)
        {
            wnd.ShowDialog();
        }
    }
}
