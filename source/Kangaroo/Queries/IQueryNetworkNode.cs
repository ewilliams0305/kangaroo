using System.Net;

namespace Kangaroo.Queries;

internal interface IQueryNetworkNode
{
    Task<NetworkNode> Query(IPAddress ipAddress, CancellationToken token = default);
}