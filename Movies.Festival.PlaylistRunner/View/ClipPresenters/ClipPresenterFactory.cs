using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ak.oss.PlaylistRunner.Data;

namespace ak.oss.PlaylistRunner.View.ClipPresenters
{
    class ClipPresenterFactory
    {
        public IClipPresenter CreatePresenterForClip(Clip clip)
        {
            switch (clip.Type)
            {
                case ClipType.Video:
                    return new VideoClipPresenter() { OriginalClip = clip };
                case ClipType.Image:
                    return new ImagePresenter() { OriginalClip = clip };
                case ClipType.Audio:
                    return new AudioPresenter() { OriginalClip = clip };
                default:
                    return new NullClipPresenter() { OriginalClip = clip };
            }
        }
    }
}
