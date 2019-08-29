using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ak.oss.PlaylistRunner.Data;

namespace ak.oss.PlaylistRunner.View
{
    public interface IClipPresenter
    {
        Clip OriginalClip { get; set; }
        void StartEntranceSequence();
        event Action<IClipPresenter> EntranceSequenceCompleted;
        event Action<IClipPresenter> ReadyForCrossfade;
        void StartExitSequence();
        event Action<IClipPresenter> ExitSequenceCompleted;
        bool HidesPreviousClip { get; }
        bool IsInExitSequence { get; }
    }
}
