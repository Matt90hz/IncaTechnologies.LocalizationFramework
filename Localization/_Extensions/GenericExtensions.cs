using System;
using System.Collections.Generic;
using System.Linq;

namespace Localization.Extensions
{
    internal static class GenericExtensions
    {
        internal static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) => enumerable is null || !enumerable.Any();

        internal static T WhenNull<T>(this T t, Func<T> func) => t == null ? func() : t;

        internal static U NotNull<U, T>(this IEnumerable<Func<T, U>> enumerable, T t)
        {
            foreach (var func in enumerable)
            {
                var result = func(t);

                if (result != null) return result;
            }

            return default!;
        }
    }

}