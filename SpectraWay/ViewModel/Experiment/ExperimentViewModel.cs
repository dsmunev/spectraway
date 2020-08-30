using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
//using LiveCharts;
//using LiveCharts.Configurations;
//using LiveCharts.Defaults;
//using LiveCharts.Wpf;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SpectraWay.Controls;
using SpectraWay.DataProvider.Entities;
using SpectraWay.Device.Spectrometer;
using SpectraWay.Device.Stepper;
using SpectraWay.Extension;
using SpectraWay.Helpers;
using SpectraWay.Localization;
using SpectraWay.Model;
using SpectraWay.ParamsRetriever;

namespace SpectraWay.ViewModel.Experiment
{
    public class ExperimentViewModel : ExperimentTileViewModel, IListenStringResourceProvider
    {
        private SpectrometerDataHelper _dataHelper = new SpectrometerDataHelper { };
        public ExperimentViewModel()
        {
            IsLedEnabled = true;
            IsRealTimeSpectrometerData = true;
            ExposureTime = 200;
            CcdLevels = 2048;
            SetCcdLevelsCollectionValues();
            StringResourceProvider.Instance.AddWeakHandler(this);

        }



        public ICommand MeasureAllDistancesCommand => new SimpleCommand
        {
            ExecuteDelegate = o =>
            {
                StartAutomaticallyMeasurements();
            }
        };

        public ICommand RetrieveParamsCommand => new SimpleCommand
        {
            ExecuteDelegate = o =>
            {
                //todo check saveddata
                RetrieveParams();
            }
        };
        public ICommand AutoFilteringCommand => new SimpleCommand
        {
            ExecuteDelegate = async o =>
            {
                WaitMessage = "";
                IsWait = true;
                //todo check saveddata
                foreach (var experimentEntityDataItemViewModel in SavedData.DataItems)
                {
                    experimentEntityDataItemViewModel.IntensityArray =
                        await
                            SpectraFilteringHelper.Filtering(experimentEntityDataItemViewModel.IntensityArray,
                                                             SavedData.WaveLengthArray, 
                                                             0.1, 
                                                             50);
                }
                IsWait = false;

            }
        };

        public ICommand OpenFilteringWindow => new SimpleCommand
        {
            ExecuteDelegate = o =>
            {
                if (o is ExperimentEntityDataItemViewModel)
                {
                    var item = (ExperimentEntityDataItemViewModel) o;
                    var data = new ExperimentEntityDataViewModel
                    {
                        WaveLengthArray = (double[]) SavedData.WaveLengthArray.Clone()
                    };
                    var spectraToFiltering = new ExperimentEntityDataItemViewModel
                    {
                        IntensityArray = (double[]) item.IntensityArray.Clone(),
                        Distance = item.Distance,
                        IsShow = true
                    };
                    var spectraFiltered = new ExperimentEntityDataItemViewModel
                    {
                        IntensityArray = (double[])item.IntensityArray.Clone(),
                        Distance = item.Distance,
                        IsShow = true,
                        IsFiltred = true
                    };
                    data.DataItems = new FullyObservableCollection<ExperimentEntityDataItemViewModel>(new [] { spectraToFiltering, spectraFiltered });

                    var window = new ApplyFilterWindow
                    {
                        FilteredData = data,
                        OriginalData = SavedData
                    };
                    window.Show();


                    //MessageBox.Show(((ExperimentEntityDataItemViewModel)o).Distance.ToString());
                }

            },
            CanExecuteDelegate = o => true
        };


        private bool _isAutamaticMeasurements;

