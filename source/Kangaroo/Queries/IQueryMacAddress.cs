using System.Net;

namespace Kangaroo.Queries;

internal interface IQueryMacAddress
{
    Task<MacAddress> Query(IPAddress ipAddress, CancellationToken token);
}