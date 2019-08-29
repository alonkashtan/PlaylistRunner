using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Movies.Festival.PlaylistRunner.Data;

namespace Movies.Festival.PlaylistRunner.Services
{
    interface IPlaylistObtainer
    {
        Playlist GetPlaylist();
    }

    class MockPlaylistObtainer : IPlaylistObtainer
    {
        public Playlist GetPlaylist()
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
