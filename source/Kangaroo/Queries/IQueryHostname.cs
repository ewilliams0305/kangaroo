using System.Net;

namespace Kangaroo.Queries;

internal interface IQueryHostname
{
    Task<IPHostEntry?> Query(IPAddress ipAddress, CancellationToken token = default);
}