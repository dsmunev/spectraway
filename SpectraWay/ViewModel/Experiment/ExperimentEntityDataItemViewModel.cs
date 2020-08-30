using System.Windows;
using System.Windows.Input;
using SpectraWay.Model;

namespace SpectraWay.ViewModel.Experiment
{
    public class ExperimentEntityDataItemViewModel: BaseViewModel
    {



        private bool _isNoise;
        private bool _isBase;
        private bool _isNormalize;
        private bool _isShow;
        private double _distance;
        private double[] _intensityArray;
        private bool _isAppliedNormalizing;
        private bool _isNoiseRemoved;
        private bool _isFiltred;

        public bool IsAppliedNormalizing
        {
            get { return _isAppliedNormalizing; }
            set
            {
                _isAppliedNormalizing = value;
                NotifyPropertyChanged(nameof(IsAppliedNormalizing));
            }
        }

        public bool IsNoiseRemoved
        {
            get { return _isNoiseRemoved; }
            set
            {
                _isNoiseRemoved = value; 
                NotifyPropertyChanged(nameof(IsNoiseRemoved));
            }
        }


        public bool IsNoise
        {
            get { return _isNoise; }
            set
            {
                _isNoise = value;
                NotifyPropertyChanged(nameof(IsNoise));
            }
        }
        //public bool IsFiltred { get; set; }

        public bool IsBase
        {
            get { return _isBase; }
            set
            {
                _isBase = value;
                NotifyPropertyChanged(nameof(IsBase));
            }
        }

        public bool IsNormalize
        {
            get { return _isNormalize; }
            set
            {
                _isNormalize = value;
                NotifyPropertyChanged(nameof(IsNormalize));
            }
        }

        public bool IsFiltred
        {
            get { return _isFiltred; }
            set
            {
                _isFiltred = value;
                NotifyPropertyChanged(nameof(IsFiltred));
            }
        }

        public bool IsShow
        {
            get { return _isShow; }
            set
            {
                _isShow = value;
                NotifyPropertyChanged(nameof(IsShow));
            }
        }

        public double Distance
        {
            get { return _distance; }
            set
            {
                _distance = value;
                NotifyPropertyChanged(nameof(Distance));
            }
        }

        public double[] IntensityArray
        {
            get { return _intensityArray; }
            set
            {
                _intensityArray = value;
                NotifyPropertyChanged(nameof(IntensityArray));
            }
        }
    }
}