using ak.oss.PlaylistRunner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ak.oss.PlaylistRunner.ViewModel
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
        TimeSpan TotalTime { get; }
        List<Mark> Marks { get; }
        event Action<IControlableClipPresenter, bool> IsPlayingChanged;
        event Action<TimeSpan> PositionChanged;
        event Action<TimeSpan> TotalTimeChanged;
    }
}
