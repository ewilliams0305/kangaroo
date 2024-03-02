using Microsoft.Extensions.Logging;
using System.Net;

namespace Kangaroo;

internal sealed class ScannerOptions
{
    public IEnumerable<IPAddress> IpAddresses { get; set; } = Enumerable.Empty<IPAddress>();

    public IPAddress? RangeStart { get; set; }

    public IPAddress? RangeStop { get; set; }

    public IPAddress? IpAddress { get; set; }

    public byte NetMask { get; set; } = 0x24;

    public bool Concurrent { get; set; } = false;

    public int ItemsPerBatch { get; set; } = 10;

    public TimeSpan Timeout { get; set; } = TimeSpan.FromMilliseconds(500);

    public int TimeToLive { get; set; } = 64;

    public ILogger Logger { get; set; } = new Logger();

    //public Action<Exception> ExceptionHandler { get; set; } = Console.WriteLine;

    //public Action<string> MessageHandler { get; set; } = Console.WriteLine;

    public ScannerOptions()
    {

    }
}

internal sealed class Logger : ILogger<IScanner>
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