using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ak.oss.PlaylistRunner.Data;

namespace ak.oss.PlaylistRunner.Services
{
    interface IPlaylistObtainer
    {
        Playlist GetPlaylist(IExtensionToTypeConverter extensionConverter, string file);
    }

    class MockPlaylistObtainer : IPlaylistObtainer
    {
        public Playlist GetPlaylist(IExtensionToTypeConverter extensionConverter, string file)
        {
            return new Playlist
            {
                Clips = new List<Clip> { 
                    new Clip { FilePath = @"D:\Video\Liel birthday\Movie5.wmv" },
                    new Clip { FilePath = @"D:\Video\Liel birthday\Movie14.wmv" },
                    new Clip { FilePath = @"D:\Video\Liel birthday\Movie12.wmv" },
                    new Clip { FilePath = @"D:\Video\Liel birthday\Movie7.wmv" },
                    new Clip { FilePath = @"D:\Video\Liel birthday\Movie2.wmv" },
                }
            };
        }
    }
}
