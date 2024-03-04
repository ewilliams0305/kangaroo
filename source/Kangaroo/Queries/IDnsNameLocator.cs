using System.Net;

namespace Kangaroo.Queries;

internal interface IDnsNameLocator
{
    Task<IPHostEntry?> GetHostname(IPAddress ipAddress, CancellationToken token = default);
}