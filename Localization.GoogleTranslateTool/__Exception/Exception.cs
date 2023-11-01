namespace Localization.GoogleTranslateTool.__Exception;

internal class Exception<T> : Exception
{
    public bool IsThrown { get; } = true;

    public T? ValueOrDefult { get; }

    public T ValueOrThrow => IsThrown ? throw this : ValueOrDefult!;

    public Exception(T value)
    {
        IsThrown = false;
        ValueOrDefult = value;
    }

    public Exception(string message) : base(message) { }

    public Exception(string message, Exception inner) : base(message, inner) { }

    public static implicit operator Exception<T>(T value) => new(value);
    public static implicit operator T(Exception<T> exception) => exception.ValueOrThrow;
}
