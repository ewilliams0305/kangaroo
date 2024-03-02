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

    public TimeSpan Timeout { get; set; } = TimeSpan.FromMilliseconds(500);

    public int TimeToLive { get; set; } = 64;

    public Action<Exception> ExceptionHandler { get; set; } = Console.WriteLine;

    public Action<string> MessageHandler { get; set; } = Console.WriteLine;

    public ScannerOptions()
    {

    }
}