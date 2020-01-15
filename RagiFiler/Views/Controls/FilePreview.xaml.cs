using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

            var parent = mediaElement.Parent;

            var slider = parent.GetChildren().OfType<Slider>().FirstOrDefault();
            if (slider != null)
            {
                slider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                slider.ValueChanged += OnSliderValueChanged;
            }
        }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
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
