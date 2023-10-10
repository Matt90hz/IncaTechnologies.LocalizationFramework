using System;
using System.Collections.Generic;
using System.Text;

namespace Localization.Tools.Extensions;

internal static class GenericExtensions
{
    internal static Exception? Try(this Action action)
    {
        try
        {
            action();
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    internal static T? Try<T>(this Func<T> func, Action<Exception> fail)
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            fail(ex);
            return default;
        }
    }

    internal static U Try<T, U>(this Func<T> func, Func<T, U> success, Func<Exception, U> fail)
    {
        try
        {
            var t = func();
            return success(t);
        }
        catch (Exception ex)
        {
            return fail(ex);
        }
    }

    internal static (U, Exception?) TryCatch<T, TException, U>(this Func<T> func, Func<T, U> success, Func<Exception, U> fail)
    {
        try
        {
            var t = func();
            return (success(t), null);
        }
        catch (Exception ex)
        {
            return (fail(ex), ex);
        }
    }

    internal static U Apply<T, U>(this T t, Func<T, U> func) => func(t);

    internal static T Do<T>(this T t, Action<T> action)
    {
        action(t);
        return t;
    }

    internal static T DoWhen<T>(this T t, bool condition, Action<T> action)
    {
        if(condition) action(t);
        return t;
    }

    internal static T If<T>(this T t, Func<T, bool> condition, Action<T> action)
    {
        if (condition(t)) action(t);
        return t;
    }

    internal static T If<T, U>(this T t, Func<T, bool> condition, Func<T, U> bind)
    {
        if (condition(t)) bind(t);
        return t;
    }

    internal static T IfNot<T>(this T t, Func<T, bool> condition, Action<T> action)
    {
        if (!condition(t)) action(t);
        return t;
    }

    internal static T IfNot<T, U>(this T t, Func<T, bool> condition, Func<T, U> bind)
    {
        if (!condition(t)) bind(t);
        return t;
    }

    internal static bool EqualsOrDefault<T>(this T? t, object? other) => t?.Equals(other) ?? false;
}
