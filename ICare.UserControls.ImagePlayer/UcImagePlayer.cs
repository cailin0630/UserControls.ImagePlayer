using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;

namespace ICare.UserControls.ImagePlayer
{
    public class UcImagePlayer : Image, INotifyPropertyChanged
    {
        private readonly DispatcherTimer _dispatcherTimer;
        

        public UcImagePlayer()
        {
            //初始化timer,用做循环播放控制
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
        }

        #region IDependencyProperty

        #region 图像列表

        public static readonly DependencyProperty ImageSourceListProperty = DependencyProperty.Register(
            nameof(ImageSourceList), typeof(List<ImageSource>), typeof(UcImagePlayer), new PropertyMetadata(default(List<ImageSource>),
                (a, b) =>
                {
                    var ctx = a as UcImagePlayer;
                    if (ctx == null)
                        return;
                    var list = b.NewValue as List<ImageSource>;
                    if (list == null)
                        return;
                    ctx.Source = list[0];
                    ctx.TotalFrameCount = list.Count;
                    ctx.CurrentFrameIndex = 1;
                }));

        public List<ImageSource> ImageSourceList
        {
            get { return (List<ImageSource>)GetValue(ImageSourceListProperty); }
            set { SetValue(ImageSourceListProperty, value); }
        }

        #endregion 图像列表

        #region 当前帧的位置

        public static readonly DependencyProperty CurrentFrameIndexProperty = DependencyProperty.Register(
            nameof(CurrentFrameIndex), typeof(int), typeof(UcImagePlayer), new PropertyMetadata(default(int), (a, b) =>
            {
                var ctx = a as UcImagePlayer;
                if (ctx == null)
                    return;
                var currentFrameIndex = b.NewValue as int?;
                if (currentFrameIndex == null)
                    return;
                if (currentFrameIndex >= 1 && currentFrameIndex <= ctx.TotalFrameCount)
                {
                    ctx.Source = ctx.ImageSourceList[(int)currentFrameIndex - 1];
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
            nameof(TotalFrameCount), typeof(int), typeof(UcImagePlayer), new PropertyMetadata(default(int)));

        public int TotalFrameCount
        {
            get { return (int)GetValue(TotalFrameCountProperty); }
            set { SetValue(TotalFrameCountProperty, value); }
        }

        #endregion 总帧数(图像列表总数)

        #region 播放速度

        public static readonly DependencyProperty PlaySpeedProperty = DependencyProperty.Register(
            nameof(PlaySpeed), typeof(double), typeof(UcImagePlayer), new PropertyMetadata(1.0, (a, b) =>
            {
                var ctx = a as UcImagePlayer;
                if (ctx == null)
                    return;
                var playSpeed = b.NewValue as double?;
                if (playSpeed == null)
                    return;
                ctx._dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / (int)((double)playSpeed * 25));
            }));

        public double PlaySpeed
        {
            get { return (double)GetValue(PlaySpeedProperty); }
            set { SetValue(PlaySpeedProperty, value); }
        }

        #endregion 播放速度

        #region 播放/暂停控制

        public static readonly DependencyProperty StartPlayProperty = DependencyProperty.Register(
            nameof(StartPlay), typeof(bool), typeof(UcImagePlayer), new PropertyMetadata(default(bool), (a, b) =>
            {
                var ctx = a as UcImagePlayer;
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

        //#region 如何调整内容大小以填充为其分配的空间(Image.Stretch)

        //public static readonly DependencyProperty CurrentImageStretchProperty = DependencyProperty.Register(
        //    nameof(CurrentImageStretch), typeof(Stretch), typeof(UcImagePlayer), new PropertyMetadata(default(Stretch), (a, b) =>
        //    {
        //        var ctx = a as UcImagePlayer;
        //        if (ctx == null)
        //            return;
        //        var currentImageStretch = b.NewValue as Stretch?;
        //        if (currentImageStretch == null)
        //            return;
        //        ctx.Stretch = (Stretch)currentImageStretch;
        //    }));

        //public Stretch CurrentImageStretch
        //{
        //    get { return (Stretch)GetValue(CurrentImageStretchProperty); }
        //    set { SetValue(CurrentImageStretchProperty, value); }
        //}

        //#endregion 如何调整内容大小以填充为其分配的空间(Image.Stretch)

        #endregion IDependencyProperty

        protected virtual void Play(bool startPlay)
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

        public event PropertyChangedEventHandler PropertyChanged;

       
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}