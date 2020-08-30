using System;
using System.Globalization;
using System.Windows.Data;
using SpectraWay.Localization;

namespace SpectraWay.Converter
{
    public class ExposureTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format(StringResourceProvider.Instance["EnterToSaveNowIsMs"].Value, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}