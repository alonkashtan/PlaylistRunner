using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ak.oss.PlaylistRunner.Data;
using System.Configuration;

namespace ak.oss.PlaylistRunner.Services
{
    public interface IExtensionToTypeConverter
    {
        ClipType GetTypeForExtension(string extension);
    }

    public class ExtensionToTypeConverter : IExtensionToTypeConverter
    {
        Dictionary<string, ClipType> _extensionToClipTypeMapping = new Dictionary<string, ClipType>();

        public ExtensionToTypeConverter()
        {
            foreach (var extension in ConfigurationManager.AppSettings["VideoFileExtensions"].Split(';'))
            {
                _extensionToClipTypeMapping.Add(extension, ClipType.Video);
            }

            foreach (var extension in ConfigurationManager.AppSettings["ImageFileExtensions"].Split(';'))
            {
                _extensionToClipTypeMapping.Add(extension, ClipType.Image);
            }

            foreach (var extension in ConfigurationManager.AppSettings["AudioFileExtensions"].Split(';'))
            {
                _extensionToClipTypeMapping.Add(extension, ClipType.Audio);
            }
        }

        public ClipType GetTypeForExtension(string extension)
        {
            if (_extensionToClipTypeMapping.ContainsKey(extension))
                return _extensionToClipTypeMapping[extension];
            else return ClipType.Unknown;
        }
    }
}
