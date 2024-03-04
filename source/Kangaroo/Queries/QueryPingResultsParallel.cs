using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;

namespace Kangaroo.Queries;

internal sealed class QueryPingResultsParallel : IQueryPingResults
{
    private readonly ILogger _logger;
    private readonly int _timeout;
    private readonly PingOptions _options;

    public QueryPingResultsParallel(ILogger logger, TimeSpan timeout, PingOptions options)
    {
        _logger = logger;
        _timeout = timeout.Milliseconds;
        _options = options;
    }

    public async Task<PingReply?> Query(IPAddress ipAddress, CancellationToken token = default)
    {
        try
        {
            using var ping = new Ping();
            var result = await ping.SendPingAsync(ipAddress, _timeout, new byte[32], _options);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Ping failed for {ipAddress}", ipAddress);
            return null;
        }
    }
}