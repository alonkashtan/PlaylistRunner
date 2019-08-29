using System;
using System.Linq;
using System.Windows;
using ak.oss.PlaylistRunner.Data;
using System.Windows.Input;
using System.Configuration;
using ak.oss.PlaylistRunner.View.ClipPresenters;
using ak.oss.PlaylistRunner.View;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Collections.Generic;

namespace ak.oss.PlaylistRunner.ViewModel
{
    class PlaylistViewModel : DependencyObject, IControlableClipPresenter, IClipProvider
    {
        private ClipPresenterFactory _factory;

        public PlaylistViewModel(Playlist playlist, ClipPresenterFactory factory)
        {
            NextClipCommand = new DelegatingCommand(Execute, CanExecute);

            _factory = factory;

            Playlist = new ObservableCollection<Clip>(playlist.Clips);
            if (playlist.Clips.Count > 0)
                CurrentClip = Playlist[0];

            if (playlist.Clips.Count > 1)
                UpcomingClip = Playlist[1];

            Presenters = new ObservableCollection<IClipPresenter>();

            TransitToNextClip(CurrentClip);
            Pause();
        }

        #region properties



        public ObservableCollection<Clip> Playlist
        {
            get { return (ObservableCollection<Clip>)GetValue(PlaylistProperty); }
            set { SetValue(PlaylistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Playlist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaylistProperty =
            DependencyProperty.Register("Playlist", typeof(ObservableCollection<Clip>), typeof(PlaylistViewModel), new UIPropertyMetadata(null));



        public Clip CurrentClip
        {
            get { return (Clip)GetValue(CurrentClipProperty); }
            set { SetValue(CurrentClipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentClip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentClipProperty =
            DependencyProperty.Register("CurrentClip", typeof(Clip), typeof(PlaylistViewModel), new UIPropertyMetadata(null, ClipPropertyChanged));

        public Clip UpcomingClip
        {
            get { return (Clip)GetValue(UpcomingClipProperty); }
            set { SetValue(UpcomingClipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UpcomingClip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpcomingClipProperty =
            DependencyProperty.Register("UpcomingClip", typeof(Clip), typeof(PlaylistViewModel), new UIPropertyMetadata(null, ClipPropertyChanged));

        static void ClipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlaylistViewModel)d).NextClipCommand.RaiseCanExecuteChanged();
        }
        
        public TimeSpan Position
        {
            get { return (TimeSpan)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(TimeSpan), typeof(PlaylistViewModel), new UIPropertyMetadata(TimeSpan.Zero));

        public TimeSpan TotalTime
        {
            get { return (TimeSpan)GetValue(TotalTimeProperty); }
            set { SetValue(TotalTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalTimeProperty =
            DependencyProperty.Register("TotalTime", typeof(TimeSpan), typeof(PlaylistViewModel), new UIPropertyMetadata(TimeSpan.Zero));

        
        /// <summary>
        /// Marks on the progress bar. Used to mark points in a clip on the progress bar
        /// </summary>
        public List<Mark> Marks
        {
            get { return (List<Mark>)GetValue(MarksProperty); }
            set { SetValue(MarksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ticks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarksProperty =
            DependencyProperty.Register("Marks", typeof(List<Mark>), typeof(PlaylistViewModel), new PropertyMetadata(null));
        

        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPlaying.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(PlaylistViewModel), new UIPropertyMetadata(false));

       public ObservableCollection<IClipPresenter> Presenters
        {
            get { return (ObservableCollection<IClipPresenter>)GetValue(PresentersProperty); }
            set { SetValue(PresentersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Presenters.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PresentersProperty =
            DependencyProperty.Register("Presenters", typeof(ObservableCollection<IClipPresenter>), typeof(PlaylistViewModel), new UIPropertyMetadata(null));

        public DelegatingCommand NextClipCommand { get; private set; }




        #endregion

        #region NextCommand implementation / IClipProvider

        public void GoToNextClip()
        {
            Execute(null);
        }

        public bool CanGoToNextClip
        {
            get { return CanExecute(null); }
        }

        public bool CanExecute(object parameter)
        {
            return UpcomingClip != null && (Presenters.Count == 0 || !Presenters[0].IsInExitSequence);
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;

            var nextPresenter = TransitToNextClip(UpcomingClip);

            if (Presenters.Count == 1) return; //no previous presenter

            var currentPresenter = Presenters[0];

            if (nextPresenter.HidesPreviousClip && !currentPresenter.IsInExitSequence)
            {
                for (int i = 0; i < Presenters.Count - 1; i++)
                {
                    if (Presenters[i].IsInExitSequence) continue;

                    Presenters[i].ReadyForCrossfade -= Clip_ReadyForCrossfade;
                    if (i==0)
                        Presenters[0].ExitSequenceCompleted += Clip_ExitSequenceCompleted;
                    else
                        Presenters[i].ExitSequenceCompleted += SubClip_ExitSequenceCompleted;

                    Presenters[i].StartExitSequence();
                }

                NextClipCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region playlist logic

        IClipPresenter _presenterInFocusBacking;
        IClipPresenter _presenterInFocus
        {
            get { return _presenterInFocusBacking; }
            set
            {
                if (_presenterInFocusBacking == value) return;

                if (_presenterInFocusBacking is IControlableClipPresenter)
                {
                    var controlable = (IControlableClipPresenter)_presenterInFocusBacking;
                    controlable.PositionChanged -= controlable_PositionChanged;
                    controlable.TotalTimeChanged -= controlable_TotalTimeChanged;
                    controlable.IsPlayingChanged -= controlable_IsPlayingChanged;
                }

                _presenterInFocusBacking = value;
                CurrentClip = _presenterInFocusBacking.OriginalClip;

                if (_presenterInFocusBacking is IControlableClipPresenter)
                {
                    var controlable = (IControlableClipPresenter)_presenterInFocusBacking;
                    TotalTime = controlable.TotalTime; RaiseTotalTimeChanged();
                    Marks = controlable.Marks;
                    if (Marks!=null && Marks.Count > 0)
                    {
                        Mark lastMark = Marks[Marks.Count - 1];
                        lastMark.Length = TotalTime - lastMark.Location;
                    }
                    Position = controlable.Position; RaisePositionChanged();
                    IsPlaying = controlable.IsPlaying; RaiseIsPlayingChanged();
                    controlable.PositionChanged += controlable_PositionChanged;
                    controlable.TotalTimeChanged += controlable_TotalTimeChanged;
                    controlable.IsPlayingChanged += controlable_IsPlayingChanged;
                }
            }
        }

        IClipPresenter TransitToNextClip(Clip nextClip)
        {
            var nextPresenter = _factory.CreatePresenterForClip(nextClip);

            _presenterInFocus=nextPresenter;

            Presenters.Add(nextPresenter);
            
            nextPresenter.StartEntranceSequence();

            nextPresenter.ReadyForCrossfade += Clip_ReadyForCrossfade;

            return nextPresenter;
        }

        /// <summary>
        /// Accures when a clip that is in exit sequence completes the sequence. Should be removed.
        /// </summary>
        /// <param name="sender"></param>
        void Clip_ExitSequenceCompleted(IClipPresenter sender)
        {
            sender.ExitSequenceCompleted -= Clip_ExitSequenceCompleted;
            Presenters.Remove(sender);

            if (!sender.HidesPreviousClip && sender == _presenterInFocus && Presenters.Count > 0)
            {
                _presenterInFocus = Presenters[0];
                CurrentClip = Presenters[0].OriginalClip;
            }
                
            if (sender.HidesPreviousClip)
                AdvanceCurrentClip();
            else
                AdvanceUpcomingClip();

            NextClipCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Happens when a presenter which his exit sequence was started when he was not Presenters[0], only need
        /// to remove from presenters.
        /// </summary>
        /// <param name="obj"></param>
        void SubClip_ExitSequenceCompleted(IClipPresenter sender)
        {
            sender.ExitSequenceCompleted -= SubClip_ExitSequenceCompleted;
            Presenters.Remove(sender);
            NextClipCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Accures when a clip is ready for crossfade
        /// </summary>
        /// <param name="sender"></param>
        void Clip_ReadyForCrossfade(IClipPresenter sender)
        {
            sender.ReadyForCrossfade -= Clip_ReadyForCrossfade;
            sender.ExitSequenceCompleted += Clip_ExitSequenceCompleted;
            sender.StartExitSequence();

            if (UpcomingClip != null && sender.HidesPreviousClip)
                TransitToNextClip(UpcomingClip);
        }

        private void AdvanceCurrentClip()
        {
            CurrentClip = UpcomingClip;
            AdvanceUpcomingClip();
        }

        void controlable_PositionChanged(TimeSpan position)
        {
            Position = position;
        }

        void controlable_TotalTimeChanged(TimeSpan totalTime)
        {
            TotalTime = totalTime;
        }

        void controlable_IsPlayingChanged(IControlableClipPresenter presenter, bool isPlaying)
        {
            IsPlaying = isPlaying;
            RaiseIsPlayingChanged();
        }

        private void AdvanceUpcomingClip()
        {
            if (UpcomingClip == null) return;

            int currIndex = Playlist.IndexOf(UpcomingClip);

            if (currIndex < Playlist.Count - 1)
            {
                UpcomingClip = Playlist[currIndex+1];
            }
            else
                UpcomingClip = null;
        }

        #endregion

        #region IControlableClipPresenter

        public void Pause()
        {
            if (_presenterInFocus is IControlableClipPresenter)
                ((IControlableClipPresenter)_presenterInFocus).Pause();
        }

        public void Play()
        {
            if (_presenterInFocus is IControlableClipPresenter)
                ((IControlableClipPresenter)_presenterInFocus).Play();
        }

        public void StopAndReset()
        {
            if (_presenterInFocus is IControlableClipPresenter)
                ((IControlableClipPresenter)_presenterInFocus).StopAndReset();
        }

        public void ResetAndStop()
        {
            if (_presenterInFocus is IControlableClipPresenter)
                ((IControlableClipPresenter)_presenterInFocus).ResetAndStop();
        }

        public void ResetAndPlay()
        {
            if (_presenterInFocus is IControlableClipPresenter)
                ((IControlableClipPresenter)_presenterInFocus).ResetAndPlay();
        }

        public void AdvancePosition(TimeSpan by)
        {
            if (_presenterInFocus is IControlableClipPresenter)
                ((IControlableClipPresenter)_presenterInFocus).AdvancePosition(by);
        }

        TimeSpan IControlableClipPresenter.Position
        {
            get { return Position; }
            set
            {
                if (_presenterInFocus is IControlableClipPresenter)
                    ((IControlableClipPresenter)_presenterInFocus).Position = value;
                else
                    Position = value;
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
    }
}
