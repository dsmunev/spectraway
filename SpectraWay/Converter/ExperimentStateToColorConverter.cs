using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using SpectraWay.DataProvider.Entities;
using SpectraWay.ViewModel;

namespace SpectraWay.Converter
{
    public class ExperimentStateToColorConverter : IValueConverter
    {
        private Brush gray = new SolidColorBrush(Colors.Gray);
        private Brush green = new SolidColorBrush(Colors.Green);
        private Brush red = new SolidColorBrush(Colors.Red);
        private Brush yellow = new SolidColorBrush(Colors.Yellow);
        private Brush blue = new SolidColorBrush(Colors.Blue);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ExperimentStatus))
                return gray;
            if(parameter == null || parameter.ToString().ToLower() == "color")
                return GetColor(value);
            if (parameter.ToString().ToLower() == "icon")
                return GetIcon(value);
            return null;
        }

        private object GetColor(object value)
        {
            switch ((ExperimentStatus) value)
            {
                case ExperimentStatus.Unknown:
                    return gray;
                case ExperimentStatus.Success:
                    return green;
                case ExperimentStatus.Failed:
                    return red;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        private object GetIcon(object value)
        {
            switch ((ExperimentStatus)value)
            {
                case ExperimentStatus.Unknown:
                    return "InformationOutline";
                case ExperimentStatus.Success:
                    return "CheckCircleOutline";
                case ExperimentStatus.Failed:
                    return "CloseCircleOutline";
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
