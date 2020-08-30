using System;
using System.Collections.ObjectModel;
using System.Linq;
using SpectraWay.DataProvider.Entities;
using SpectraWay.ParamsRetriever;

namespace SpectraWay.ViewModel.Experiment
{
    public class ExperimentTileViewModel : BaseViewModel
    {
        private string _category;

        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
                NotifyPropertyChanged(nameof(Category));
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        private DateTime _dateTime;

        public DateTime DateTime
        {
            get { return _dateTime; }
            set
            {
                _dateTime = value;
                NotifyPropertyChanged(nameof(DateTime));
            }
        }

        private ExperimentStatus _experimentStatus;

        public ExperimentStatus ExperimentStatus
        {
            get { return _experimentStatus; }
            set
            {
                _experimentStatus = value;
                NotifyPropertyChanged(nameof(ExperimentStatus));
            }
        }

        private string _spectrometerName;

        public string SpectrometerName
        {
            get { return _spectrometerName; }
            set
            {
                _spectrometerName = value;
                NotifyPropertyChanged(nameof(SpectrometerName));
            }
        }

        private ParamsRetrieverBase _physicModel;

        public ParamsRetrieverBase PhysicModel
        {
            get { return _physicModel; }
            set
            {
                _physicModel = value;
                if (_physicModel != null)
                {
                    WaveMin = _physicModel.GetWavelengthArray().DefaultIfEmpty(0).Min()-5;
                    WaveMax = _physicModel.GetWavelengthArray().DefaultIfEmpty(0).Max()+5;
                    DistanceRange = new ObservableCollection<double>(_physicModel.GetDistancesArray());
                    BaseDistance = _physicModel.GetDistancesArray().DefaultIfEmpty(0).First();
                }
                NotifyPropertyChanged(nameof(PhysicModel));
            }
        }


        private  double _waveMin;

        public double WaveMin
        {
            get { return _waveMin; }
            set
            {
                _waveMin = value;
                NotifyPropertyChanged(nameof(WaveMin));
            }
        }

        private double _waveMax;

        public double WaveMax
        {
            get { return _waveMax; }
            set
            {
                _waveMax = value;
                NotifyPropertyChanged(nameof(WaveMax));
            }
        }

        private ObservableCollection<double> _distanceRange;

        public ObservableCollection<double> DistanceRange
        {
            get { return _distanceRange; }
            set
            {
                _distanceRange = value;
                NotifyPropertyChanged(nameof(DistanceRange));
            }
        }

        private double _baseDistance;

        public double BaseDistance
        {
            get { return _baseDistance; }
            set
            {
                _baseDistance = value;
                NotifyPropertyChanged(nameof(BaseDistance));
            }
        }




    }



}
