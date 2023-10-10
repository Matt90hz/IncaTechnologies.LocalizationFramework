using Microsoft.CodeAnalysis;

namespace Localization.Tools.Extensions;

internal static class UriExtensions
{
    private static readonly Action<Exception> _defaultLog = e => Console.WriteLine(e.Message);

    internal static IEnumerable<Uri> GetFiles(this Uri uri, string? extension = null, SearchOption searchOption = SearchOption.AllDirectories) => Directory.GetFiles(uri.LocalPath, extension ?? "*", searchOption).Select(f => new Uri(f));

    internal static IEnumerable<Uri> GetDotCsFiles(this Uri uri, SearchOption searchOption = SearchOption.AllDirectories) => uri.GetFiles("*.cs", searchOption);

    internal static string TryReadFile(this Uri uri, Action<Exception>? log = null)
    {
        try
        {
            return File.ReadAllText(uri.LocalPath);
        }
        catch (Exception e)
        {
            log ??= _defaultLog;
            log(e);
            return string.Empty;
        }
    }

    internal static bool UriContainsCsproj(this Uri uri) => Directory.GetFiles(uri.LocalPath).Any(f => f.EndsWith(".csproj"));

}
