using Microsoft.Extensions.Logging;

namespace Advisor.Tests.Helpers;

public class InMemoryLoggerProvider : ILoggerProvider
{
    private readonly List<string> _logMessages;

    public InMemoryLoggerProvider(List<string> logMessages)
    {
        _logMessages = logMessages;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new InMemoryLogger(_logMessages);
    }

    public void Dispose() { }

    private class InMemoryLogger : ILogger
    {
        private readonly List<string> _logMessages;

        public InMemoryLogger(List<string> logMessages)
        {
            _logMessages = logMessages;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logMessages.Add(formatter(state, exception));
        }
    }
}