        private async void StartAutomaticallyMeasurements()
        {
            //todo rewrite to states and events
            string message = null;
            var isReady = true;
            if (!_spectrometer.IsSpectrometerReady)
            {
                message =
                    StringResourceProvider.Instance[
                        StringResourceProvider.Keys.PleaseRunSpectrometerAndTryToGetSpectralMeasurements].Value;
                isReady = false;
            }
            if (!_stepper.IsStepperReady)
            {
                message =
                    StringResourceProvider.Instance[
                        StringResourceProvider.Keys.PleaseMoveDetectorForSomeDistance].Value;
                isReady = false;
            }

            if (!isReady)
            {
                IsWait = true;
                WaitMessage =
                    StringResourceProvider.Instance[StringResourceProvider.Keys.SpectrometerFailedSeeLogForDetails].Value;
                Logger.Warn(message);
                await Task.Delay(3000);
                IsWait = false;
                return;
            }

            try
            {
                _isAutamaticMeasurements = true;
                var newData = new ExperimentEntityDataViewModel();
                newData.WaveLengthArray = Values.ToArray().Select(x => x.WaveLength).ToArray();
                var list = new List<ExperimentEntityDataItemViewModel>();


                foreach (var d in DistanceRange)
                {
                    IsWait = true;
                    message =
                        string.Format(
                            StringResourceProvider.Instance[StringResourceProvider.Keys.StartMovingToDistance_PLACE_Mm]
                                .Value, d);
                    Logger.Info(message);
                    WaitMessage = message;
                    await _stepper.GoToDistanceAsync(d);

                    //TODO REWRITE!!!
                    await Task.Delay(ExposureTime*3);

                    var item = new ExperimentEntityDataItemViewModel();
                    item.Distance = CurrentDistance;
                    item.IsBase = BaseDistance == item.Distance;

                    var values = Values.ToArray();
                    item.IntensityArray = values.Select(x => x.Intencity).ToArray();
                    item.IsAppliedNormalizing = IsNormalize;
                    item.IsNoiseRemoved = IsNoiseRemove;
                    //item.IsShow = true;
                    list.Add(item);

                }

                if (SavedData?.DataItems?.Any(x => x.IsNoise) ?? false)
                    list.Add(SavedData.DataItems.First(x => x.IsNoise));
                if (SavedData?.DataItems?.Any(x => x.IsNormalize) ?? false)
                    list.Add(SavedData.DataItems.First(x => x.IsNormalize));

                message = $"--=== {StringResourceProvider.Instance[StringResourceProvider.Keys.Saving]} ===--";
                Logger.Info(message);
                WaitMessage = message;
                newData.DataItems = new FullyObservableCollection<ExperimentEntityDataItemViewModel>(list);
                SavedData = newData;


                Logger.Info(
                    StringResourceProvider.Instance[
                        StringResourceProvider.Keys.SpectraForAllDistancesSuccessfullyRecieved].Value);
                await _stepper.GoToDistanceAsync(0);
                RetrieveParams();

            }
            catch (Exception e)
            {
                WaitMessage =
                    StringResourceProvider.Instance[StringResourceProvider.Keys.SpectrometerFailedSeeLogForDetails]
                        .Value;
                await Task.Delay(3000);
                Logger.Error(e);
            }
            finally
            {
                _isAutamaticMeasurements = false;
            }
            IsWait = false;

        }


        public ICommand SetExposureCommand => new SimpleCommand
        {
            ExecuteDelegate = o =>
            {
                var param = o as double?;
                if (param != null)
                {
                    Logger.Info(StringResourceProvider.Instance[StringResourceProvider.Keys.AttemptToSetExposureTimeTo_PLACE_Ms].Value, param.Value);
                    var expTime = (int)param.Value;

                    if (_spectrometer != null && _spectrometer.IsSpectrometerReady)
                    {
                        _exposureTime = _spectrometer.SetExposureTime(expTime)
                            ? expTime
                            : _spectrometer.GetExposureTime();
                        NotifyPropertyChanged(nameof(ExposureTime));
                        Logger.Info(StringResourceProvider.Instance[StringResourceProvider.Keys.ExposureTimeWasSetTo_PLACE_Ms].Value, expTime);
                    }
                    else
                    {
                        Logger.Info(StringResourceProvider.Instance[StringResourceProvider.Keys.AttemptSetExposureTimeTo_PLACE_MsFailedSpectrometerNotReady].Value, param.Value);
                    }

                }

            }
        };

