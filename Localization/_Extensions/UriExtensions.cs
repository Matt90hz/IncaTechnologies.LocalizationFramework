using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Localization.Extensions
{
    internal static class UriExtensions
    {
        internal static IEnumerable<Uri> Files(this Uri uri, string? extension = null, SearchOption searchOption = SearchOption.AllDirectories) => Directory.GetFiles(uri.LocalPath, extension ?? "*", searchOption).Select(f => new Uri(f));

        internal static string FileName(this Uri uri) => Path.GetFileName(uri.LocalPath);

        internal static string FileExtension(this Uri uri)
        {
            var lastDotIndex = uri.LocalPath.LastIndexOf('.');

            if (lastDotIndex == -1) return string.Empty;

            return uri.LocalPath.Substring(lastDotIndex + 1);
        }

        internal static Uri? FileWithExtension(this Uri uri, string ext) => uri.Files().FirstOrDefault(u => u.LocalPath.EndsWith(ext));

        internal static bool IsFileExtension(this Uri uri, string extension) => uri.LocalPath.EndsWith(extension);

        internal static Uri? ToUri(this string path)
        {
            try
            {
                return new Uri(path);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

}