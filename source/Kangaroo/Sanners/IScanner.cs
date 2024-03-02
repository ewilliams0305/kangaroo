using System.Net;

namespace Kangaroo;

public interface IScanner : IDisposable
{
    Task<ScanResults> QueryAddresses(CancellationToken token = default);
    Task<NetworkNode> CheckNetworkNode(IPAddress ipAddress, CancellationToken token = default);
}