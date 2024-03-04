using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;

namespace Kangaroo.Queries;

internal sealed class QueryPingResultsOrderly : IQueryPingResults
{
    private readonly ILogger _logger;
    private readonly int _timeout;
    private readonly Ping _ping;
    private readonly PingOptions _options;

    public QueryPingResultsOrderly(ILogger logger,Ping ping, QueryOptions options)
    {
        _logger = logger;
        _timeout = options.ToTimeout();
        _ping = ping;
        _options = options.ToPingOptions();
    }

    public async Task<PingReply?> Query(IPAddress ipAddress, CancellationToken token = default)
    {
        try
        {
            var result = await _ping.SendPingAsync(ipAddress, _timeout, new byte[32], _options);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Ping failed for {ipAddress}", ipAddress);
            return null;
        }
    }

    #region IDisposable

    /// <inheritdoc />
    public void Dispose()
    {
        _ping.Dispose();
    }

    #endregion
}