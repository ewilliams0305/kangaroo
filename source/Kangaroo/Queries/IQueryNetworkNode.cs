using System.Net;

namespace Kangaroo.Queries;

internal interface IQueryNetworkNode : IDisposable
{
    Task<NetworkNode> Query(IPAddress ipAddress, CancellationToken token = default);
}