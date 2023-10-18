using System;
using System.Runtime.CompilerServices;

namespace Localization.Tools.ExeptionResult
{
    internal class Exception<T> : Exception
    {
        internal T? ValueOrDefault { get; } = default;

        internal Exception(T value) => ValueOrDefault = value;      

        internal Exception() { }

        internal Exception(string message) : base(message) { }

        internal Exception(string message, Exception? innerException) : base(message, innerException) { }

        public static implicit operator Exception<T>(T t) => new(t);
        public static implicit operator T?(Exception<T> exception) => exception.ValueOrDefault;

    }

    internal static class ExceptionExtensions
    {
        internal static Exception<T> ToException<T>(this T t) => new(t);

        internal static Exception<T> ToException<T>(this Exception e) => new(e.Message, e.InnerException);

        internal static T Throw<T>(this Exception<T> exception) => throw exception;

        internal static T Value<T>(this Exception<T> exception) => exception.ValueOrDefault ?? throw exception;
    }
}
