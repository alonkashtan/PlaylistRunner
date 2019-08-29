using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using Movies.Festival.PlaylistRunner.Data;

namespace Movies.Festival.PlaylistRunner.Services
{
    class TextFilePlaylistObtainer : IPlaylistObtainer
    {
        public Data.Playlist GetPlaylist()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "*.txt|*.txt", Multiselect = false };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName == null) return null;

            string[] filelines = File.ReadAllLines(openFileDialog.FileName);

            string basePath = filelines[0];
            List<Clip> clips = new List<Clip>(filelines.Length - 1);

            for (int i = 1; i < filelines.Length; i++)
            {
                clips.Add(new Clip() { FilePath = Path.Combine(basePath, filelines[i]) });
            }

            return new Playlist() { Clips = clips };
        }
    }
}
