using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SpectraWay.DataProvider;
using SpectraWay.DataProvider.Entities;
using SpectraWay.ParamsRetriever;
using SpectraWay.ViewModel;
using SpectraWay.ViewModel.Experiment;

namespace SpectraWay.Extension
{
    public static class ViewModelExtension
    {

        public static ExperimentViewModel ToViewModel([NotNull] this ExperimentEntity experimentEntity)
        {
            var experimentViewModel = new ExperimentViewModel
            {
                DateTime = experimentEntity.DateTime,
                BaseDistance = experimentEntity.BaseDistance,
                Category = experimentEntity.Category,
                DistanceRange = new ObservableCollection<double>(experimentEntity.DistanceRange),
                ExperimentStatus = experimentEntity.ExperimentStatus,
                Name = experimentEntity.Name,
                PhysicModel = experimentEntity.PhysicModel == null? null : ParamsRetrieverManager.GetParamsRetrievers().FirstOrDefault(x=> x.Name == experimentEntity.PhysicModel),
                SpectrometerName = experimentEntity.Spectrometer,
                WaveMax = experimentEntity.WaveMax,
                WaveMin = experimentEntity.WaveMin,
                RetrievedParams = (experimentEntity.RetrievedParams??new List<Params>()).ToArray(),
                SavedData = experimentEntity.Data == null? null : new ExperimentEntityDataViewModel()
                {
                    WaveLengthArray = experimentEntity.Data.WaveLengthArray,
                    DataItems = new FullyObservableCollection<ExperimentEntityDataItemViewModel>(experimentEntity.Data.DataItems.Select(x=>x.ToViewModel()))
                }
                
            };
            //todo move to other place
            EntityDataProvider<ExperimentEntity>.Instance.SaveManager.Map(experimentViewModel, experimentEntity);
            return experimentViewModel;
        }

        public static ExperimentEntityDataItemViewModel ToViewModel([NotNull] this ExperimentEntityDataItem dataItem)
        {
            return new ExperimentEntityDataItemViewModel
            {
                IsNoise = dataItem.IsNoise,
                Distance = dataItem.Distance,
                IsBase = dataItem.IsBase,
                IsNormalize = dataItem.IsNormalize,
                IntensityArray = dataItem.IntensityArray,
                IsShow = dataItem.IsShow,
                IsAppliedNormalizing = dataItem.IsAppliedNormalizing,
                IsNoiseRemoved = dataItem.IsNoiseRemoved
            };
        }


        public static ExperimentEntity ToEntity([NotNull] this ExperimentViewModel experimentVm)
        {
            return new ExperimentEntity
            {
                DateTime = experimentVm.DateTime,
                BaseDistance = experimentVm.BaseDistance,
                Category = experimentVm.Category,
                DistanceRange = experimentVm.DistanceRange.ToList(),
                ExperimentStatus = experimentVm.ExperimentStatus,
                Name = experimentVm.Name,
                PhysicModel = experimentVm.PhysicModel.Name,
                RetrievedParams = (experimentVm.RetrievedParams ?? new List<Params>()).ToArray(),
                Spectrometer = experimentVm.SpectrometerName,
                WaveMax = experimentVm.WaveMax,
                WaveMin = experimentVm.WaveMin,
                Data = experimentVm.SavedData == null ? null : new ExperimentEntityData()
                {
                    WaveLengthArray = experimentVm.SavedData.WaveLengthArray,
                    DataItems = experimentVm.SavedData.DataItems?.Select(x => x.ToEntity())
                }
            };
        }

        public static ExperimentEntityDataItem ToEntity([NotNull] this ExperimentEntityDataItemViewModel vm)
        {
            return new ExperimentEntityDataItem
            {
                IsNoise = vm.IsNoise,
                Distance = vm.Distance,
                IsBase = vm.IsBase,
                IsNormalize = vm.IsNormalize,
                IntensityArray = vm.IntensityArray,
                IsShow = vm.IsShow,
                IsAppliedNormalizing = vm.IsAppliedNormalizing,
                IsNoiseRemoved = vm.IsNoiseRemoved
            };
        }
    }
}
