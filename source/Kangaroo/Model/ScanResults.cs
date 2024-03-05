using System.Net;

namespace Kangaroo;

/// <summary>
/// Results of a network scan.  
/// </summary>
/// <param name="Nodes">All nodes scanned with results of each node.  This includes nodes not found.</param>
/// <param name="ElapsedTime">Total elapsed time to query all addresses</param>
/// <param name="NumberOfAddressesScanned">Total number of device queried</param>
/// <param name="NumberOfAliveNodes">Total number of alive nodes</param>
/// <param name="StartAddress">The first address scanned</param>
/// <param name="EndAddress">The last address scanned</param>
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