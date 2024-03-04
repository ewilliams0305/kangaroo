using Microsoft.Extensions.Logging;
using System.Net;

namespace Kangaroo;

internal sealed class ScannerOptions
{
    public IEnumerable<IPAddress> IpAddresses { get; set; } = Enumerable.Empty<IPAddress>();

    public IPAddress? RangeStart { get; set; }

    public IPAddress? RangeStop { get; set; }

    public IPAddress? IpAddress { get; set; }

    public IPAddress SubnetMask { get; set; } = IPAddress.Parse("255.255.255.0");
    
    public bool Concurrent { get; set; } = false;

    public int ItemsPerBatch { get; set; } = 10;

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

    public int TimeToLive { get; set; } = 64;

    public ILogger Logger { get; set; } = new DefaultLogger();
}