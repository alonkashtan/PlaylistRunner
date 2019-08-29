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
using Movies.Festival.PlaylistRunner.ViewModel;

namespace Movies.Festival.PlaylistRunner.View
{
    /// <summary>
    /// Interaction logic for ClipControlPanel.xaml
    /// </summary>
    public partial class ClipControlPanel : UserControl
    {
        readonly TimeSpan ADVANCE_BY = TimeSpan.FromSeconds(2);

        public ClipControlPanel()
        {
            InitializeComponent();
        }
        
        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPlaying.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(ClipControlPanel), new UIPropertyMetadata(false));

        IControlableClipPresenter presenter;
        public IControlableClipPresenter Presenter
        {
            get { return presenter; }
            set
            {
                if (presenter == value) return;

                if (presenter != null)
                {
                    presenter.IsPlayingChanged -= presenter_IsPlayingChanged;
                }

                presenter = value;

                if (value != null)
                {
                    IsPlaying = presenter.IsPlaying;
                    presenter.IsPlayingChanged += presenter_IsPlayingChanged;
                }
            }
        }

        void presenter_IsPlayingChanged(IControlableClipPresenter sender, bool isPlaying)
        {
            IsPlaying = isPlaying;
        }


        void Backwards_Click(object sender, RoutedEventArgs e)
        {
            Presenter.AdvancePosition(ADVANCE_BY.Negate());
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Presenter.StopAndReset();
        }


        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Presenter.ResetAndPlay();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            Presenter.Pause();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Presenter.Play();
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            Presenter.AdvancePosition(ADVANCE_BY);
        }

        private void SetPosition_Click(object sender, RoutedEventArgs e)
        {
            Presenter.Position = TimeSpan.FromMilliseconds(slider.Value);
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Presenter.Position = (TimeSpan)e.Parameter;
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute= slider.Maximum>0 || Presenter.IsPlaying;
        }
    }
}
