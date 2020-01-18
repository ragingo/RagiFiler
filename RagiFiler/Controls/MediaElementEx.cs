using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RagiFiler.Controls
{
    // TODO: uwp で作ったやつを wpf 向けに移植していく
    class MediaElementEx : UserControl
    {
        private DrawingBrush _drawingBrush;
        private VideoDrawing _videoDrawing;
        private MediaPlayer _mediaPlayer;

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(object), typeof(MediaElementEx), new PropertyMetadata(null, OnSourceChanged));

        /// <summary>
        /// 映像ソース
        /// </summary>
        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty CurrentPositionProperty =
            DependencyProperty.Register(nameof(CurrentPosition), typeof(TimeSpan), typeof(MediaElementEx), new PropertyMetadata(TimeSpan.Zero, OnPositionChanged));

        /// <summary>
        /// 現在の再生位置
        /// </summary>
        public TimeSpan CurrentPosition
        {
            get { return (TimeSpan)GetValue(CurrentPositionProperty); }
            set { SetValue(CurrentPositionProperty, value); }
        }

        public static readonly DependencyProperty InitialPositionProperty =
            DependencyProperty.Register(nameof(InitialPosition), typeof(TimeSpan), typeof(MediaElementEx), new PropertyMetadata(TimeSpan.Zero, OnPositionChanged));

        /// <summary>
        /// 最初の再生位置
        /// </summary>
        public TimeSpan InitialPosition
        {
            get { return (TimeSpan)GetValue(InitialPositionProperty); }
            set { SetValue(InitialPositionProperty, value); }
        }

        /// <summary>
        /// 動画長
        /// </summary>
        public Duration NaturalDuration
        {
            get { return _mediaPlayer.NaturalDuration; }
        }

        public static readonly DependencyProperty PosterSourceProperty =
            DependencyProperty.Register(nameof(PosterSource), typeof(ImageSource), typeof(MediaElementEx), new PropertyMetadata(null));

        /// <summary>
        /// 再生準備段階で表示される画像のソース
        /// </summary>
        public ImageSource PosterSource
        {
            get { return GetValue(PosterSourceProperty) as ImageSource; }
            set { SetValue(PosterSourceProperty, value); }
        }

        public MediaElementEx()
        {
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.BufferingStarted += OnBufferingStarted;
            _mediaPlayer.BufferingEnded += OnBufferingEnded;
            _mediaPlayer.MediaOpened += OnMediaOpened;
            _mediaPlayer.MediaEnded += OnMediaEnded;
            _mediaPlayer.MediaFailed += OnMediaFailed;

            _videoDrawing = new VideoDrawing();
            _videoDrawing.Player = _mediaPlayer;

            _drawingBrush = new DrawingBrush(_videoDrawing);
            _drawingBrush.Stretch = Stretch.None;

            Background = _drawingBrush;
        }

        private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is MediaElementEx mediaElement))
            {
                return;
            }

            switch (mediaElement.Source)
            {
                case Uri src when
                    src.Scheme.StartsWith("http", StringComparison.OrdinalIgnoreCase) ||
                    src.Scheme.StartsWith("https", StringComparison.OrdinalIgnoreCase):
                    mediaElement._mediaPlayer.Open(src);
                    break;

                case Uri src when src.Scheme.StartsWith("rtmp", StringComparison.OrdinalIgnoreCase):
                    // TODO: ffmpeg interop
                    break;

                case Uri src:
                    mediaElement._mediaPlayer.Open(src);
                    break;

                case string src:
                    mediaElement._mediaPlayer.Open(new Uri(src));
                    break;
            }
        }

        private static void OnPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void OnBufferingStarted(object sender, EventArgs e)
        {
        }

        private void OnBufferingEnded(object sender, EventArgs e)
        {
        }

        private void OnMediaOpened(object sender, EventArgs e)
        {
            _mediaPlayer.Position = InitialPosition;
            _mediaPlayer.Play();
        }

        private void OnMediaEnded(object sender, EventArgs e)
        {
        }

        private void OnMediaFailed(object sender, ExceptionEventArgs e)
        {
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            _videoDrawing.Rect = new Rect(0, 0, sizeInfo.NewSize.Width, sizeInfo.NewSize.Width * 0.5625);
        }
    }
}
