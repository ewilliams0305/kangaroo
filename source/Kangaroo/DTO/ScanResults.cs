using System.Net;

namespace Kangaroo;

public record ScanResults(
    IEnumerable<NetworkNode> Nodes,
    TimeSpan ElapsedTime,
    int NumberOfAddressesScanned,
    IPAddress StartAddress,
    IPAddress EndAddress
    )
{
    internal static ScanResults Empty => new(Array.Empty<NetworkNode>(), TimeSpan.MinValue, 0, IPAddress.Any, IPAddress.Any);
};