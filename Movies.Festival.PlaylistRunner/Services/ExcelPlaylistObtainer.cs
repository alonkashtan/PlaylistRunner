using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ak.oss.PlaylistRunner.Data;
using OfficeOpenXml;
using System.IO;

namespace ak.oss.PlaylistRunner.Services
{
    public class ExcelPlaylistObtainer : IPlaylistObtainer
    {
        public Playlist GetPlaylist(IExtensionToTypeConverter extensionConverter, string file)
        {
            using (ExcelPackage package = new ExcelPackage(new FileInfo(file)))
            {
                List<Clip> clips = new List<Clip>();
                buildPlaylist(extensionConverter, package, clips);
                AddMarks(package, clips);

                return new Playlist { Clips = clips };
            }
        }

        private static void buildPlaylist(IExtensionToTypeConverter extensionConverter, ExcelPackage package, List<Clip> clips)
        {
            string basePath = package.Workbook.Names["BasePath"].Text;
            var worksheet = package.Workbook.Worksheets["Playlist"];
            var table = worksheet.Tables["Playlist"];

            int tableLeft = table.Address.Start.Column;
            int displayNameIndex = tableLeft + table.Columns["Clip Display Name"].Position;
            int filePathIndex = tableLeft + table.Columns["File Path"].Position;
            int loopIndex = tableLeft + table.Columns["Loop"].Position;
            int volumeIndex = tableLeft + table.Columns["Volume"].Position;
            int skipIndex = tableLeft + table.Columns["Skip"].Position;

            // iterate table rows, skipping the header column
            for (int row = table.Address.Start.Row + 1; row <= table.Address.End.Row; row++)
            {
                string displayName = worksheet.Cells[row, displayNameIndex].Text;
                if (string.IsNullOrWhiteSpace(displayName) || worksheet.Cells[row, skipIndex].Text == "Yes")
                    continue;

                string path = worksheet.Cells[row, filePathIndex].Text;
                path = IsFullPath(path) ? path : Path.Combine(basePath, path);
                ClipType clipType = extensionConverter.GetTypeForExtension(Path.GetExtension(path));

                double volume = 0.5;
                double.TryParse(worksheet.Cells[row, volumeIndex].Text, out volume);
                clips.Add(new Clip
                {
                    FilePath = path,
                    Type = File.Exists(path) ? clipType : ClipType.Unknown,
                    DisplayName = displayName,
                    IsLoop = worksheet.Cells[row, loopIndex].Text == "Yes",
                    Volume = volume
                });
            }
        }

        private void AddMarks(ExcelPackage package, List<Clip> clips)
        {
            var worksheet = package.Workbook.Worksheets["Marks"];
            var table = worksheet.Tables["Marks"];

            int tableLeft = table.Address.Start.Column;
            int fileIndex = tableLeft + table.Columns["File"].Position;
            int timeIndex = tableLeft + table.Columns["time"].Position;
            int nameIndex = tableLeft + table.Columns["name"].Position;

            for (int row = table.Address.Start.Row + 1; row <= table.Address.End.Row; row++)
            {
                string file = worksheet.Cells[row, fileIndex].Text;
                if (string.IsNullOrWhiteSpace(file))
                    continue;

                TimeSpan time = TimeSpan.Parse(worksheet.Cells[row, timeIndex].Text);
                string name = worksheet.Cells[row, nameIndex].Text;

                clips.ForEach(clip =>
                {
                    if (Path.GetFileName(clip.FilePath) == file)
                        clip.Marks.Add(new Mark() { Location = time, Name = name });
                });
            }

            clips.ForEach(clip =>
            {
                clip.Marks.Sort((m1, m2) => m1.Location.CompareTo(m2.Location));
                for (int i = 0; i < clip.Marks.Count-1; i++)
                {
                    clip.Marks[i].Length = clip.Marks[i + 1].Location - clip.Marks[i].Location;
                }
            });
        }

        public static bool IsFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path.IndexOfAny(Path.GetInvalidPathChars()) != -1 || !Path.IsPathRooted(path))
                return false;

            var pathRoot = Path.GetPathRoot(path);
            if (pathRoot.Length <= 2 && pathRoot != "/") // Accepts X:\ and \\UNC\PATH, rejects empty string, \ and X:, but accepts / to support Linux
                return false;

            return !(pathRoot == path && pathRoot.StartsWith("\\\\") && pathRoot.IndexOf('\\', 2) == -1); // A UNC server name without a share name (e.g "\\NAME") is invalid
        }
    }
}
