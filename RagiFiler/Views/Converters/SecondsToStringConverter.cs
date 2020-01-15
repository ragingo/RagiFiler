using System;
using System.Globalization;
using System.Windows.Data;

namespace RagiFiler.Views.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    class SecondsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double seconds))
            {
                return null;
            }

            if (targetType != typeof(string))
            {
                return null;
            }

            try
            {
                return TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss", culture.DateTimeFormat);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
