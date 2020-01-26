using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RagiFiler.Views.Converters
{
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool b))
            {
                return Visibility.Collapsed;
            }

            if (targetType != typeof(Visibility))
            {
                return Visibility.Collapsed;
            }

            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility v))
            {
                return false;
            }

            if (targetType != typeof(bool))
            {
                return false;
            }

            return v == Visibility.Visible;
        }
    }
}
