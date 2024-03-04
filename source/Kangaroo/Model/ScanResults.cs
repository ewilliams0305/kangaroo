using System.Net;

namespace Kangaroo;

public record ScanResults(
    IEnumerable<NetworkNode> Nodes,
    TimeSpan ElapsedTime,
    int NumberOfAddressesScanned,
    int NumberOfAliveNodes,
    IPAddress StartAddress,
    IPAddress EndAddress
    )
{
    internal static ScanResults Empty => new(Array.Empty<NetworkNode>(), TimeSpan.MinValue, 0, 0, IPAddress.Any, IPAddress.Any);
};