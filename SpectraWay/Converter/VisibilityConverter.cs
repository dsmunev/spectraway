using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SpectraWay.Converter
{
    public class VisibilityConverter : IValueConverter
    {
        public Visibility IsTrue { get; set; }
        public Visibility IsFalse { get; set; }

        public VisibilityConverter()
        {
            IsTrue = Visibility.Visible;
            IsFalse = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || parameter.ToString().ToLower() == "direct")
            {
                if (value == null)
                {
                    return IsFalse;
                }

                switch (value.GetType().FullName)
                {
                    case "System.Boolean":
                        return (bool) value ? IsTrue : IsFalse;

                    case "System.Int32":
                        return (int) value > 0 ? IsTrue : IsFalse;

                    case "System.String":
                        return !string.IsNullOrEmpty((string) value) ? IsTrue : IsFalse;

                    default:
                        return IsTrue;
                }
            }
            if (parameter.ToString().ToLower() == "inverse")
            {
                if (value == null)
                {
                    return IsTrue;
                }

                switch (value.GetType().FullName)
                {
                    case "System.Boolean":
                        return (bool)value ? IsFalse : IsTrue;

                    case "System.Int32":
                        return (int)value > 0 ? IsFalse : IsTrue;

                    case "System.String":
                        return !string.IsNullOrEmpty((string)value) ? IsFalse : IsTrue;

                    default:
                        return IsFalse;
                }
            }
            return IsTrue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class VisibilityHiddenConverter : VisibilityConverter
    {
        public VisibilityHiddenConverter()
        {
            IsTrue = Visibility.Visible;
            IsFalse = Visibility.Hidden;
        }
    }

#if !SILVERLIGHT
    public class MultiVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            foreach (object value in values)
            {
                if (value != DependencyProperty.UnsetValue && ((bool?)value ?? false))
                {
                    return true;
                }
            }
            return false;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
#endif
}
