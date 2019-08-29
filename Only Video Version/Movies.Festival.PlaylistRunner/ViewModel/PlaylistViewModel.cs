using System;
using System.Linq;
using System.Windows;
using Movies.Festival.PlaylistRunner.Data;
using System.Windows.Input;
using System.Configuration;

namespace Movies.Festival.PlaylistRunner.ViewModel
{
    class PlaylistViewModel : DependencyObject, ICommand
    {
        public PlaylistViewModel(Playlist playlist)
        {
            Playlist = playlist;
            if (playlist.Clips.Count > 0)
                CurrentClip = playlist.Clips[0];
            if (playlist.Clips.Count > 1)
                UpcomingClip = playlist.Clips[1];

            CrossfadeTime = TimeSpan.FromSeconds(double.Parse(ConfigurationManager.AppSettings["CrossfadeSeconds"]));
        }

        public Playlist Playlist
        {
            get { return (Playlist)GetValue(PlaylistProperty); }
            set { SetValue(PlaylistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Playlist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaylistProperty =
            DependencyProperty.Register("Playlist", typeof(Playlist), typeof(PlaylistViewModel), new UIPropertyMetadata(null));

        public Clip CurrentClip
        {
            get { return (Clip)GetValue(CurrentClipProperty); }
            set { SetValue(CurrentClipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentClip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentClipProperty =
            DependencyProperty.Register("CurrentClip", typeof(Clip), typeof(PlaylistViewModel), new UIPropertyMetadata(null));

        public TimeSpan CrossfadeTime
        {
            get { return (TimeSpan)GetValue(CrossfadeTimeProperty); }
            set { SetValue(CrossfadeTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CrossfadeTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CrossfadeTimeProperty =
            DependencyProperty.Register("CrossfadeTime", typeof(TimeSpan), typeof(PlaylistViewModel), new UIPropertyMetadata(TimeSpan.FromSeconds(2)));

        public Clip UpcomingClip
        {
            get { return (Clip)GetValue(UpcomingClipProperty); }
            set { SetValue(UpcomingClipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UpcomingClip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpcomingClipProperty =
            DependencyProperty.Register("UpcomingClip", typeof(Clip), typeof(PlaylistViewModel), new UIPropertyMetadata(null, UpComingClipPropertyChanged));

        static void UpComingClipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlaylistViewModel)d).RaiseCanExecuteChanged();
        }
        
        public TimeSpan Position
        {
            get { return (TimeSpan)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(TimeSpan), typeof(PlaylistViewModel), new UIPropertyMetadata(TimeSpan.Zero));

        public TimeSpan TotalLength
        {
            get { return (TimeSpan)GetValue(TotalLengthProperty); }
            set { SetValue(TotalLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalLengthProperty =
            DependencyProperty.Register("TotalLength", typeof(TimeSpan), typeof(PlaylistViewModel), new UIPropertyMetadata(TimeSpan.Zero));



        public bool CanExecute(object parameter)
        {
            return CurrentClip != null && UpcomingClip != null;
        }

        public event EventHandler CanExecuteChanged;
        private void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, null);
            }
        }

        public void Execute(object parameter)
        {
            CurrentClip = UpcomingClip;
            int currIndex = Playlist.Clips.IndexOf(CurrentClip);
            
            if (currIndex < Playlist.Clips.Count - 1)
                UpcomingClip = Playlist.Clips[currIndex + 1];
            else
                UpcomingClip = null;
        }
    }
}
