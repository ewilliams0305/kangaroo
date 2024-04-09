using Kangaroo.Queries;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;

namespace Kangaroo;

internal sealed class SingleScanner : IScanner
{
    public static IScannerIpConfiguration Configure()
    {
        return new ScannerBuilder();
    }

    internal static SingleScanner CreateScanner(
        ILogger logger,
        IQueryNetworkNode querier,
        IPAddress address)
    {
        return new SingleScanner(logger, querier, address);
    }

    /// <inheritdoc />
    public Action<ScanResults, LiveUpdateStatus> ScanStatusUpdate { get; set; }

    /// <inheritdoc />
    public Action<NetworkNode, LiveUpdateStatus> NodeStatusUpdate { get; set; }

    private readonly ILogger _logger;
    private readonly IQueryNetworkNode _querier;
    private readonly IPAddress _address;
    private readonly Stopwatch _stopWatch = new();

    private SingleScanner(ILogger logger, IQueryNetworkNode querier, IPAddress address)
    {
        _logger = logger;
        _querier = querier;
        _address = address;
    }

    public async Task<ScanResults> QueryNetwork(CancellationToken token = default)
    {
        ScanStatusUpdate?.Invoke(ScanResults.Initial(1), LiveUpdateStatus.Started);
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
            1, 
            nodes.Count(n => n.Alive),
            _address, 
            _address)
            .PublishResults(ScanStatusUpdate);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<NetworkNode> QueryNetworkNodes(CancellationToken token = default)
    {
        return NetworkQueryAsync(token);
    }

    public async Task<NetworkNode> CheckNetworkNode(IPAddress ipAddress, CancellationToken token = default)
    {
        NodeStatusUpdate?.Invoke(NetworkNode.InProgress(ipAddress), LiveUpdateStatus.Started);
        var node = await _querier.Query(ipAddress, token);
        return node.PublishStatus(NodeStatusUpdate);
    }

    private async IAsyncEnumerable<NetworkNode> NetworkQueryAsync([EnumeratorCancellation] CancellationToken token = default)
    {
        yield return await CheckNetworkNode(_address, token);
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