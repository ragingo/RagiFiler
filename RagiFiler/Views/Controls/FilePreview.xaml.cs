using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RagiFiler.Views.Controls
{
    public partial class FilePreview : UserControl
    {
        public FilePreview()
        {
            InitializeComponent();
        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (!(sender is MediaElement mediaElement))
            {
                return;
            }
        }
    }

    [ValueConversion(typeof(Duration), typeof(string))]
    class DurationToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Duration duration))
            {
                return null;
            }

            if (targetType != typeof(string))
            {
                return null;
            }

            if (!duration.HasTimeSpan)
            {
                return "00:00:00";
            }

            return duration.TimeSpan.ToString("HH:mm:ss", culture.DateTimeFormat);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
