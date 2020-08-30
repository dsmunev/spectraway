using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SpectraWay.Converter
{
    public abstract class BoolToTConverter<T> : IValueConverter
    {
        public abstract T IsTrue { get; set; }
        public abstract T IsFalse { get; set; }
        public abstract T IsNull { get; set; }
        public abstract T Default { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is bool)
                {
                    return (bool)value ? IsTrue : IsFalse;
                }
                if (value is int)
                {
                    return (int) value > 0 ? IsTrue : IsFalse;
                }
                var s = value as string;
                if (s != null)
                {
                    return s != string.Empty ? IsTrue : IsFalse;
                }
                return Default;
            }
            return IsNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class BoolToDouble : BoolToTConverter<double>
    {
        public override double IsTrue { get; set; }
        public override double IsFalse { get; set; }
        public override double IsNull { get; set; }
        public override double Default { get; set; }
    }

    public class BoolToInt : BoolToTConverter<int>
    {
        public override int IsTrue { get; set; }
        public override int IsFalse { get; set; }
        public override int IsNull { get; set; }
        public override int Default { get; set; }
    }

    public class BoolToThickness : BoolToTConverter<Thickness>
    {
        public override Thickness IsTrue { get; set; }
        public override Thickness IsFalse { get; set; }
        public override Thickness IsNull { get; set; }
        public override Thickness Default { get; set; }
    }

    public class BoolToStyle : BoolToTConverter<Style>
    {
        public override Style IsTrue { get; set; }
        public override Style IsFalse { get; set; }
        public override Style IsNull { get; set; }
        public override Style Default { get; set; }
    }

    public class BoolToBool : BoolToTConverter<bool?>
    {
        public override bool? IsTrue { get; set; }
        public override bool? IsFalse { get; set; }
        public override bool? IsNull { get; set; }
        public override bool? Default { get; set; }
    }


    public class MultiInvertBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values?.Length == 3)
            {
                var first = values[0] != DependencyProperty.UnsetValue && ((bool?) values[0] ?? false);
                var second = values[0] != DependencyProperty.UnsetValue && ((bool?) values[1] ?? false);
                var third = values[0] != DependencyProperty.UnsetValue && ((bool?) values[2] ?? false);
                return (first && (second || third)) ? Visibility.Hidden: Visibility.Visible;
            }
            var returnValue = true;
            foreach (object value in values)
            {
                returnValue = returnValue && value != DependencyProperty.UnsetValue && ((bool?) value ?? false);

            }
            return returnValue ? Visibility.Hidden: Visibility.Visible;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


}
