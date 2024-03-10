using Kangaroo.Platforms;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Kangaroo.Queries;

internal sealed class NetworkQuerierFactory : IQueryFactory
{
    private readonly ILogger _logger;
    private readonly IQueryPingResults _ping;
    private readonly Func<HttpClient>? _clientFactory;

    public NetworkQuerierFactory(ILogger logger, IQueryPingResults ping, Func<HttpClient>? clientFactory = null)
    {
        _logger = logger;
        _ping = ping;
        _clientFactory = clientFactory;
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
                    http: _clientFactory != null 
                        ? new QueryWebServer(_logger, _clientFactory)
                        : null)
                : new QueryNetworkNode(
                    logger: _logger,
                    ping: _ping,
                    mac: new WindowsQueryMacAddress(_logger),
                    host: new QueryHostname(_logger),
                    http: _clientFactory != null 
                        ? new QueryWebServer(_logger, _clientFactory) 
                        : null);
    }
}