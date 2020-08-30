using System;
using System.Globalization;
using System.Windows.Data;
using SpectraWay.ViewModel.Experiment;

namespace SpectraWay.Converter
{
    public class ExperimentEntityDataItemToDesc : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ExperimentEntityDataItemViewModel)) return null;
            var viewModel = (ExperimentEntityDataItemViewModel) value;
            if (viewModel.IsNoise) return "Noise";
            if (viewModel.IsNormalize) return "Normalize";
            var str = $"L = {viewModel.Distance}";
            if (viewModel.IsBase) str = $"{str}(Base)";
            return str;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}