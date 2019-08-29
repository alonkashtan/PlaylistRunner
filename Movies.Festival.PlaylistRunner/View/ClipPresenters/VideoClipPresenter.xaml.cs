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
using ak.oss.PlaylistRunner.ViewModel;
using ak.oss.PlaylistRunner.Data;
using System.Configuration;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ak.oss.PlaylistRunner.View.ClipPresenters
{
    /// <summary>
    /// Interaction logic for VideoClipPresenter.xaml
    /// </summary>
    public partial class VideoClipPresenter : UserControl, IClipPresenter, IControlableClipPresenter
    {
        private TimeSpan _crossfadeTime;
        readonly TimeSpan POSITION_RESOLUTION = TimeSpan.FromSeconds(0.1);

        public VideoClipPresenter()
        {
            _crossfadeTime = TimeSpan.FromSeconds(double.Parse(ConfigurationManager.AppSettings["CrossfadeSeconds"]));
            
            Opacity = 0;

            InitializeComponent();

            IsInExitSequence = false;

            _timer = new DispatcherTimer();
            _timer.Interval = POSITION_RESOLUTION;
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }

        #region IClipPresenter

        private Clip _clip;
        public Clip OriginalClip
        {
            get
            {
                return _clip;
            }
            set
            {
                _clip = value;
                _mediaElement.Source = new Uri(_clip.FilePath);
            }
        }

        public void StartEntranceSequence()
        {
            Duration animationDuration = new Duration(_crossfadeTime - TimeSpan.FromSeconds(0.2));

            DoubleAnimation animation = new DoubleAnimation(0, 1, animationDuration);
            animation.Completed += new EventHandler(EntranceAnimation_Completed);
            this.BeginAnimation(UserControl.OpacityProperty, animation);
            
            _mediaElement.Volume = OriginalClip.Volume;
            //_mediaElement.BeginAnimation(MediaElement.VolumeProperty, new DoubleAnimation(0, 1, animationDuration));
            Play();
        }

        void EntranceAnimation_Completed(object sender, EventArgs e)
        {
            if (EntranceSequenceCompleted != null)
                EntranceSequenceCompleted(this);
        }

        public event Action<IClipPresenter> EntranceSequenceCompleted;
        public event Action<IClipPresenter> ReadyForCrossfade;
        private void RaiseReadyForCrossfade()
        {
            if (ReadyForCrossfade != null)
            {
                ReadyForCrossfade(this);
            }
        }

        public void StartExitSequence()
        {
            Duration animationDuration = new Duration(_crossfadeTime - TimeSpan.FromSeconds(0.2));

            DoubleAnimation animation = new DoubleAnimation(OriginalClip.Volume, 0, animationDuration);
            animation.Completed += new EventHandler(ExitAnimation_Completed);
            //this.BeginAnimation(UserControl.OpacityProperty, animation);

            _mediaElement.BeginAnimation(MediaElement.VolumeProperty, animation);

            IsInExitSequence = true;
        }

        void ExitAnimation_Completed(object sender, EventArgs e)
        {
            Pause();
            if (ExitSequenceCompleted != null)
                ExitSequenceCompleted(this);
        }

        public event Action<IClipPresenter> ExitSequenceCompleted;

        public bool HidesPreviousClip
        {
            get { return true; }
        }


        public bool IsInExitSequence { get; private set; }

        #endregion

        #region IControlableClipPresenter

        public void Pause()
        {
            _mediaElement.Pause();
            IsPlaying = false;
        }

        public void Play()
        {
            _mediaElement.Play();
            IsPlaying = true;
        }

        public void StopAndReset()
        {
            _mediaElement.Stop();
            IsPlaying = false;
        }

        public void ResetAndStop()
        {
            StopAndReset();
        }

        public void ResetAndPlay()
        {
            _mediaElement.Position = TimeSpan.Zero;
            Play();
        }

        public void AdvancePosition(TimeSpan by)
        {
            _mediaElement.Position += by;
        }

        private bool isPlaying;
        public bool IsPlaying
        {
            get { return isPlaying; }
            private set { isPlaying = value; RaiseIsPlayingChanged(); }
        }

        public TimeSpan Position
        {
            get { return _mediaElement != null ? _mediaElement.Position : TimeSpan.Zero; }
            set
            {
                _mediaElement.Position = value;
                RaisePositionChanged();
            }
        }

        public TimeSpan TotalTime
        {
            get
            {
                return _mediaElement != null && _mediaElement.NaturalDuration.HasTimeSpan
                   ? _mediaElement.NaturalDuration.TimeSpan
                   : TimeSpan.Zero;
            }
        }

        public List<Mark> Marks
        {
            get
            {
                return _clip.Marks;
            }
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

        #region maintaine position

        DispatcherTimer _timer;
        private bool _isInCrossfade = false;

        void _timer_Tick(object sender, EventArgs e)
        {
            RaisePositionChanged();
            RaiseTotalTimeChanged();

            if (OriginalClip.IsLoop)
            {
                if (_mediaElement.NaturalDuration.HasTimeSpan &&
                    _mediaElement.NaturalDuration.TimeSpan - _mediaElement.Position <= POSITION_RESOLUTION)
                {
                    ResetAndPlay();
                }
            }
            else
            {
                if (!_isInCrossfade &&
                    _mediaElement.NaturalDuration.HasTimeSpan &&
                    _mediaElement.NaturalDuration.TimeSpan - _mediaElement.Position <= _crossfadeTime)
                {
                    RaiseReadyForCrossfade();
                    _isInCrossfade = true;
                }
            }
        }

        #endregion
    }
}
