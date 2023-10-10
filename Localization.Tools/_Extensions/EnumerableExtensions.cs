using Microsoft.CodeAnalysis;

namespace Localization.Tools.Extensions;

internal static class EnumerableExtensions
{
    private static readonly Action _defaultLog = () => Console.WriteLine("The collection is empty.");

    internal static IEnumerable<T> LogIfEmpty<T>(this IEnumerable<T> values, Action? log = null)
    {
        log ??= _defaultLog;

        if(!values.Any()) log();

        return values;
    }

    internal static IEnumerable<T> LogIfEmpty<T>(this IEnumerable<T> values, string messsage) => values.LogIfEmpty(() => Console.WriteLine(messsage));
}
