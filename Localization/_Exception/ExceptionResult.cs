using System;
using System.Runtime.CompilerServices;

namespace Localization.ExeptionResult
{
    internal class Exception<T> : Exception
    {
        internal T ValueOrDefault { get; } = default!;

        internal Exception(T value)
        {
            ValueOrDefault = value;
        }

        internal Exception()
        {

        }

        internal Exception(string message) : base(message)
        {

        }

        internal Exception(string message, Exception innerException) : base(message, innerException)
        {
            
        }

        public static implicit operator Exception<T>(T t) => new Exception<T>(t);
        public static implicit operator T(Exception<T> exception) => exception.ValueOrDefault;

    }

    internal static class ExceptionExtensions
    {
        internal static Exception<T> ToException<T>(this T t) => new Exception<T>(t);

        internal static T Throw<T>(this Exception<T> exception) => throw exception;

        internal static T Value<T>(this Exception<T> exception) => exception.ValueOrDefault is null || (exception.ValueOrDefault is int value && value == default) || (exception.ValueOrDefault is bool flag && flag == default) ? throw exception : exception.ValueOrDefault;
    }
}
