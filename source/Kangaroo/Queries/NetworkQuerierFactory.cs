using Kangaroo.Platforms;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Kangaroo.Queries;

internal sealed class NetworkQuerierFactory : IQueryFactory
{
    private readonly ILogger _logger;
    private readonly IQueryPingResults _ping;

    public NetworkQuerierFactory(ILogger logger, IQueryPingResults ping)
    {
        _logger = logger;
        _ping = ping;
    }

    /// <inheritdoc />
    public IQueryNetworkNode CreateQuerier()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) 
                ? new QueryNetworkNode(
                    logger: _logger,
                    ping: _ping,
                    mac: new LinuxQueryMacAddress(_logger),
                    host: new QueryHostname(_logger),
                    http: new QueryWebServer())
                : new QueryNetworkNode(
                    logger: _logger,
                    ping: _ping,
                    mac: new WindowsQueryMacAddress(_logger),
                    host: new QueryHostname(_logger),
                    http: new QueryWebServer());
    }
}