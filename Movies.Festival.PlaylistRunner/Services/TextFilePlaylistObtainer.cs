using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using System.IO;
using ak.oss.PlaylistRunner.Data;

namespace ak.oss.PlaylistRunner.Services
{
    class TextFilePlaylistObtainer : IPlaylistObtainer
    {

        public Playlist GetPlaylist(IExtensionToTypeConverter extensionConverter, string filename)
        {
            try
            {
                string[] filelines = File.ReadAllLines(filename);


                string basePath = filelines[0].Split(',')[0];

                List<Clip> clips = new List<Clip>(filelines.Length - 1);

                for (int i = 1; i < filelines.Length; i++)
                {
                    string[] clipDetails = filelines[i].Split('\t');
                    if (clipDetails[0].StartsWith("#")) continue;

                    string path = Path.Combine(basePath, clipDetails[0]);
                    ClipType clipType = extensionConverter.GetTypeForExtension(Path.GetExtension(path));
                    clips.Add(new Clip()
                    {
                        FilePath = path,
                        Type = File.Exists(path) ? clipType : ClipType.Unknown,
                        DisplayName = clipDetails[1],
                        IsLoop = bool.Parse(clipDetails[2]),
                        Volume = double.Parse(clipDetails[3])
                    });
                }

                return new Playlist() { Clips = clips };
            }
            catch (Exception e) { return null; }
        }
    }
}
