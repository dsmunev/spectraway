using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using SpectraWay.DataProvider;
using SpectraWay.DataProvider.Entities;
using SpectraWay.Extension;
using SpectraWay.Localization;
using SpectraWay.Model;
using SpectraWay.ParamsRetriever;
using SpectraWay.ViewModel.Dialog;
using SpectraWay.ViewModel.Experiment;

namespace SpectraWay.ViewModel
{
    public class AccentColorMenuData
    {
        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }

        private ICommand changeAccentCommand;

        public ICommand ChangeAccentCommand
        {
            get { return this.changeAccentCommand ?? (changeAccentCommand = new SimpleCommand { CanExecuteDelegate = x => true, ExecuteDelegate = x => this.DoChangeTheme(x) }); }
        }

        public bool IsFocused
        {
            get
            {
                return Application.Current.MainWindow != null && this.ColorBrush.Equals(Application.Current.MainWindow.Resources["AccentColorBrush"]);
            }
        }

        protected virtual void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var accent = ThemeManager.GetAccent(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
        }
    }

    public class AppThemeMenuData : AccentColorMenuData
    {
        protected override void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var appTheme = ThemeManager.GetAppTheme(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);
        }
    }

    public class MainWindowViewModel : BaseViewModel
    {
        private string _currentThemaName;
        private string _currentOperation;

        public MainWindowViewModel()
        {
            SpectraWayNLogTarget.NewMessageRecieved += TargetOnNewMessageRecieved;
            Logger.Info($"{StringResourceProvider.Instance[StringResourceProvider.Keys.Initializing]} ...");
            // create accent color menu items for the demo
            this.AccentColors = ThemeManager.Accents
                                            .Select(a => new AccentColorMenuData() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                                            .ToList();



            // create metro theme color menu items for the demo
            this.AppThemes = ThemeManager.AppThemes
                                           .Select(a => new AppThemeMenuData() { Name = a.Name.Replace("Base", ""), BorderColorBrush = a.Resources["BlackColorBrush"] as Brush, ColorBrush = a.Resources["WhiteColorBrush"] as Brush })
                                           .ToList();

            ThemeManager.IsThemeChanged += ThemeManagerOnIsThemeChanged;
            _currentThemaName = "Emerald";
            
            EntityDataProvider<ExperimentEntity>.Instance.SaveManager.ReadyToSaveChanged += (sender, args) =>
            {
                //Application.Current.Dispatcher.Invoke((Action) delegate()
                //{
                //    NotifyPropertyChanged(nameof(IsSaveAllEnabled));
                //});
                CallThreadSafe(() => NotifyPropertyChanged(nameof(IsSaveAllEnabled)));

            };

        }

        private void TargetOnNewMessageRecieved(object sender, NewMessageRecievedEventHandlerArgs args)
        {
            CallThreadSafe(() =>
            CurrentOperation = args.Message);
        }

        public bool IsSaveAllEnabled => EntityDataProvider<ExperimentEntity>.Instance.SaveManager.IsReadyToSave;

        public ICommand SaveAllCommand => new SimpleCommand
        {
            ExecuteDelegate = async o =>
            {
                var controller = await MetroDialogCoordinator.ShowProgressAsync(this, $"{StringResourceProvider.Instance[StringResourceProvider.Keys.Saving]}...", $"{StringResourceProvider.Instance[StringResourceProvider.Keys.PleaseWait]}...");
                await Task.Run(() => EntityDataProvider<ExperimentEntity>.Instance.SaveManager.SaveAll());
                Logger.Info(StringResourceProvider.Instance[StringResourceProvider.Keys.AllChangesAreSaved]);
                await controller.CloseAsync();
            }
        };

        public ICommand CreateExperimentCommand => new SimpleCommand
        {
            ExecuteDelegate = o => this.CreateExperimentAction()
        };
        public ICommand ShowExperimentListCommand => new SimpleCommand
        {
            ExecuteDelegate = o =>
            {
                ExperimentListViewModel.SelectedExperimentTile?.StopSpectrometer();
                ExperimentListViewModel.SelectedExperimentTile?.StopStepper();
                ExperimentListViewModel.SelectedExperimentTile = null;
            }
        };
        public ICommand SettingsCommand => new SimpleCommand
        {
            ExecuteDelegate = o => this.SettingsAction()
        };

        private async void SettingsAction()
        {
            var dialog = (BaseMetroDialog)Application.Current.Windows.OfType<MainWindow>().First().Resources["SettingsDialog"];
            var settingsViewModel = new SettingsViewModel
            {
                DialogCloseAction = async (vm) =>
                {
                    await MetroDialogCoordinator.HideMetroDialogAsync(this, dialog);
                },
                DialogSaveAction = async (vm) =>
                {

                    try
                    {
                        var lang = ((SettingsViewModel) vm).Language;
                        var path = ((SettingsViewModel) vm).SavePath;
                        if (lang != "En")
                        {
                            var xml = XElement.Load($"Language/{lang}.xml");
                            StringResourceProvider.Instance.PopulateStringResources(xml.Elements().ToDictionary(x=>x.Attribute("ElementID")?.Value, x=>x.Value));
                        }
                        else
                        {
                            StringResourceProvider.Instance.SetToDefaultStringResources();
                        }
                        //Logger.Info("");
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                    await MetroDialogCoordinator.HideMetroDialogAsync(this, dialog);


                },

            };

            //dialog.Content = experimentDialogViewModel;
            dialog.DataContext = settingsViewModel;
            await MetroDialogCoordinator.ShowMetroDialogAsync(this, dialog);
        }


        private async void CreateExperimentAction()
        {

            var dialog = (BaseMetroDialog)Application.Current.Windows.OfType<MainWindow>().First().Resources["ExperimentDialog"];
            var experimentDialogViewModel = new ExperimentDialogViewModel
            {
                DialogCloseAction = async (vm) =>
                {
                    await MetroDialogCoordinator.HideMetroDialogAsync(this, dialog);
                },
                DialogAddAction = async (vm) =>
                {

                    var newExperimentVm = new ExperimentViewModel();
                    newExperimentVm.InitFieldsFrom(vm);
                    ExperimentListViewModel.ExperimentTileCollection.Add(newExperimentVm);
                    EntityDataProvider<ExperimentEntity>.Instance.SaveManager.Map(newExperimentVm);

                    Logger.Info(StringResourceProvider.Instance[StringResourceProvider.Keys.Experiment_PLACE_WasAdded].Value, newExperimentVm.Name);
                    //todo save previous selected item data

                    ExperimentListViewModel.SelectedExperimentTile = null;
                    await Task.Delay(300);
                    ExperimentListViewModel.SelectedExperimentTile = newExperimentVm;
                    await MetroDialogCoordinator.HideMetroDialogAsync(this, dialog);


                },
                Name = "Experiment #"+DateTime.Now.ToString("yyyy MMMM dd HH:mm"),
                DateTime = DateTime.Now,
                Category = "Spectrum",
                WaveMax = 720,
                WaveMin = 430,
                ExperimentStatus = ExperimentStatus.Unknown,
                //DistanceRange = new ObservableCollection<double>() { 1.25, 1.5, 1.75, 2, 2.5 },
                DistanceRange = new ObservableCollection<double>() { 1.25, 1.375, 1.5, 1.625, 1.75, 1.875, 2, 2.25, 2.5 },
                SpectrometerName = "LOTIS CMS-400",
                
                BaseDistance = 1.25,
                DistanceToAdd = 2

            };

            //dialog.Content = experimentDialogViewModel;
            dialog.DataContext = experimentDialogViewModel;
            experimentDialogViewModel.PhysicModel = ParamsRetrieverManager.GetParamsRetrievers().FirstOrDefault();
            await MetroDialogCoordinator.ShowMetroDialogAsync(this, dialog);

        }

        private void ThemeManagerOnIsThemeChanged(object sender, OnThemeChangedEventArgs e)
        {
            if (e.Accent != null)
            {
                CurrentThemaName = e.Accent.Name;
            }
        }

        public List<AccentColorMenuData> AccentColors { get; set; }
        public List<AppThemeMenuData> AppThemes { get; set; }

        public string CurrentThemaName
        {
            get { return _currentThemaName; }
            set
            {
                _currentThemaName = value;
                NotifyPropertyChanged(nameof(CurrentThemaName));
            }
        }

        public string CurrentOperation
        {
            get { return _currentOperation; }
            set
            {
                _currentOperation = value;
                NotifyPropertyChanged(nameof(CurrentOperation));
            }
        }

        private ExperimentListViewModel _experimentListViewModel;

        public ExperimentListViewModel ExperimentListViewModel
        {
            get { return _experimentListViewModel; }
            set
            {
                _experimentListViewModel = value;
                NotifyPropertyChanged(nameof(ExperimentListViewModel));
            }
        }

    }
}
