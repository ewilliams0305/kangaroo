using System.Net;

namespace Kangaroo;

/// <summary>
/// Data returned from a scanned node
/// </summary>
/// <param name="QueryTime">The time the query took</param>
/// <param name="Latency">The node latency reported from a ping test</param>
/// <param name="NumberOfAddressesScanned">The number of scanned network addresses.</param>
/// <param name="NumberOfAliveNodes">The number of active network addresses</param>
/// <param name="ScannedAddress">The addresses scanned</param>
public record LiveNodeResult(
    TimeSpan QueryTime,
    TimeSpan Latency,
    int NumberOfAddressesScanned,
    int NumberOfAliveNodes,
    IPAddress ScannedAddress)
{
    internal static LiveNodeResult Empty => new(TimeSpan.Zero, TimeSpan.Zero, 0, 0, IPAddress.Any);
};
