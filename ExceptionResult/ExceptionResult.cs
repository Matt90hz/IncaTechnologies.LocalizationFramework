using System;
using System.Runtime.CompilerServices;

namespace ExceptionResult
{
    public class Exception<T> : Exception
    {
        public T ValueOrDefault { get; } = default!;

        public Exception(T value)
        {
            ValueOrDefault = value;
        }

        public Exception()
        {

        }

        public Exception(string message) : base(message)
        {

        }

        public Exception(string message, Exception innerException) : base(message, innerException)
        {
            
        }

        public static implicit operator Exception<T>(T t) => new Exception<T>(t);
        public static implicit operator T(Exception<T> exception) => exception.ValueOrDefault;

    }

    public static class ExceptionExtensions
    {
        public static Exception<T> ToException<T>(this T t) => new Exception<T>(t);

        public static T Throw<T>(this Exception<T> exception) => throw exception;

        public static T Value<T>(this Exception<T> exception) => exception.ValueOrDefault is null || (exception.ValueOrDefault is int value && value == default) || (exception.ValueOrDefault is bool flag && flag == default) ? throw exception : exception.ValueOrDefault;
    }
}
