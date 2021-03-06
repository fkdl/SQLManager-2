﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DidiDerDenker.BirdsEyeView.Command
{
    /// <summary>
    ///     This class allows delegating the commanding logic to methods passed as parameters,
    ///     and enables a View to bind commands to objects that are not part of the element tree.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region Constructors
        public DelegateCommand(Action executeMethod)
            : this(executeMethod, null, false)
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, false)
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod, bool isAutomaticRequeryDisabled)
        {
            this.executeMethod = executeMethod ?? throw new ArgumentNullException("executeMethod");
            this.canExecuteMethod = canExecuteMethod;
            this.isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }

        #endregion

        #region Public Methods
        
        public bool CanExecute()
        {
            if (this.canExecuteMethod != null)
            {
                return this.canExecuteMethod();
            }
            return true;
        }
        
        public void Execute()
        {
            this.executeMethod?.Invoke();
        }
        
        public bool IsAutomaticRequeryDisabled
        {
            get
            {
                return this.isAutomaticRequeryDisabled;
            }
            set
            {
                if (this.isAutomaticRequeryDisabled != value)
                {
                    if (value)
                    {
                        CommandManagerHelper.RemoveHandlersFromRequerySuggested(this.canExecuteChangedHandlers);
                    }
                    else
                    {
                        CommandManagerHelper.AddHandlersToRequerySuggested(this.canExecuteChangedHandlers);
                    }
                    this.isAutomaticRequeryDisabled = value;
                }
            }
        }
        
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }
        
        protected virtual void OnCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(this.canExecuteChangedHandlers);
        }

        #endregion

        #region ICommand Members
        
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!this.isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested += value;
                }
                CommandManagerHelper.AddWeakReferenceHandler(ref this.canExecuteChangedHandlers, value, 2);
            }
            remove
            {
                if (!this.isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested -= value;
                }
                CommandManagerHelper.RemoveWeakReferenceHandler(this.canExecuteChangedHandlers, value);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            Execute();
        }

        #endregion

        #region Fields

        private readonly Action executeMethod = null;
        private readonly Func<bool> canExecuteMethod = null;
        private bool isAutomaticRequeryDisabled = false;
        private List<WeakReference> canExecuteChangedHandlers;

        #endregion
    }

    /// <summary>
    ///     This class allows delegating the commanding logic to methods passed as parameters,
    ///     and enables a View to bind commands to objects that are not part of the element tree.
    /// </summary>
    /// <typeparam name="T">Type of the parameter passed to the delegates</typeparam>
    public class DelegateCommand<T> : ICommand
    {
        #region Constructors
        
        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null, false)
        {
        }
        
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, false)
        {
        }
        
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod, bool isAutomaticRequeryDisabled)
        {
            this.executeMethod = executeMethod ?? throw new ArgumentNullException("executeMethod");
            this.canExecuteMethod = canExecuteMethod;
            this.isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }

        public DelegateCommand(ICommand singularOperationButtonPress, Func<string, bool> canSingularOperationButtonPress)
        {
            this.singularOperationButtonPress = singularOperationButtonPress;
            this.canSingularOperationButtonPress = canSingularOperationButtonPress;
        }

        #endregion

        #region Public Methods

        public bool CanExecute(T parameter)
        {
            if (this.canExecuteMethod != null)
            {
                return this.canExecuteMethod(parameter);
            }
            return true;
        }
        
        public void Execute(T parameter)
        {
            this.executeMethod?.Invoke(parameter);
        }
        
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(this.canExecuteChangedHandlers);
        }
        
        public bool IsAutomaticRequeryDisabled
        {
            get
            {
                return this.isAutomaticRequeryDisabled;
            }
            set
            {
                if (this.isAutomaticRequeryDisabled != value)
                {
                    if (value)
                    {
                        CommandManagerHelper.RemoveHandlersFromRequerySuggested(this.canExecuteChangedHandlers);
                    }
                    else
                    {
                        CommandManagerHelper.AddHandlersToRequerySuggested(this.canExecuteChangedHandlers);
                    }
                    this.isAutomaticRequeryDisabled = value;
                }
            }
        }

        #endregion

        #region ICommand Members
        
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!this.isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested += value;
                }
                CommandManagerHelper.AddWeakReferenceHandler(ref this.canExecuteChangedHandlers, value, 2);
            }
            remove
            {
                if (!this.isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested -= value;
                }
                CommandManagerHelper.RemoveWeakReferenceHandler(this.canExecuteChangedHandlers, value);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (parameter == null &&
                typeof(T).IsValueType)
            {
                return (this.canExecuteMethod == null);
            }
            return CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            Execute((T)parameter);
        }

        #endregion

        #region Fields

        private readonly Action<T> executeMethod = null;
        private readonly Func<T, bool> canExecuteMethod = null;
        private bool isAutomaticRequeryDisabled = false;
        private List<WeakReference> canExecuteChangedHandlers;
        private ICommand singularOperationButtonPress;
        private Func<string, bool> canSingularOperationButtonPress;

        #endregion
    }

    /// <summary>
    ///     This class contains methods for the CommandManager that help avoid memory leaks by
    ///     using weak references.
    /// </summary>
    internal class CommandManagerHelper
    {
        internal static void CallWeakReferenceHandlers(List<WeakReference> handlers)
        {
            if (null != handlers)
            {
                EventHandler[] callees = new EventHandler[handlers.Count];
                int count = 0;

                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    WeakReference reference = handlers[i];
                    EventHandler handler = reference.Target as EventHandler;
                    if (null == handler)
                    {
                        handlers.RemoveAt(i);
                    }
                    else
                    {
                        callees[count] = handler;
                        count++;
                    }
                }
                
                for (int i = 0; i < count; i++)
                {
                    EventHandler handler = callees[i];
                    handler(null, EventArgs.Empty);
                }
            }
        }

        internal static void AddHandlersToRequerySuggested(List<WeakReference> handlers)
        {
            if (null != handlers)
            {
                foreach (WeakReference handlerRef in handlers)
                {
                    EventHandler handler = handlerRef.Target as EventHandler;
                    if (null != handler)
                    {
                        CommandManager.RequerySuggested += handler;
                    }
                }
            }
        }

        internal static void RemoveHandlersFromRequerySuggested(List<WeakReference> handlers)
        {
            if (null != handlers)
            {
                foreach (WeakReference handlerRef in handlers)
                {
                    EventHandler handler = handlerRef.Target as EventHandler;
                    if (null != handler)
                    {
                        CommandManager.RequerySuggested -= handler;
                    }
                }
            }
        }

        internal static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler)
        {
            AddWeakReferenceHandler(ref handlers, handler, -1);
        }

        internal static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler, int defaultListSize)
        {
            if (null == handlers)
            {
                handlers = (defaultListSize > 0 ? new List<WeakReference>(defaultListSize) : new List<WeakReference>());
            }

            handlers.Add(new WeakReference(handler));
        }

        internal static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
        {
            if (null != handlers)
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    WeakReference reference = handlers[i];
                    EventHandler existingHandler = reference.Target as EventHandler;
                    if ((null == existingHandler) || (existingHandler == handler))
                    {
                        handlers.RemoveAt(i);
                    }
                }
            }
        }
    }
}
