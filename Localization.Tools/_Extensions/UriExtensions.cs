using Localization.Tools.ExeptionResult;
using Microsoft.CodeAnalysis;

namespace Localization.Tools.Extensions;

internal static class UriExtensions
{
    internal static IEnumerable<Uri> Files(this Uri uri, string? extension = null, SearchOption searchOption = SearchOption.AllDirectories) => Directory.GetFiles(uri.LocalPath, extension ?? "*", searchOption).Select(f => new Uri(f));

    internal static IEnumerable<Uri> DotCsFiles(this Uri uri, SearchOption searchOption = SearchOption.AllDirectories) => uri.Files("*.cs", searchOption);

    internal static Exception<string> ReadFile(this Uri uri)
    {
        try
        {
            return File.ReadAllText(uri.LocalPath);
        }
        catch (Exception e)
        {
            return e.ToException<string>();
        }
    }

    internal static bool UriContainsCsproj(this Uri uri) => Directory.GetFiles(uri.LocalPath).Any(f => f.EndsWith(".csproj"));

    internal static Uri? FileWithExtension(this Uri uri, string ext) => uri.Files().FirstOrDefault(u => u.LocalPath.EndsWith(ext));

}
