using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using NLog.Config;
using SpectraWay.Extension;
using SpectraWay.Model;

namespace SpectraWay.ViewModel
{
    public abstract class BaseViewModel : IViewModel,IDataErrorInfo, IDisposable
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected static object MainWindowDataContext => Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.DataContext;
        public BaseViewModel(Action<BaseViewModel> closeAction)
        {
            DialogCloseAction = closeAction;
            //();
        }



        protected BaseViewModel()
        {
            DialogCloseAction = (viewmodel) => {};
            //InitLogger();
        }

        //static BaseViewModel()
        //{
        //    ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("sppectra-way-layout",
        //        typeof(SpectraWayNLogLayoutRenderer));

        //    //target
        //    ConfigurationItemFactory.Default.Targets.RegisterDefinition("sppectra-way-target", typeof(SpectraWayNLogTarget));
        //    //ConfigurationItemFactory.Default.Targets.RegisterDefinition("sppectra-way-target", typeof(BaseViewModel));
        //    //ConfigurationItemFactory.Default.Targets.RegisterDefinition("sppectra-way-target", typeof(MainWindowViewModel));
        //    LogManager.ReconfigExistingLoggers();
        //}
        //protected abstract void UpdateData();
        public IDialogCoordinator MetroDialogCoordinator => DialogCoordinator.Instance;
        public SimpleCommand DialogCloseCommand => new SimpleCommand()
                                                   {
                                                       ExecuteDelegate = o => DialogCloseAction(this)
                                                   };

        public Action<BaseViewModel> DialogCloseAction { get; set; }
 
        public virtual void UpdateView()
        {
            NotifyPropertyChanged(string.Empty);
        }

        #region INotifyPropertyChanged Methods

        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Notifies property changed
        /// </summary>
        /// <param name="propertyName">changed property name</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            OnPropertyChanged(e);
        }

        /// <summary>
        /// Fires property changed event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        #endregion


        public virtual string this[string columnName] => null;

        public virtual string Error { get; }

        public virtual void Dispose()
        {

        }

        public static void CallThreadSafe(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(action);
            }
        }

        private bool _isWait;
        public bool IsWait
        {
            get { return _isWait; }
            set
            {
                _isWait = value;
                if (MainWindowDataContext != null && this != MainWindowDataContext)
                {
                    ((MainWindowViewModel)MainWindowDataContext).IsWait = value;
                }
                NotifyPropertyChanged(nameof(IsWait));
            }
        }

        private string _waitMessage;


        public string WaitMessage
        {
            get { return _waitMessage; }
            set
            {
                _waitMessage = value;
                if (MainWindowDataContext != null && this != MainWindowDataContext)
                {
                    ((MainWindowViewModel)MainWindowDataContext).WaitMessage = value;
                }
                NotifyPropertyChanged(nameof(WaitMessage));
            }
        }

    }
}