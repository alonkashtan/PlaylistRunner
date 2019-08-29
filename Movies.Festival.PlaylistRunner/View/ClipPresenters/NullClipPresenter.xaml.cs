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

namespace ak.oss.PlaylistRunner.View.ClipPresenters
{
    /// <summary>
    /// Interaction logic for NullClipPresenter.xaml
    /// </summary>
    public partial class NullClipPresenter : UserControl, IClipPresenter
    {
        public NullClipPresenter()
        {
            InitializeComponent();
        }

        Clip _originalClip;
        public Data.Clip OriginalClip
        {
            get
            {
                return _originalClip;
            }
            set
            {
                _originalClip = value;
                _label.Content = "תוכן חסר: " + _originalClip.DisplayName;
            }
        }

        public void StartEntranceSequence()
        {
            
        }

        public event Action<IClipPresenter> EntranceSequenceCompleted;

        public event Action<IClipPresenter> ReadyForCrossfade;

        public void StartExitSequence()
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
            get { return true; }
        }

        public bool IsInExitSequence
        {
            get { return false; }
        }
    }
}
