using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ICare.UserControls.ImagePlayer
{
    /// <summary>
    /// UcImagePlayerExt.xaml 的交互逻辑
    /// </summary>
    public sealed partial class UcImagePlayerExt : UserControl
    {
        private DispatcherTimer _dispatcherTimer;
        private const double PlaySpeedMin = 1;
        private const double PlaySpeedMax = 7;

        public UcImagePlayerExt()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            //初始化timer,用做循环播放控制
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            SpeedSlider.Minimum = PlaySpeedMin;
            SpeedSlider.Maximum = PlaySpeedMax;
            SpeedSlider.Value = 4;
        }

        #region IDependencyProperty

        #region 图像列表

        public static readonly DependencyProperty ImageSourceListProperty = DependencyProperty.Register(
            nameof(ImageSourceList), typeof(List<ImageSource>), typeof(UcImagePlayerExt), new PropertyMetadata(default(List<ImageSource>),
                (a, b) =>
                {
                    var ctx = a as UcImagePlayerExt;
                    if (ctx == null)
                        return;
                    var list = b.NewValue as List<ImageSource>;
                    if (list == null)
                        return;
                    ctx.CurrentImage.Source = list[0];
                    ctx.TotalFrameCount = list.Count;
                    ctx.CurrentFrameIndex = 1;
                    ctx.CurrentFrameTextBox.Text = "1";
                }));

        public List<ImageSource> ImageSourceList
        {
            get { return (List<ImageSource>)GetValue(ImageSourceListProperty); }
            set { SetValue(ImageSourceListProperty, value); }
        }

        #endregion 图像列表

        #region 当前帧的位置

        public static readonly DependencyProperty CurrentFrameIndexProperty = DependencyProperty.Register(
            nameof(CurrentFrameIndex), typeof(int), typeof(UcImagePlayerExt), new PropertyMetadata(default(int), (a, b) =>
            {
                var ctx = a as UcImagePlayerExt;
                if (ctx == null)
                    return;
                var currentFrameIndex = b.NewValue as int?;
                if (currentFrameIndex == null)
                    return;
                if (currentFrameIndex >= 1 && currentFrameIndex <= ctx.TotalFrameCount)
                {
                    ctx.CurrentImage.Source = ctx.ImageSourceList[(int)currentFrameIndex - 1];
                    ctx.FrameSlider.Value = (double)currentFrameIndex;
                    ctx.CurrentFrameTextBox.Text = currentFrameIndex.ToString();
                }
            }));

        public int CurrentFrameIndex
        {
            get { return (int)GetValue(CurrentFrameIndexProperty); }
            set { SetValue(CurrentFrameIndexProperty, value); }
        }

        #endregion 当前帧的位置

        #region 总帧数(图像列表总数)

        public static readonly DependencyProperty TotalFrameCountProperty = DependencyProperty.Register(
            nameof(TotalFrameCount), typeof(int), typeof(UcImagePlayerExt), new PropertyMetadata(default(int), (a, b) =>
            {
                var ctx = a as UcImagePlayerExt;
                if (ctx == null)
                    return;
                var totalFrameCount = b.NewValue as int?;
                if (totalFrameCount == null)
                    return;
                ctx.FrameSlider.Maximum = (double)totalFrameCount;
                ctx.TotalFrameTextBlock.Text = totalFrameCount.ToString();
            }));

        public int TotalFrameCount
        {
            get { return (int)GetValue(TotalFrameCountProperty); }
            set { SetValue(TotalFrameCountProperty, value); }
        }

        #endregion 总帧数(图像列表总数)

        #region 播放速度

        public static readonly DependencyProperty PlaySpeedProperty = DependencyProperty.Register(
            nameof(PlaySpeed), typeof(double), typeof(UcImagePlayerExt), new PropertyMetadata(1.0, (a, b) =>
            {
                var ctx = a as UcImagePlayerExt;
                if (ctx == null)
                    return;
                var playSpeed = b.NewValue as double?;
                if (playSpeed == null)
                    return;
                if (ctx._dispatcherTimer != null)
                {
                    ctx._dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / (int)((double)playSpeed * 25));
                }
            }));

        public double PlaySpeed
        {
            get { return (double)GetValue(PlaySpeedProperty); }
            set { SetValue(PlaySpeedProperty, value); }
        }

        #endregion 播放速度

        #region 播放/暂停控制

        public static readonly DependencyProperty StartPlayProperty = DependencyProperty.Register(
            nameof(StartPlay), typeof(bool), typeof(UcImagePlayerExt), new PropertyMetadata(default(bool), (a, b) =>
            {
                var ctx = a as UcImagePlayerExt;
                if (ctx == null)
                    return;
                var startPlay = b.NewValue as bool?;
                if (startPlay == null)
                    return;
                ctx.Play((bool)startPlay);
            }));

        public bool StartPlay
        {
            get { return (bool)GetValue(StartPlayProperty); }
            set { SetValue(StartPlayProperty, value); }
        }

        #endregion 播放/暂停控制

        #region 填充(Image.Stretch)

        public static readonly DependencyProperty CurrentImageStretchProperty = DependencyProperty.Register(
            nameof(CurrentImageStretch), typeof(Stretch), typeof(UcImagePlayerExt), new PropertyMetadata(default(Stretch), (a, b) =>
            {
                var ctx = a as UcImagePlayerExt;
                if (ctx == null)
                    return;
                var currentImageStretch = b.NewValue as Stretch?;
                if (currentImageStretch == null)
                    return;
                ctx.CurrentImage.Stretch = (Stretch)currentImageStretch;
            }));

        public Stretch CurrentImageStretch
        {
            get { return (Stretch)GetValue(CurrentImageStretchProperty); }
            set { SetValue(CurrentImageStretchProperty, value); }
        }

        #endregion 填充(Image.Stretch)

        #endregion IDependencyProperty

        private void Play(bool startPlay)
        {
            if (ImageSourceList == null)
            {
                return;
            }

            if (!startPlay)
            {
                _dispatcherTimer.Stop();
                return;
            }
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / (int)(PlaySpeed * 25));
            _dispatcherTimer.Start();
        }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (CurrentFrameIndex < TotalFrameCount)
            {
                CurrentFrameIndex++;
                return;
            }
            CurrentFrameIndex = 1;
        }

        private void ButtonPrev_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentFrameIndex > 1)
                CurrentFrameIndex--;
        }

        private void ButtonPlay_OnClick(object sender, RoutedEventArgs e)
        {
            StartPlay = true;
            ButtonPlay.Visibility = Visibility.Collapsed;
            ButtonStop.Visibility = Visibility.Visible;
        }

        private void ButtonStop_OnClick(object sender, RoutedEventArgs e)
        {
            StartPlay = false;
            ButtonPlay.Visibility = Visibility.Visible;
            ButtonStop.Visibility = Visibility.Collapsed;
        }

        private void ButtonNext_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentFrameIndex < TotalFrameCount)
                CurrentFrameIndex++;
        }

        /// <summary>
        /// 调整速度时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeedSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var speedNewValue = (int)e.NewValue;
            var speedInfo = SpeedInfoTuple.FirstOrDefault(p => p.Item1 == speedNewValue);
            if (speedInfo == null)
                return;
            PlaySpeed = speedInfo.Item2;
            SpeedTextBlock.Text = speedInfo.Item3;
        }

        /// <summary>
        /// 调整当前帧时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrameSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CurrentFrameIndex = (int)e.NewValue;
        }

        /// <summary>
        /// 输入跳转帧时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentFrameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(((TextBox)e.Source).Text, out int destFrame))
                return;
            CurrentFrameIndex = destFrame;
        }

        public static List<Tuple<int, double, string>> SpeedInfoTuple = new List<Tuple<int, double, string>>
        {
                new Tuple<int, double, string>(1,0.125,"1/8"),
                new Tuple<int, double, string>(2,0.25,"1/4"),
                new Tuple<int, double, string>(3,0.5,"1/2"),
                new Tuple<int, double, string>(4,1,"1"),
                new Tuple<int, double, string>(5,2,"2"),
                new Tuple<int, double, string>(6,4,"4"),
                new Tuple<int, double, string>(7,8,"8")
    };
    }
}