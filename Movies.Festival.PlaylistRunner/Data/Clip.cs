using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ak.oss.PlaylistRunner.Data
{
    public class Clip
    {
        public Clip()
        {
            Marks = new List<Mark>();
        }

        public string FilePath { get; set; }
        public ClipType Type { get; set; }
        public string DisplayName { get; set; }
        public bool IsLoop { get; set; }
        public double Volume { get; set; }
        /// <summary>
        /// Marks points in the clip
        /// </summary>
        public List<Mark> Marks { get; private set; }
    }

    /// <summary>
    /// Marks a point in a clip
    /// </summary>
    public class Mark
    {
        public TimeSpan Location { get; set; }
        public TimeSpan Length { get; set; }
        public string Name { get; set; }
    }

    public enum ClipType
    {
        Video, Image, Audio, Unknown
    }

    public class Playlist
    {
        public IList<Clip> Clips { get; set; }
    }
}
