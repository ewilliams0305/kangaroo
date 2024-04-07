using Kangaroo.Queries;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;

namespace Kangaroo;

internal sealed class OrderlyScanner : IScanner
{
    public static IScannerIpConfiguration Configure()
    {
        return new ScannerBuilder();
    }

    internal static OrderlyScanner CreateScanner(
        ILogger logger,
        IQueryNetworkNode querier,
        IEnumerable<IPAddress> addresses)
    {
        return new OrderlyScanner(logger, querier, addresses);
    }

    private readonly ILogger _logger;
    private readonly IQueryNetworkNode _querier;
    private readonly IEnumerable<IPAddress> _addresses;
    private readonly Stopwatch _stopWatch = new();

    private OrderlyScanner(ILogger logger, IQueryNetworkNode querier, IEnumerable<IPAddress> addresses)
    {
        _logger = logger;
        _querier = querier;
        _addresses = addresses;
    }

    public async Task<ScanResults> QueryNetwork(CancellationToken token = default)
    {
        _stopWatch.Restart();

        var nodes = new List<NetworkNode>();

        await foreach (var node in NetworkQueryAsync(null, token))
        {
            _logger.LogInformation("{node}", node);
            nodes.Add(node);
        }

        _stopWatch.Stop();
        return new ScanResults(nodes, _stopWatch.Elapsed, _addresses.Count(), nodes.Count(n => n.Alive), _addresses.First(), _addresses.Last());
    }

    /// <inheritdoc />
    public IAsyncEnumerable<NetworkNode> QueryNetworkNodes(Action<LiveNodeResult>? resultsHandler = null, CancellationToken token = default)
    {
        return NetworkQueryAsync(resultsHandler, token);
    }

    public async Task<NetworkNode> CheckNetworkNode(IPAddress ipAddress, CancellationToken token = default) =>
        await _querier.Query(ipAddress, token);

    private async IAsyncEnumerable<NetworkNode> NetworkQueryAsync(Action<LiveNodeResult>? resultsHandler = null, [EnumeratorCancellation] CancellationToken token = default)
    {
        var results = new LiveNodeResult(TimeSpan.Zero, TimeSpan.Zero, 0, 0, _addresses.First());

        foreach (var ip in _addresses)
        {
            var result = await CheckNetworkNode(ip, token);
            
            resultsHandler?.Invoke(
                new LiveNodeResult(
                    ScannedAddress: result.IpAddress, 
                    Latency: result.Latency ?? TimeSpan.Zero,
                    QueryTime: result.QueryTime, 
                    NumberOfAddressesScanned: results.NumberOfAddressesScanned + 1, 
                    NumberOfAliveNodes: result.Alive 
                        ? results.NumberOfAliveNodes + 1
                        : results.NumberOfAliveNodes));

            yield return result;
        }
    }

    #region IDisposable

    private bool _disposed = false;

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _stopWatch.Stop();
            _querier.Dispose();
        }
        _disposed = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
    }

    #endregion
}