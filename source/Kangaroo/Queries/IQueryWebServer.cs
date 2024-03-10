using System.Net;

namespace Kangaroo.Queries;

internal interface IQueryWebServer
{
    Task<string> Query(IPAddress ipAddress, CancellationToken token = default);
}