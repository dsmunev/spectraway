using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using NLog.Config;
using SpectraWay.Controls;
using SpectraWay.Extension;
using SpectraWay.ViewModel;

namespace SpectraWay
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public StringBuilder LogStringBuilder = new StringBuilder();
        public App() : base()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            //ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("sppectra-way-layout", typeof(SpectraWayNLogLayoutRenderer));

            //target
           
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("spectra-way-layout", typeof(SpectraWayNLogLayoutRenderer));
            ConfigurationItemFactory.Default.Targets.RegisterDefinition("spectra-way-target", typeof(SpectraWayNLogTarget));
            //ConfigurationItemFactory.Default.Targets.RegisterDefinition("sppectra-way-target", typeof(BaseViewModel));
            //ConfigurationItemFactory.Default.Targets.RegisterDefinition("sppectra-way-target", typeof(MainWindowViewModel));
            //LogManager.ReconfigExistingLoggers();
            SpectraWayNLogTarget.NewMessageRecieved += (sender, args) => LogStringBuilder.Append(args.Message);

        }

        private ExceptionMessageBox _exceptionMessageBox;
        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (_exceptionMessageBox != null) return;

            string errorMessage = $"An unhandled exception occurred: {e.Exception.Message}";
            _exceptionMessageBox = new ExceptionMessageBox(e.Exception, errorMessage);
            _exceptionMessageBox.Show();
            _exceptionMessageBox.Closed += ExceptionViewerOnClosed;

            e.Handled = true;
        }

        private void ExceptionViewerOnClosed(object sender, EventArgs eventArgs)
        {
            _exceptionMessageBox.Closed -= ExceptionViewerOnClosed;
            _exceptionMessageBox = null;
        }
    }
}
