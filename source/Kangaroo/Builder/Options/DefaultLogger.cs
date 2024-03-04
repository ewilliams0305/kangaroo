using Microsoft.Extensions.Logging;

namespace Kangaroo;

internal sealed class DefaultLogger : ILogger<IScanner>
{
    #region Implementation of ILogger

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        var message = formatter(state, exception);

        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logLevel}] {message}");
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return default!;
    }

    #endregion
}