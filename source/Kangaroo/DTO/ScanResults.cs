using System.Net;

namespace Kangaroo;

public record ScanResults(
    IEnumerable<NetworkNode> Nodes,
    TimeSpan ElapsedTime,
    int NumberOfAddressesScanned,
    IPAddress EndAddress,
    IPAddress StartAddress)
{
    internal static ScanResults Empty => new(Array.Empty<NetworkNode>(), TimeSpan.MinValue, 0, IPAddress.Any, IPAddress.Any);
    
};