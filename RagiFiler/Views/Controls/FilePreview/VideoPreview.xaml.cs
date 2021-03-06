﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using RagiFiler.Controls;

namespace RagiFiler.Views.Controls
{
    public partial class VideoPreview : UserControl
    {
        private readonly MediaElement _mediaElement;
        private readonly Slider _slider;
        private readonly Button _playButton;
        private readonly DispatcherTimer _timer;

        private bool _isDragging;
        private bool _isPlaying;
        private bool _isAutoPlay = true; // TODO: 直す
        private bool _isRepeatEnabled = true; // TODO: 直す

        public VideoPreview()
        {
            InitializeComponent();

            _timer = new DispatcherTimer(DispatcherPriority.Normal);
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;

            _playButton = this.FindName<Button>("playButton");
            _slider = this.FindName<Slider>("slider");

            _mediaElement = this.FindName<MediaElement>("mediaElement");
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            // TODO: こんなことしたくないから MediaElement をやめる
            if (e.Property.Name == "DataContext")
            {
                _timer.Stop();
                _mediaElement.Stop();
                _mediaElement.Close();

                base.OnPropertyChanged(e);

                if (_isAutoPlay)
                {
                    _isPlaying = true;
                    _mediaElement.Play();
                    UpdatePlayButtonContent();
                }
            }
            else
            {
                base.OnPropertyChanged(e);
            }
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
            _isPlaying = true;
            UpdatePlayButtonContent();

            _slider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            _slider.Value = 0d;
            _timer.Start();
        }

        private void OnMediaElementMediaEnded(object sender, RoutedEventArgs e)
        {
            if (_isRepeatEnabled)
            {
                _mediaElement.Position = TimeSpan.FromSeconds(0);
                _slider.Value = 0;
            }
            else
            {
                _isPlaying = false;
                UpdatePlayButtonContent();

                _mediaElement.Stop();
                _timer.Stop();
            }
        }

        private void OnMediaElementMediaFailed(object sender, RoutedEventArgs e)
        {
            _isPlaying = false;
            UpdatePlayButtonContent();

            _mediaElement.Stop();
            _timer.Stop();
        }

        private void OnThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;
            _isPlaying = false;
            UpdatePlayButtonContent();
            _mediaElement.Pause();
            _mediaElement.ScrubbingEnabled = true;
        }

        private void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            _mediaElement.Position = TimeSpan.FromSeconds(_slider.Value);
        }

        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;
            _isPlaying = true;
            UpdatePlayButtonContent();
            _mediaElement.ScrubbingEnabled = false;
            _mediaElement.Play();
        }

        private void OnPlayButtonClick(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                _isPlaying = false;
                _mediaElement.Pause();
            }
            else
            {
                _isPlaying = true;
                _mediaElement.Play();
            }

            UpdatePlayButtonContent();
        }

        private void UpdatePlayButtonContent()
        {
            if (_isPlaying)
            {
                _playButton.Content = new PackIcon { Kind = PackIconKind.Pause };
            }
            else
            {
                _playButton.Content = new PackIcon { Kind = PackIconKind.Play };
            }
        }
    }
}
