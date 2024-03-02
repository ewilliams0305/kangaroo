using System.Net;

namespace Kangaroo;

public interface IScanner : IDisposable
{
    Task<IEnumerable<NetworkNode>> QueryAddresses(CancellationToken token = default);
    IAsyncEnumerable<NetworkNode> NetworkQueryAsync(CancellationToken token = default);
    Task<IEnumerable<NetworkNode>> NetworkQueryConcurrent(CancellationToken token = default);
    Task<NetworkNode> CheckNetworkNode(IPAddress ipAddress, CancellationToken token = default);
}