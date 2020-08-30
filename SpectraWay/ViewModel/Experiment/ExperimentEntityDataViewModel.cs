using System.Collections.ObjectModel;
using SpectraWay.DataProvider.Entities;
using SpectraWay.Extension;

namespace SpectraWay.ViewModel.Experiment
{
    public class ExperimentEntityDataViewModel : BaseViewModel
    {
        private double[] _waveLengthArray;
        private FullyObservableCollection<ExperimentEntityDataItemViewModel> _dataItems;

        public double[] WaveLengthArray
        {
            get { return _waveLengthArray; }
            set
            {
                _waveLengthArray = value;
                NotifyPropertyChanged(nameof(WaveLengthArray));
            }
        }

        public FullyObservableCollection<ExperimentEntityDataItemViewModel> DataItems
        {
            get { return _dataItems; }
            set
            {
                _dataItems = value;
                NotifyPropertyChanged(nameof(DataItems));
            }
        }
    }
}