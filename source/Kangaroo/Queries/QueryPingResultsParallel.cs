using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;

namespace Kangaroo.Queries;

internal sealed class QueryPingResultsParallel : IQueryPingResults
{
    private readonly ILogger _logger;
    private readonly PingOptions _options;
    private readonly int _timeout;

    public QueryPingResultsParallel(ILogger logger, QueryOptions options)
    {
        _logger = logger;
        _options = options.ToPingOptions();
        _timeout = options.ToTimeout();
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

    #region IDisposable

    /// <inheritdoc />
    public void Dispose()
    {
        // TODO Determine if this interface even needs to be disposable or just the orderly scanner
    }

    #endregion
}