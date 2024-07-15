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
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(nameof(formatter));
#else
        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }
#endif

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

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return default!;
    }

#endregion
}