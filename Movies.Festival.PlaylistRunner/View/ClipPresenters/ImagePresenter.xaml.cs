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
using System.Windows.Media.Animation;
using System.Configuration;
using System.Windows.Threading;

namespace ak.oss.PlaylistRunner.View.ClipPresenters
{
    /// <summary>
    /// Interaction logic for ImagePresenter.xaml
    /// </summary>
    public partial class ImagePresenter : UserControl, IClipPresenter
    {
        private TimeSpan _crossfadeTime;
        public ImagePresenter()
        {
            _crossfadeTime = TimeSpan.FromSeconds(double.Parse(ConfigurationManager.AppSettings["CrossfadeSeconds"]));
            InitializeComponent();
        }

        private Clip _originalClip;
        public Clip OriginalClip
        {
            get
            {
                return _originalClip;
            }
            set
            {
                _originalClip = value;
                _image.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(_originalClip.FilePath);
            }
        }

        public void StartEntranceSequence()
        {
            Duration animationDuration = new Duration(_crossfadeTime - TimeSpan.FromSeconds(0.2));

            DoubleAnimation animation = new DoubleAnimation(0, 1, animationDuration);
            animation.Completed += new EventHandler(EntranceAnimation_Completed);
            this.BeginAnimation(UserControl.OpacityProperty, animation);
        }

        void EntranceAnimation_Completed(object sender, EventArgs e)
        {
            if (EntranceSequenceCompleted != null)
                EntranceSequenceCompleted(this);
        }

        public event Action<IClipPresenter> EntranceSequenceCompleted;

        public event Action<IClipPresenter> ReadyForCrossfade;

        DispatcherTimer _timer;
        public void StartExitSequence()
        {
            if (_timer != null) _timer.Stop();

            _timer = new DispatcherTimer();
            _timer.Tick += ExitAnimation_Completed;
            _timer.Interval = _crossfadeTime;
            _timer.Start();
        }

        void ExitAnimation_Completed(object sender, EventArgs e)
        {
            _timer.Stop();
            _timer = null;

            if (ExitSequenceCompleted != null)
                ExitSequenceCompleted(this);
        }

        public event Action<IClipPresenter> ExitSequenceCompleted;

        public bool HidesPreviousClip
        {
            get { return true; }
        }

        public bool IsInExitSequence
        {
            get { return false; }
        }
    }
}
