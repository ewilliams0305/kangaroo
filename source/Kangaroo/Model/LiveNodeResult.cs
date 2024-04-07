using System.Net;

namespace Kangaroo;

public record LiveNodeResult(
    TimeSpan QueryTime,
    TimeSpan Latency,
    int NumberOfAddressesScanned,
    int NumberOfAliveNodes,
    IPAddress ScannedAddress)
{
    internal static LiveNodeResult Empty => new(TimeSpan.Zero, TimeSpan.Zero, 0, 0, IPAddress.Any);
};