        public ICommand GoToDistanceCommand => new SimpleCommand
        {
            ExecuteDelegate = async o =>
            {

                if (o is double)
                {

                    //IsRealTimeSpectrometerData = true;
                    //CurrentDistance = (double)o;
                    var message = string.Format(StringResourceProvider.Instance[StringResourceProvider.Keys.StartMovingToDistance_PLACE_Mm].Value, o);//$"Start moving to distance {o}mm";
                    Logger.Info(message);
                    WaitMessage = message;
                    IsWait = true;
                    await _stepper.GoToDistanceAsync((double)o);

                }

            }
        };

        public ICommand SaveAsCommand => new SimpleCommand
        {
            ExecuteDelegate = o =>
           {

               if (o is string)
               {
                   SaveValues((string)o);

               }

           }
        };

        public ICommand SetIsRealTimeSpectrometerCommand => new SimpleCommand
        {
            ExecuteDelegate = o =>
            {

                if (o is bool)
                {
                    IsRealTimeSpectrometerData = (bool)o;
                }

            }
        };


        private async void SaveValues(string param)
        {

            if (Values == null || Values.Count < 1)
            {
                await MetroDialogCoordinator.ShowMessageAsync(MainWindowDataContext,
                    StringResourceProvider.Instance[StringResourceProvider.Keys.DataIsEmpty].Value,
                    StringResourceProvider.Instance[StringResourceProvider.Keys.PleaseRunSpectrometerAndTryToGetSpectralMeasurements].Value);
                return;
            }

            var previousState = IsRealTimeSpectrometerData;
            IsRealTimeSpectrometerData = false;
            var dataName = StringResourceProvider.Instance[param].Value;
            var @override = false;
            var item = new ExperimentEntityDataItemViewModel();
            switch (param)
            {
                case "CurrentL":
                    if (CurrentDistance == 0)
                    {
                        await MetroDialogCoordinator.ShowMessageAsync(MainWindowDataContext,
                            StringResourceProvider.Instance[StringResourceProvider.Keys.DistanceDoesNotDefined].Value,
                            StringResourceProvider.Instance[StringResourceProvider.Keys.PleaseMoveDetectorForSomeDistance].Value);
                        IsRealTimeSpectrometerData = previousState;
                        return;
                    }
                    @override = SavedData?.DataItems != null &&
                                SavedData.DataItems.Any(x => x.Distance == CurrentDistance);
                    if (@override)
                    {
                        item = SavedData.DataItems.First(x => x.Distance == CurrentDistance);
                    }
                    else
                    {
                        item.Distance = CurrentDistance;
                        item.IsBase = BaseDistance == item.Distance;
                    }

                    dataName = $"L = {item.Distance}{(item.IsBase ? $"({StringResourceProvider.Instance[StringResourceProvider.Keys.Base]})" : "")}";
                    break;
                case "Noise":
                    @override = SavedData?.DataItems != null &&
                                SavedData.DataItems.Any(x => x.IsNoise);
                    if (@override)
                    {
                        item = SavedData.DataItems.First(x => x.IsNoise);
                    }
                    else
                    {
                        item.Distance = 0;//CurrentDistance;
                        item.IsNoise = true;
                    }
                    break;
                case "Normalize":
                    @override = SavedData?.DataItems != null &&
                                SavedData.DataItems.Any(x => x.IsNormalize);
                    if (@override)
                    {
                        item = SavedData.DataItems.First(x => x.IsNormalize);
                    }
                    else
                    {
                        item.Distance = 0;//CurrentDistance;
                        item.IsNormalize = true;
                    }


                    break;
                default:
                    throw new InvalidEnumArgumentException($"'{param}' is undefined value");
            }

            if (@override)
            {
                //MessageDialogResult result = await this.MetroDialogCoordinator.ShowMessageAsync(MainWindowDataContext, "Data already exist", $"Do you wont to replace {dataName}?", MessageDialogStyle.AffirmativeAndNegative);
                MessageDialogResult result = await MetroDialogCoordinator.ShowMessageAsync(MainWindowDataContext,
                            StringResourceProvider.Instance[StringResourceProvider.Keys.DataAlreadyExist].Value,
                            $"{StringResourceProvider.Instance[StringResourceProvider.Keys.DoYouWantToReplace].Value} '{dataName}'?",
                            MessageDialogStyle.AffirmativeAndNegative);

                if (result == MessageDialogResult.Negative)
                {
                    IsRealTimeSpectrometerData = previousState;
                    return;
                }

            }

            item.IntensityArray = Values.Select(x => x.Intencity).ToArray();
            item.IsAppliedNormalizing = IsNormalize;
            item.IsNoiseRemoved = IsNoiseRemove;

            if (!@override)
            {
                if (SavedData?.DataItems != null)
                {
                    SavedData.DataItems.Add(item);
                }
                else if (SavedData == null)
                {
                    SavedData = new ExperimentEntityDataViewModel()
                    {
                        WaveLengthArray = Values.Select(x => x.WaveLength).ToArray(),
                        DataItems = new FullyObservableCollection<ExperimentEntityDataItemViewModel>()
                        {
                            item
                        }
                    };
                }
                else if (SavedData.DataItems == null)
                {
                    SavedData.WaveLengthArray = Values.Select(x => x.WaveLength).ToArray();
                    SavedData.DataItems = new FullyObservableCollection<ExperimentEntityDataItemViewModel>()
                    {
                        item
                    };
                }
            }
            SetDataToHelper();
            NotifyPropertyChanged(nameof(IsShowSavedDataEnabled));
            IsRealTimeSpectrometerData = previousState;
        }

