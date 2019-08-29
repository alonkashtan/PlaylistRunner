using System;
using System.Collections.Generic;
using System.Linq;
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
using ak.oss.PlaylistRunner.Data;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Diagnostics;
using ak.oss.PlaylistRunner.ViewModel;

namespace ak.oss.PlaylistRunner.View
{
     /// <summary>
    /// Interaction logic for ClipView.xaml
    /// </summary>
    public partial class ClipView : UserControl, IControlableClipPresenter
    {
        public ClipView()
        {
            InitializeComponent();
            currentBorder = Border1;
            nextBorder = Border2;

            currentBorder.Opacity = 1;
            Border2.Opacity = 0;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        DispatcherTimer timer;
        MediaElement currentMediaElement;
        Border currentBorder;
        Border nextBorder;
        bool isPlaying = false;

        #region Properties

        public TimeSpan CrossfadeTime
        {
            get { return (TimeSpan)GetValue(CrossfadeTimeProperty); }
            set { SetValue(CrossfadeTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CrossfadeTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CrossfadeTimeProperty =
            DependencyProperty.Register("CrossfadeTime", typeof(TimeSpan), typeof(ClipView), new UIPropertyMetadata(TimeSpan.FromSeconds(2)));
        
        

        public IClipPresenter CurrentClip
        {
            get { return (IClipPresenter)GetValue(CurrentClipProperty); }
            set { SetValue(CurrentClipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentClip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentClipProperty =
            DependencyProperty.Register("CurrentClip", typeof(IClipPresenter), typeof(ClipView), new UIPropertyMetadata(null, ClipPropertyChanged));

        static void ClipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ClipView)d).ClipChanged((Clip)e.NewValue);
        }

        public ICommand CrossfadeReadyCommand
        {
            get { return (ICommand)GetValue(CrossfadeReadyCommandProperty); }
            set { SetValue(CrossfadeReadyCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CrossfadeReadyCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CrossfadeReadyCommandProperty =
            DependencyProperty.Register("CrossfadeReadyCommand", typeof(ICommand), typeof(ClipView), new UIPropertyMetadata(null));

        #endregion

        #region Methods

        public void Pause()
        {
            currentMediaElement.Pause();
            IsPlaying = false;
        }

        public void Play()
        {
            currentMediaElement.Play();
            IsPlaying = true;
        }

        public void StopAndReset()
        {
            currentMediaElement.Stop();
            IsPlaying = false;
        }

        //just for API convinience
        public void ResetAndStop()
        {
            StopAndReset();
        }

        public void ResetAndPlay()
        {
            currentMediaElement.Position = TimeSpan.Zero;
            IsPlaying = true;
        }

        public void AdvancePosition(TimeSpan by)
        {
            currentMediaElement.Position += by;
        }

        public TimeSpan Position
        {
            get { return currentMediaElement!=null? currentMediaElement.Position:TimeSpan.Zero; }
            set
            {
                currentMediaElement.Position = value;
                RaisePositionChanged();
            }
        }
        
        public TimeSpan TotalTime
        {
            get
            {
                return currentMediaElement != null && currentMediaElement.NaturalDuration.HasTimeSpan
                   ? currentMediaElement.NaturalDuration.TimeSpan
                   : TimeSpan.Zero;
            }
        }

        public List<Mark> Marks
        {
            get
            {
                return CurrentClip.OriginalClip.Marks;
            }
        }

        public bool IsPlaying
        {
            get { return isPlaying; }
            private set { isPlaying = value; RaiseIsPlayingChanged(); }
        }

        public event Action<IControlableClipPresenter, bool> IsPlayingChanged;

        private void RaiseIsPlayingChanged()
        {
            if (IsPlayingChanged != null)
            {
                IsPlayingChanged(this, IsPlaying);
            }
        }

        public event Action<TimeSpan> PositionChanged;
        private void RaisePositionChanged()
        {
            if (PositionChanged != null)
            {
                PositionChanged(Position);
            }
        }

        public event Action<TimeSpan> TotalTimeChanged;
        private void RaiseTotalTimeChanged()
        {
            if (TotalTimeChanged != null)
            {
                TotalTimeChanged(TotalTime);
            }
        }

        #endregion

        #region internal stuff

        bool firstTime=true;
        Clip waitingClip = null;
        void ClipChanged(Clip newClip)
        {
            if (inTransition)
            {
                waitingClip = newClip;
                return;
            }

            MediaElement oldMedia = currentMediaElement;
            currentMediaElement = new MediaElement() { Source = new Uri(newClip.FilePath), LoadedBehavior = MediaState.Manual, Volume=0 };
            
            if (firstTime)
            {
                currentBorder.Child = currentMediaElement;
                currentMediaElement.Volume = 1;
                //currentMediaElement.Play();
                firstTime = false;
                return;
            }

            inTransition = true;

            Duration animationDuration = new Duration(CrossfadeTime - TimeSpan.FromSeconds(0.2));

            DoubleAnimation animation = new DoubleAnimation(0, 1, animationDuration);
            animation.Completed += new EventHandler(animation_Completed);
            nextBorder.BeginAnimation(MediaElement.OpacityProperty, animation);

            currentMediaElement.BeginAnimation(MediaElement.VolumeProperty, new DoubleAnimation(0, 1, animationDuration));
            oldMedia.BeginAnimation(MediaElement.VolumeProperty, new DoubleAnimation(1, 0, animationDuration));

            nextBorder.Child = currentMediaElement;
            currentMediaElement.Play();
            IsPlaying = true;

            Border temp = currentBorder;
            currentBorder = nextBorder;
            nextBorder = temp;
        }

        void animation_Completed(object sender, EventArgs e)
        {
            nextBorder.Opacity = 0;
            ((MediaElement)nextBorder.Child).Stop();
            nextBorder.Child = null;
            
            currentBorder.Opacity = 1;

            Grid.SetZIndex(currentBorder, 0);
            Grid.SetZIndex(nextBorder, 1);

            inTransition = false;

            if (waitingClip != null)
            {
                Clip curr = waitingClip;
                waitingClip = null;
                ClipChanged(curr);
            }
        }

        bool inTransition=false;

        void timer_Tick(object sender, EventArgs e)
        {
            RaisePositionChanged();
            RaiseTotalTimeChanged();

            if (!inTransition &&
                currentMediaElement != null &&
                currentMediaElement.NaturalDuration.HasTimeSpan &&
                currentMediaElement.NaturalDuration.TimeSpan - currentMediaElement.Position <= CrossfadeTime &&
                CrossfadeReadyCommand != null && CrossfadeReadyCommand.CanExecute(null))
            {
                CrossfadeReadyCommand.Execute(null);
            }
        }

        #endregion
    }
}
