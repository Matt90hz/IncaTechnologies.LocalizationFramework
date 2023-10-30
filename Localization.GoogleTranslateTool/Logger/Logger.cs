using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localization.GoogleTranslateTool.Logger
{
    internal class Logger : ILogger
    {
        public static ILogger Default { get; } = new Logger();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

        public bool IsEnabled(LogLevel logLevel) => logLevel switch
        {
            LogLevel.Trace =>
                #if TRACE
                    true,
                #else
                    false,
                #endif
            LogLevel.Debug =>
                #if DEBUG 
                    true,
                #else
                    false,
                #endif
            LogLevel.Information => true,
            LogLevel.Warning => true,
            LogLevel.Error => true,
            LogLevel.Critical => true,
            LogLevel.None => false,
            _ => false,
        };

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if(IsEnabled(logLevel))
            {
                Console.WriteLine($"[{eventId.Id,2}: {logLevel,-12}]");
                var message = formatter(state, exception);
                Console.WriteLine(message);
            }
        }
    }

}
