using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ak.oss.PlaylistRunner.ViewModel
{
    public interface IClipProvider
    {
        void GoToNextClip();
        bool CanGoToNextClip { get; }
    }
}
