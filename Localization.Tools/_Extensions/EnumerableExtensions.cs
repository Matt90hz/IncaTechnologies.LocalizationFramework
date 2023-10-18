using Microsoft.CodeAnalysis;

namespace Localization.Tools.Extensions;

internal static class EnumerableExtensions
{
    internal static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();

}