        public void RestartStepper(bool isForce = true)
        {
            if (Stepper == null)
            {
                Stepper = StepperManager.Instance.GetDevice(SpectrometerName);
                Logger.Info(StringResourceProvider.Instance[StringResourceProvider.Keys.StepperAssigned]);
            }
            Logger.Info($"{StringResourceProvider.Instance[StringResourceProvider.Keys.StepperIsRestarting]} ...");
            if (_stepper != null && (isForce || !_stepper.IsStepperReady))
            {
                _stepper.DistanceChanged -= StepperOnDistanceChanged;
                _stepper.DistanceChanged += StepperOnDistanceChanged;

                _stepper.StatusChanged -= StepperOnStatusChanged;
                _stepper.StatusChanged += StepperOnStatusChanged;

                Task.Run(() => _stepper.Start());
            }
        }



        public async void StopStepper(bool showWait = true)
        {
            _stepper.DistanceChanged -= StepperOnDistanceChanged;
            _stepper.StatusChanged -= StepperOnStatusChanged;
            if (showWait)
            {
                WaitMessage = $"{StringResourceProvider.Instance[StringResourceProvider.Keys.StepperIsStopping]} ...";
                IsWait = true;
            }
            await Task.Run(() =>
            {
                _stepper.Stop();

            });
            //WaitMessage = null;
            if (showWait)
            {
                IsWait = false;
            }
            Logger.Info($"{StringResourceProvider.Instance[StringResourceProvider.Keys.StepperIsStopping]} ...");
        }

        private void StepperOnDistanceChanged(object sender, StepperDistanceChangedEventHandlerArgs args)
        {
            Logger.Info(StringResourceProvider.Instance[StringResourceProvider.Keys.CurrentDistanceIs_PLACE_Mm].Value, _stepper.CurrentDistance);
            CallThreadSafe(() =>
            {
                //WaitMessage = null;
                if(!_isAutamaticMeasurements) IsWait = false;
                CurrentDistance = _stepper.CurrentDistance;
            });

        }

