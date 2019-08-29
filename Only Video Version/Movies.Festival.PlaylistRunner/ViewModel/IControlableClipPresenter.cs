using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.Festival.PlaylistRunner.ViewModel
{
    public interface IControlableClipPresenter
    {
        void Pause();
        void Play();
        void StopAndReset();
        void ResetAndStop(); //Same as StopAndReset, just for API convinience
        void ResetAndPlay();
        void AdvancePosition(TimeSpan by);
        bool IsPlaying { get; }
        TimeSpan Position { get; set; }
        event Action<IControlableClipPresenter, bool> IsPlayingChanged;
    }
}
