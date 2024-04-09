using Kangaroo.Queries;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

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

    /// <inheritdoc />
    public Action<ScanResults, LiveUpdateStatus> ScanStatusUpdate { get; set; }

    /// <inheritdoc />
    public Action<NetworkNode, LiveUpdateStatus> NodeStatusUpdate { get; set; }

    private OrderlyScanner(ILogger logger, IQueryNetworkNode querier, IEnumerable<IPAddress> addresses)
    {
        _logger = logger;
        _querier = querier;
        _addresses = addresses;
    }

    public async Task<ScanResults> QueryNetwork(CancellationToken token = default)
    {
        ScanStatusUpdate?.Invoke(ScanResults.Initial(_addresses.Count()), LiveUpdateStatus.Started);
        _stopWatch.Restart();

        var nodes = new List<NetworkNode>();

        await foreach (var node in NetworkQueryAsync(token))
        {
            _logger.LogInformation("{node}", node);
            nodes.Add(node);
        }

        _stopWatch.Stop();
        return new ScanResults(
            nodes, 
            _stopWatch.Elapsed, 
            _addresses.Count(), 
            nodes.Count(n => n.Alive), 
            _addresses.First(), 
            _addresses.Last())
            .PublishResults(ScanStatusUpdate);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<NetworkNode> QueryNetworkNodes(CancellationToken token = default)
    {
        return NetworkQueryAsync(token);
    } 
    
    private async IAsyncEnumerable<NetworkNode> NetworkQueryAsync([EnumeratorCancellation] CancellationToken token = default)
    {
        foreach (var ip in _addresses)
        {
            NodeStatusUpdate?.Invoke(NetworkNode.InProgress(ip), LiveUpdateStatus.Started);
            var result = await CheckNetworkNode(ip, token);
            yield return result.PublishStatus(NodeStatusUpdate);
        }
    }

    public async Task<NetworkNode> CheckNetworkNode(IPAddress ipAddress, CancellationToken token = default) =>
        await _querier.Query(ipAddress, token);


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