        private void StepperOnStatusChanged(object sender, StepperStatusChangedEventHandlerArgs args)
        {
            var stepper = ((IStepper)sender);
            if (args.IsStepperReady)
            {
                //_spectrometer.StatusChanged -= SpectrometerStatusChanged;

                CallThreadSafe(() => IsWait = _isAutamaticMeasurements && IsWait);
            }
            else
            {
                Task.Run(() =>
                {
                    CallThreadSafe(() => WaitMessage = StringResourceProvider.Instance[StringResourceProvider.Keys.StepperFailedSeeLogForDetails].Value);
                    Thread.Sleep(3000);
                    CallThreadSafe(() => IsWait = _isAutamaticMeasurements && IsWait) ;
                });
            }
        }



        public async void StopSpectrometer(bool showWait = true)
        {
            IsRealTimeSpectrometerData = false;
            if (_spectrometer != null)
            {
                _spectrometer.StatusChanged -= SpectrometerStatusChanged;
                _spectrometer.DataChanged -= SpectrometerDataChanged;
                if (showWait)
                {
                    WaitMessage =
                        $"{StringResourceProvider.Instance[StringResourceProvider.Keys.SpectrometerIsStopping]} ...";
                    IsWait = true;
                }
                await Task.Run(() =>
                {
                    _spectrometer.Stop();

                });
                //WaitMessage = null;
                if (showWait)
                {
                    IsWait = false;
                }
                Logger.Info($"{StringResourceProvider.Instance[StringResourceProvider.Keys.SpectrometerIsStopping]} ...");
            }
        }

        public void RestartSpectrometer(bool isForce = true)
        {
            if (Spectrometer == null && !string.IsNullOrEmpty(SpectrometerName))
            {
                if (Spectrometer == null)
                {
                    Spectrometer = SpectrometerManager.Instance.GetDevice(SpectrometerName);
                    Logger.Info(StringResourceProvider.Instance[StringResourceProvider.Keys.SpectrometerAssigned]);
                }
            }
            if (_spectrometer != null && (isForce || !_spectrometer.IsSpectrometerReady))
            {
                Logger.Info($"{StringResourceProvider.Instance[StringResourceProvider.Keys.SpectrometerIsRestarting]} ...");
                _spectrometer.SetSpectralRange(WaveMin, WaveMax);
                _spectrometer.StatusChanged -= SpectrometerStatusChanged;
                _spectrometer.StatusChanged += SpectrometerStatusChanged;

                _spectrometer.DataChanged -= SpectrometerDataChanged;
                _spectrometer.DataChanged += SpectrometerDataChanged;
                WaitMessage = $"{StringResourceProvider.Instance[StringResourceProvider.Keys.ConnectToSpectrometerPleaseWait]} ...";
                IsWait = true;
                Task.Run(() =>
                {
                    _spectrometer.Start();
                    if (ExposureTime > 0)
                    {
                        if (!_spectrometer.SetExposureTime(ExposureTime))
                        {
                            Thread.Sleep(100);//time to init dll
                            if (!_spectrometer.SetExposureTime(ExposureTime))
                            {
                                var exposureTime = _spectrometer.GetExposureTime();
                                if (exposureTime > 0)
                                    ExposureTime = exposureTime;
                            }
                        }
                    }
                    //Logger.Info("Spectrometer restarted");
                });
            }
        }

        private async void RetrieveParams()
        {
            //todo validation SavedData
            var spectralPointArray = SpectralPointHelper.ToSpectralPoints(SavedData, PhysicModel.GetWavelengthArray());
            WaitMessage = $"{StringResourceProvider.Instance[StringResourceProvider.Keys.RetrieveParamsPleaseWait]} ...";
            IsWait = true;
            var @params = await Task.Run(() =>
            {
                Task.Delay(1000);
                return PhysicModel.GetParams(spectralPointArray);
                
            });

            if (!_isAutamaticMeasurements) IsWait = false;

            RetrievedParams = @params;


        }



