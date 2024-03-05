using System.Net;

namespace Kangaroo.Queries;

internal interface IQueryWebServer : IDisposable
{
    Task<string> Query(IPAddress ipAddress, CancellationToken token = default);
}