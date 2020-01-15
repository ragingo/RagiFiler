using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace RagiFiler.Views.Controls
{
    public partial class FilePreview : UserControl
    {
        private MediaElement _mediaElement;
        private Slider _slider;
        private DispatcherTimer _timer;
        private bool _isDragging;

        public FilePreview()
        {
            InitializeComponent();

            _timer = new DispatcherTimer(DispatcherPriority.Normal);
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_isDragging)
            {
                return;
            }

            _slider.Value = _mediaElement.Position.TotalSeconds;
        }

        private void OnMediaElementMediaOpened(object sender, RoutedEventArgs e)
        {
            if (!(sender is MediaElement mediaElement))
            {
                return;
            }

            _mediaElement = mediaElement;

            var parent = mediaElement.Parent;

            _slider = parent.GetChildren().OfType<Slider>().FirstOrDefault();
            if (_slider != null)
            {
                _slider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                _slider.Value = 0d;
            }

            _timer.Start();
        }

        private void OnMediaElementMediaEnded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private void OnMediaElementMediaFailed(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private void OnThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;
        }

        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;
            _mediaElement.Position = TimeSpan.FromSeconds(_slider.Value);
        }
    }

}