        private void SpectrometerDataChanged(object sender, SpectrometerDataChangedEventHandlerArgs args)
        {
            if (!_isRealTimeSpectrometerData || Spectrometer == null || !Spectrometer.IsSpectrometerReady) return;
            try
            {
                //_spectrometer.DataChanged -= SpectrometerDataChanged;
                var spectrometerDataPoints = args.Data;//IsLedEnabled ? Spectrometer.GetData() : Spectrometer.GetNoise();
                spectrometerDataPoints = _dataHelper.Process(spectrometerDataPoints);
                CallThreadSafe(() =>
                {
                    Values = spectrometerDataPoints.ToList();
                    AutoResize();
                });
            }
            catch (Exception ex)
            {
            }
        }

        private void SpectrometerStatusChanged(object sender, SpectrometerStatusChangedEventHandlerArgs args)
        {
            var spectrometer = ((ISpectrometer)sender);
            if (args.IsSpectrometerReady)
            {
                //_spectrometer.StatusChanged -= SpectrometerStatusChanged;

                CallThreadSafe(() =>
                {
                    if(!_isAutamaticMeasurements) IsWait = false;
                    CcdLevels = spectrometer.CcdLevels;
                    SetCcdLevelsCollectionValues();
                });
            }
            else
            {
                Task.Run(() =>
                {
                    Logger.Error(spectrometer.LastError);
                    CallThreadSafe(() =>
                    {
                        WaitMessage = StringResourceProvider.Instance[StringResourceProvider.Keys.SpectrometerFailedSeeLogForDetails].Value;
                    });
                    Thread.Sleep(3000);

                    CallThreadSafe(() =>
                    {
                        if (!_isAutamaticMeasurements) IsWait = false;
                        IsRealTimeSpectrometerData = false;
                        Values = null;
                    });

                });
            }


        }

        private void SetCcdLevelsCollectionValues()
        {
            if (CcdLevelsCollection == null)
            {
                CcdLevelsCollection = new ObservableCollection<int>();
            }
            else
            {
                CcdLevelsCollection.Clear();
            }
            int max = CcdLevels > 0 ? CcdLevels : (_spectrometer?.CcdLevels ?? 2048);
            for (int i = 128; i < max; i *= 2)
            {
                CcdLevelsCollection.Add(i);
            }
            CcdLevelsCollection.Add(max);
            CcdLevels = max;
            //CcdLevelsCollection.Add(CcdLevels);
        }

        private volatile int _autoSizeCounter;
        private volatile int _previousPow2;
        public void AutoResize()
        {
            if (!IsAutoResize || Values == null) return;

            var max = Values.Select(x => x.Intencity).DefaultIfEmpty(0).Max();
            if (max <= 0) return;

            var power2 = (int)Math.Pow(2, Math.Ceiling(Math.Log(max, 2)));

            if (power2 > 131072)//2^17
            {
                power2 = 131072;
            }

            if (power2 == CcdLevels) return;

            if (_previousPow2 != power2)
            {
                _previousPow2 = power2;
                _autoSizeCounter = 0;
            }

            _autoSizeCounter++;

            if (_autoSizeCounter > 2) //magic constant
            {
                CcdLevels = power2;
            }


        }

        public IEnumerable<Params> RetrievedParams
        {
            get { return _retrievedParams; }
            set
            {
                _retrievedParams = value;
                NotifyPropertyChanged(nameof(RetrievedParams));
            }
        }


        private int _exposureTime;

        public int ExposureTime
        {
            get { return _exposureTime; }
            set
            {
                _exposureTime = value;
                NotifyPropertyChanged(nameof(ExposureTime));
            }
        }

