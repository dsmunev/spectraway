using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SpectraWay.Converter
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility NullValue  {get;set;}
        public Visibility TrueValue  {get;set;}
        public Visibility FalseValue { get; set; }

        public BoolToVisibilityConverter ()
        {
            NullValue = Visibility.Collapsed;
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof (Visibility))
                throw new InvalidOperationException("Converter can only convert to value of type Visibility.");
            if (value == null)
                return NullValue;
            var visible = System.Convert.ToBoolean(value, culture);
            return visible ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
