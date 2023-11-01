namespace Localization.GoogleTranslateTool._Extensions;

internal static class UriExtensions
{
    public static bool IsDirectory(this Uri uri) => uri.IsFile && File.GetAttributes(uri.LocalPath) is FileAttributes.Directory;
}