        private ObservableCollection<int> _ccdLevelsCollection;
        public ObservableCollection<int> CcdLevelsCollection
        {
            get { return _ccdLevelsCollection; }
            set
            {
                _ccdLevelsCollection = value;
                NotifyPropertyChanged(nameof(CcdLevelsCollection));
            }
        }

        private int _ccdLevels;
        public int CcdLevels
        {
            get { return _ccdLevels; }
            set
            {
                _ccdLevels = value;

                NotifyPropertyChanged(nameof(CcdLevels));
            }
        }

        private bool _isLedEnabled;
        public bool IsLedEnabled
        {
            get { return _isLedEnabled; }
            set
            {
                _isLedEnabled = value;
                NotifyPropertyChanged(nameof(IsLedEnabled));
            }
        }

        private bool _isRealTimeSpectrometerData;
        public bool IsRealTimeSpectrometerData
        {
            get { return _isRealTimeSpectrometerData; }
            set
            {
                _isRealTimeSpectrometerData = value;

                if (IsShowSavedData && _isRealTimeSpectrometerData)
                {
                    IsShowSavedData = false;
                }
                NotifyPropertyChanged(nameof(IsRealTimeSpectrometerData));
                NotifyPropertyChanged(nameof(IsNormalizeEnabled));
                NotifyPropertyChanged(nameof(IsNoiseRemoveEnabled));

                if (_isRealTimeSpectrometerData && (_spectrometer == null || !_spectrometer.IsSpectrometerReady))
                {
                    RestartSpectrometer();
                }
            }
        }


        private ISpectrometer _spectrometer;
        public ISpectrometer Spectrometer
        {
            get { return _spectrometer; }
            set
            {
                _spectrometer = value;
                //RestartSpectrometer(isForce: false);
                NotifyPropertyChanged(nameof(Spectrometer));
            }
        }

        private IStepper _stepper;
        public IStepper Stepper
        {
            get { return _stepper; }
            set
            {
                _stepper = value;
                //RestartSpectrometer(isForce: false);
                NotifyPropertyChanged(nameof(Stepper));
            }
        }


        private double _currentDistance;
        public double CurrentDistance
        {
            get { return _currentDistance; }
            set
            {
                _currentDistance = value;
                NotifyPropertyChanged(nameof(CurrentDistance));
            }
        }



        private ExperimentEntityDataViewModel _savedData;
        public ExperimentEntityDataViewModel SavedData
        {
            get { return _savedData; }
            set
            {
                if (_savedData != null)
                {
                    _savedData.PropertyChanged -= SavedDataOnPropertyChanged;
                    if (_savedData.DataItems != null)
                    {
                        _savedData.DataItems.CollectionChanged -= DataItemsOnCollectionChanged;
                        _savedData.DataItems.ItemPropertyChanged -= DataItemOnCollectionChanged;
                    }
                }
                _savedData = value;


                if (_savedData != null)
                {
                    _savedData.PropertyChanged += SavedDataOnPropertyChanged;
                    if (_savedData.DataItems != null)
                    {
                        _savedData.DataItems.CollectionChanged += DataItemsOnCollectionChanged;
                        _savedData.DataItems.ItemPropertyChanged += DataItemOnCollectionChanged;
                    }
                }

                SetDataToHelper();

                UpdateNestedFieldFromSavedData();

                NotifyPropertyChanged(nameof(SavedData));
            }
        }

        private void DataItemOnCollectionChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            //todo more properties should be filtered
            if (e.PropertyName != "IsShow")
                SetDataToHelper();
        }

        private void SetDataToHelper()
        {
            if (_savedData?.DataItems != null)
            {
                if (_savedData.DataItems.Any(x => x.IsNoise))
                {
                    _dataHelper.NoiseArray = _savedData.DataItems.First(x => x.IsNoise).IntensityArray;
                }

                if (_savedData.DataItems.Any(x => x.IsNormalize))
                {
                    _dataHelper.NormalizingArray = _savedData.DataItems.First(x => x.IsNormalize).IntensityArray;
                }
            }
        }

