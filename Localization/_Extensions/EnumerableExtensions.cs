using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Localization.Extensions
{
    internal static class EnumerableExtensions
    {
        internal static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) => enumerable is null || !enumerable.Any();
    }
}
