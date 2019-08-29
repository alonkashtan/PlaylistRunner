using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.Festival.PlaylistRunner.Data
{
    public class Clip
    {
        public string FilePath { get; set; }

    }

    public class Playlist
    {
        public IList<Clip> Clips { get; set; }
    }
}