        private void DataItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetDataToHelper();
        }

        private void SavedDataOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetDataToHelper();
        }


        private void UpdateNestedFieldFromSavedData()
        {
            NotifyPropertyChanged(nameof(IsShowSavedDataEnabled));
            NotifyPropertyChanged(nameof(IsDivideToBaseEnabled));

            NotifyPropertyChanged(nameof(IsLogarithmicScaleEnabled));

        }

        private List<SpectrometerDataPoint> _values;
        public List<SpectrometerDataPoint> Values
        {
            get { return _values; }
            set
            {
                _values = value;
                NotifyPropertyChanged(nameof(Values));
            }
        }

        private IEnumerable<double> _xValues;
        public IEnumerable<double> XValues
        {
            get { return _xValues; }
            set
            {
                _xValues = value;
                NotifyPropertyChanged(nameof(XValues));
            }
        }


        private bool _isShowSavedData;
        public bool IsShowSavedData
        {
            get { return _isShowSavedData; }
            set
            {
                _isShowSavedData = value;
                if (_isShowSavedData && IsRealTimeSpectrometerData)
                {
                    IsRealTimeSpectrometerData = false;
                }
                NotifyPropertyChanged(nameof(IsShowSavedData));
                NotifyPropertyChanged(nameof(IsLogarithmicScaleEnabled));
                UpdateNestedFieldFromSavedData();
            }
        }
        public bool IsShowSavedDataEnabled => SavedData?.DataItems != null && SavedData.DataItems.Count > 0;


        private bool _isLogarithmicScale;
        public bool IsLogarithmicScale
        {
            get { return _isLogarithmicScale; }
            set
            {
                _isLogarithmicScale = value;
                NotifyPropertyChanged(nameof(IsLogarithmicScale));
            }
        }

        public bool IsLogarithmicScaleEnabled => IsShowSavedDataEnabled && IsShowSavedData;

        private bool _isDivideToBase;
        public bool IsDivideToBase
        {
            get { return _isDivideToBase; }
            set
            {
                _isDivideToBase = value;
                NotifyPropertyChanged(nameof(IsDivideToBase));
            }
        }
        public bool IsDivideToBaseEnabled => IsShowSavedDataEnabled && IsShowSavedData && SavedData.DataItems.Any(x => x.IsBase);

        private bool _isNormalize;
        public bool IsNormalize
        {
            get { return _isNormalize; }
            set
            {
                _isNormalize = value;
                _dataHelper.IsUseNormalizingArray = _isNormalize;
                SetDataToHelper();
                NotifyPropertyChanged(nameof(IsNormalize));
            }
        }
        public bool IsNormalizeEnabled => IsRealTimeSpectrometerData && SavedData?.DataItems != null && SavedData.DataItems.Any(x => x.IsNormalize);

        private bool _isNoiseRemove;



        public bool IsNoiseRemove
        {
            get { return _isNoiseRemove; }
            set
            {
                _isNoiseRemove = value;
                _dataHelper.IsUseNoiseArray = _isNoiseRemove;
                SetDataToHelper();
                NotifyPropertyChanged(nameof(IsNoiseRemove));
            }
        }
        public bool IsNoiseRemoveEnabled => IsRealTimeSpectrometerData && SavedData?.DataItems != null && SavedData.DataItems.Any(x => x.IsNoise);


        private bool _isAutoResize;
        private IEnumerable<Params> _retrievedParams;


        public bool IsAutoResize
        {
            get { return _isAutoResize; }
            set
            {
                _isAutoResize = value;
                NotifyPropertyChanged(nameof(IsAutoResize));
            }
        }

        public void OnPopulateStringResourceProvider()
        {
            NotifyPropertyChanged(nameof(ExposureTime));
        }
    }
}
