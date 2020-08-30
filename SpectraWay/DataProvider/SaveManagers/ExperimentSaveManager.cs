using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SpectraWay.DataProvider.Entities;
using SpectraWay.Extension;
using SpectraWay.Helpers;
using SpectraWay.ViewModel.Experiment;

namespace SpectraWay.DataProvider.SaveManagers
{
    //TODO UNSUBSCRIBE FOR ALL EVENTS!!!
    public class ExperimentSaveManager: ISaveManager<ExperimentEntity>
    {

        public ExperimentSaveManager()
        {
            _settings.Converters.Add(new ArrayNoFormattingConverter());
        }

        private class Wrapper
        {
            public ExperimentEntity Entity { get; set; }
            public bool IsReadyToSave { get; set; }
        }
        private readonly Dictionary<INotifyPropertyChanged, Wrapper> _mapper = new Dictionary<INotifyPropertyChanged, Wrapper>();


        public event ReadyToSaveChangedEventHandler ReadyToSaveChanged;

        protected void NotifyReadyToSaveChanged()
        {
            var e = new ReadyToSaveChangedEventHandlerArgs(IsReadyToSave);
            ReadyToSaveChanged?.Invoke(this, e);
        }

        public void Map(INotifyPropertyChanged viewModel, ExperimentEntity entity = null)
        {
            if (_mapper.ContainsKey(viewModel)) return;
            if(!(viewModel is ExperimentViewModel)) return;
            

            var experimentViewModel = (ExperimentViewModel) viewModel;
            var currentEntity = entity ?? experimentViewModel.ToEntity();

            experimentViewModel.PropertyChanged += ExperimentViewModelOnPropertyChanged;
            DistanceRangeCollectionChangeSubscribe(experimentViewModel);
            SavedDataPropertyChangedSubscribe(experimentViewModel);

            DataItemsChangedSubscribe(experimentViewModel);
            if (_mapper.ContainsKey(viewModel))
            {
                _mapper[viewModel].Entity = currentEntity;
                _mapper[viewModel].IsReadyToSave = entity == null;
            }
            else
            {
                _mapper.Add(viewModel, new Wrapper { Entity = currentEntity, IsReadyToSave = entity == null });
            }
            NotifyReadyToSaveChanged();

        }

        private void DistanceRangeCollectionChangeSubscribe(ExperimentViewModel experimentViewModel)
        {
            if (experimentViewModel.DistanceRange != null)
                experimentViewModel.DistanceRange.CollectionChanged += (o, e) => DistanceRangeOnCollectionChanged(o, e, experimentViewModel);
        }

        private void DistanceRangeOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs, ExperimentViewModel experimentViewModel)
        {
            SetReady(experimentViewModel);
        }

        public bool SaveAll()
        {
            var saveAll = _mapper.Aggregate(false, (current, pair) => current || Save(pair.Key));
            NotifyReadyToSaveChanged();
            return saveAll;
        }

        private JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
        };

        private bool Save(ExperimentViewModel viewModel, bool isRaiseEvent = false)
        {
            if(!_mapper.ContainsKey(viewModel)) return false;

            if (!_mapper[viewModel].IsReadyToSave) return false;
            try
            {
                var experimentsDirectory = "experiments";
                if (!Directory.Exists(experimentsDirectory))
                {
                    Directory.CreateDirectory(experimentsDirectory);
                }
                var oldEntity = _mapper[viewModel].Entity;
                var newEntity = viewModel.ToEntity();
                if (oldEntity.Name != newEntity.Name)
                {
                    var oldPath = $"{experimentsDirectory}\\{oldEntity.Name.ToFileName()}.json";
                    if (File.Exists(oldPath))
                    {
                        File.Delete(oldPath);
                    }
                }
                var newPath = $"{experimentsDirectory}\\{newEntity.Name.ToFileName()}.json";
                //if (File.Exists(newPath))
                //{
                //    newPath = $"{experimentsDirectory}\\{newEntity.Name}_{Guid.NewGuid()}.json";
                //}
                File.WriteAllText(newPath, JsonConvert.SerializeObject(newEntity, _settings));
                _mapper[viewModel].Entity = newEntity;
                _mapper[viewModel].IsReadyToSave = false;
                if(isRaiseEvent) NotifyReadyToSaveChanged();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Save(INotifyPropertyChanged viewModel)
        {
            return Save(viewModel as ExperimentViewModel, isRaiseEvent:true);
        }

        public bool IsReadyToSave
        {
            get
            {
                return _mapper.Any(pair => pair.Value.IsReadyToSave);
            }
        }

        private void DataItemsChangedSubscribe(ExperimentViewModel experimentViewModel)
        {
            if (experimentViewModel.SavedData?.DataItems == null) return;
            experimentViewModel.SavedData.DataItems.ItemPropertyChanged += (o, e) => DataItemsOnItemPropertyChanged(o, e, experimentViewModel);
            experimentViewModel.SavedData.DataItems.CollectionChanged += (o, e) => DataItemsOnCollectionChanged(o, e, experimentViewModel);
        }

        private void SavedDataPropertyChangedSubscribe(ExperimentViewModel experimentViewModel)
        {
            if (experimentViewModel.SavedData == null) return ;
            experimentViewModel.SavedData.PropertyChanged += (o, e) => SavedDataOnPropertyChanged(o, e, experimentViewModel);
            DataItemsChangedSubscribe(experimentViewModel);
        }

        private void DataItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs, ExperimentViewModel experimentViewModel)
        {
            SetReady(experimentViewModel);
        }

        private void DataItemsOnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs itemPropertyChangedEventArgs, ExperimentViewModel experimentViewModel)
        {
            SetReady(experimentViewModel);
        }

        private void SavedDataOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs, ExperimentViewModel experimentViewModel)
        {
            if (propertyChangedEventArgs.PropertyName == "DataItems")
            {
                DataItemsChangedSubscribe(experimentViewModel);
            }
            SetReady(experimentViewModel);
        }

        private void ExperimentViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if(!_allowExperimentViewModelProperties.Contains(propertyChangedEventArgs.PropertyName)) return;

            var experimentViewModel = (ExperimentViewModel)sender;
            if (propertyChangedEventArgs.PropertyName == "SavedData")
            {
                SavedDataPropertyChangedSubscribe(experimentViewModel);
                DataItemsChangedSubscribe(experimentViewModel);
            }
            if (propertyChangedEventArgs.PropertyName == "DistanceRange")
            {
                DistanceRangeCollectionChangeSubscribe(experimentViewModel);
            }
            SetReady(experimentViewModel);
        }

        private void SetReady(ExperimentViewModel experimentViewModel, bool isReady = true)
        {
            if (_mapper.ContainsKey(experimentViewModel))
            {
                _mapper[experimentViewModel].IsReadyToSave = isReady;
            }
            NotifyReadyToSaveChanged();
        }

        private readonly string[] _allowExperimentViewModelProperties = 
                            {
                                "DateTime",
                                "BaseDistance",
                                "Category",
                                "DistanceRange",
                                "ExperimentStatus",
                                "Name",
                                "PhysicModel",
                                "SpectrometerName",
                                "WaveMax",
                                "WaveMin",
                                "SavedData",
                            };
    }
}