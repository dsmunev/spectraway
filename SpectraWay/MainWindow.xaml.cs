using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using SpectraWay.Controls;
using SpectraWay.Device.Spectrometer;
using SpectraWay.ViewModel;
using SpectraWay.ViewModel.Experiment;

namespace SpectraWay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public MainWindowViewModel ViewModel  = new MainWindowViewModel();
        public MainWindow()
        {
            DataContext = ViewModel;
            ViewModel.ExperimentListViewModel = new ExperimentListViewModel();
            ViewModel.ExperimentListViewModel.PropertyChanged += ReloadTransitition;
            //ViewModel.ExperimentListViewModel.PropertyChanged += AssignSpectrometer;
            //InitializeComponent();
            Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            _logWindow?.Close();
            Closed -= OnClosed;
        }

        //private void AssignSpectrometer(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "SelectedExperimentTile")
        //    {
        //        if (ViewModel.ExperimentListViewModel.SelectedExperimentTile != null)
        //        {
        //            if (ViewModel.ExperimentListViewModel.SelectedExperimentTile.Spectrometer == null)
        //            {
        //                ViewModel.ExperimentListViewModel.SelectedExperimentTile.Spectrometer =
        //                    SpectrometerManager.Instance.GetDevice(
        //                        ViewModel.ExperimentListViewModel.SelectedExperimentTile.SpectrometerName);

        //            }
        //        }
        //    }
        //}

        private void ReloadTransitition(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedExperimentTile")
            {
                var transitioningContentControl
                    = (TransitioningContentControl)FindName("TransitioningContentControl");
                if (transitioningContentControl != null)
                {
                    if (transitioningContentControl.IsTransitioning)
                    {
                        transitioningContentControl.AbortTransition();
                        transitioningContentControl.TransitionCompleted += TransitioningContentControlOnTransitionCompleted;
                    }
                    else
                    {
                        transitioningContentControl.ReloadTransition();
                    }
                }
            }
        }

        private void TransitioningContentControlOnTransitionCompleted(object sender, RoutedEventArgs routedEventArgs)
        {
            var transitioningContentControl = (TransitioningContentControl) sender;
            transitioningContentControl.ReloadTransition();
            transitioningContentControl.TransitionCompleted -= TransitioningContentControlOnTransitionCompleted;
        }

        public void ThemaButtonOnClick(object sender, RoutedEventArgs e)
        {
            ((Flyout)this.Flyouts.Items[0]).IsOpen = true;
        }


        private void Themaflyout_OnIsOpenChanged(object sender, RoutedEventArgs e)
        {
            
            var flyout = sender as Flyout;
            if (flyout != null)
            {
                //Logger.Info("UI Settings");
                var button = FindVisualChildren<Button>(flyout).FirstOrDefault(x => x.Tag != null && x.Tag.ToString() == ViewModel.CurrentThemaName);
                flyout.FocusedElement = button;
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void Themaflyout_OnClosingFinished(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentOperation = "";
        }

        private static MetroWindow _logWindow = new LogViewer();
        private void ConsoleButton_OnClick(object sender, RoutedEventArgs e)
        {
            _logWindow?.Close();
            _logWindow = new LogViewer();
            _logWindow.Show();
        }

        
    }
}
