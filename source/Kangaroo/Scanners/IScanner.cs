using System.Net;

namespace Kangaroo;

/// <summary>
/// The scanner, well scans...  A scanner will scan the entire range of IP addresses provided during the build configuration pipeline.
/// </summary>
public interface IScanner : IDisposable
{
    /// <summary>
    /// Queries all ip addresses provided during the build pipeline
    /// <seealso cref="ScannerBuilder"/>
    /// </summary>
    /// <param name="token">optional cancellation token.</param>
    /// <returns>The results of the scan.</returns>
    Task<ScanResults> QueryAddresses(CancellationToken token = default);

    /// <summary>
    /// Queries a single node on the network.
    /// </summary>
    /// <param name="ipAddress">Provided IP address to query</param>
    /// <param name="token">optional cancellation token.</param>
    /// <returns>The results of the scan.</returns>
    Task<NetworkNode> CheckNetworkNode(IPAddress ipAddress, CancellationToken token = default);
}