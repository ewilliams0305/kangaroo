

using System.Net;
using System.Net.NetworkInformation;

namespace Kangaroo.Queries;

internal interface IQueryPingResults
{
    Task<PingReply?> Query(IPAddress ipAddress, CancellationToken token = default);
}