using System;

namespace Localization.ExceptionResult
{
    internal class Exception<T> : Exception
    {
        internal bool IsError { get; }

        internal T ValueOrDefault { get; } = default!;

        internal Exception(T value)
        {
            IsError = false;
            ValueOrDefault = value;
        }

        internal Exception() 
        {
            IsError = true;
        }

        internal Exception(string message) : base(message) 
        {
            IsError = false;
        }

        internal Exception(string message, Exception innerException) : base(message, innerException)
        {
            IsError = true;
        }

        public static implicit operator Exception<T>(T t) => new Exception<T>(t);
        public static implicit operator T(Exception<T> exception) => exception.ValueOrDefault;
    }

    internal static class ExceptionExtensions
    {
        internal static Exception<T> ToException<T>(this T t) => new Exception<T>(t);

        internal static Exception<T> ToException<T>(this Exception e) => new Exception<T>(e.Message, e.InnerException);

        internal static T Throw<T>(this Exception<T> exception) => throw exception;

        internal static T Value<T>(this Exception<T> exception) => exception.ValueOrDefault is null
            || (exception.ValueOrDefault is int value && value == default)
            || (exception.ValueOrDefault is bool flag && flag == default)
            ? throw exception
            : exception.ValueOrDefault;
    }
}
