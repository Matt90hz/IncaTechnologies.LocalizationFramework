using Microsoft.Extensions.Logging;

namespace Localization.GoogleTranslateTool.Logger;

internal class TranslatorLogger : ILogger<Translator.Translator>
{
    private readonly Func<LogLevel, bool> _filter;
    private readonly object _lock = new();

    public static ILogger<Translator.Translator> Default { get; } = new TranslatorLogger(_ => true);

    public TranslatorLogger(Func<LogLevel, bool> filter)
    {
        _filter = filter;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

    public bool IsEnabled(LogLevel logLevel) => _filter(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (IsEnabled(logLevel))
        {
            lock (_lock)
            {
                Console.ForegroundColor = logLevel switch
                {
                    LogLevel.Trace => ConsoleColor.White,
                    LogLevel.Debug => ConsoleColor.White,
                    LogLevel.Information => ConsoleColor.White,
                    LogLevel.Warning => ConsoleColor.Yellow,
                    LogLevel.Error => ConsoleColor.Red,
                    LogLevel.Critical => ConsoleColor.Red,
                    LogLevel.None => ConsoleColor.White,
                    _ => ConsoleColor.White,
                };

                Console.WriteLine(state);
                if (exception is not null) Console.WriteLine(exception.Message);

                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
