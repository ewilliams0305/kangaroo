using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;

namespace Kangaroo.Queries;

internal sealed class QueryNetworkNode: IQueryNetworkNode
{
    private readonly ILogger _logger;
    private readonly IQueryPingResults _ping;
    private readonly IQueryMacAddress _mac;
    private readonly IQueryHostname _host;

    public QueryNetworkNode(ILogger logger, IQueryPingResults ping, IQueryMacAddress mac, IQueryHostname host)
    {
        _logger = logger;
        _ping = ping;
        _mac = mac;
        _host = host;
    }

    /// <inheritdoc />
    public async Task<NetworkNode> Query(IPAddress ipAddress, CancellationToken token = default)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            var reply = await _ping.Query(ipAddress, token);

            if (reply is not { Status: IPStatus.Success })
            {
                stopwatch.Stop();
                var badNode = NetworkNode.BadNode(ipAddress, stopwatch.Elapsed);
                _logger.LogInformation("{node}", badNode);
                return badNode;
            }

            var mac = await _mac.Query(ipAddress, token);
            var host = await _host.Query(ipAddress, token);

            stopwatch.Stop();
            var node = new NetworkNode(
                ipAddress,
                mac,
                host != null ? host.HostName : "N/A",
                TimeSpan.FromMilliseconds(reply.RoundtripTime),
                stopwatch.Elapsed,
                true);

            _logger.LogDebug("Processed Batch node {node} on Thread {threadId}", node, Thread.CurrentThread.ManagedThreadId);
            return node;
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed testing node {ipAddress}", ipAddress);
            return NetworkNode.BadNode(ipAddress, stopwatch.Elapsed);
        }
    }

}