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
using System.Windows.Threading;
using ak.oss.PlaylistRunner.Data;
using System.Windows.Media.Animation;
using System.Configuration;

namespace ak.oss.PlaylistRunner.View.ClipPresenters
{
    /// <summary>
    /// Interaction logic for AudioPresenter.xaml
    /// </summary>
    public partial class AudioPresenter : UserControl, IClipPresenter, IControlableClipPresenter
    {
        private TimeSpan _crossfadeTime;

        public AudioPresenter()
        {
            _crossfadeTime = TimeSpan.FromSeconds(double.Parse(ConfigurationManager.AppSettings["CrossfadeSeconds"]));

            InitializeComponent();
            
            IsInExitSequence = false;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.1);
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
                _mediaElement.Volume = _clip.Volume;
            }
        }

        public void StartEntranceSequence()
        {
            Play();
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

            _mediaElement.BeginAnimation(MediaElement.VolumeProperty, animation);

            IsInExitSequence = true;
        }

        void ExitAnimation_Completed(object sender, EventArgs e)
        {
            RaiseExitSequenceCompleted();
        }

        public event Action<IClipPresenter> ExitSequenceCompleted;
        private void RaiseExitSequenceCompleted()
        {
            if (ExitSequenceCompleted != null)
                ExitSequenceCompleted(this);
        }

        public bool HidesPreviousClip
        {
            get { return false; }
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

            if (!_isInCrossfade &&
                _mediaElement.NaturalDuration.HasTimeSpan &&
                _mediaElement.NaturalDuration.TimeSpan - _mediaElement.Position <= TimeSpan.Zero)
            {
                _isInCrossfade = true;
                RaiseReadyForCrossfade();
                RaiseExitSequenceCompleted();
            }
        }

        #endregion
    }
}
