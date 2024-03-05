using Kangaroo.Queries;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;

namespace Kangaroo;

internal sealed class ParallelScanner : IScanner
{

    internal static ParallelScanner CreateScanner(
        ILogger logger,
        IQueryNetworkNode querier,
        IEnumerable<IPAddress> addresses,
        int batchSize)
    {
        return new ParallelScanner(logger, querier, addresses, batchSize);
    }

    private readonly ILogger _logger;
    private readonly IQueryNetworkNode _querier;
    private readonly IEnumerable<IPAddress> _addresses;
    private readonly Stopwatch _stopWatch = new();
    private readonly int _batchSize;

    private ParallelScanner(ILogger logger, IQueryNetworkNode querier, IEnumerable<IPAddress> addresses, int batchSize)
    {
        _logger = logger;
        _querier = querier;
        _addresses = addresses;
        _batchSize = batchSize;
    }

    public async Task<ScanResults> QueryAddresses(CancellationToken token = default)
    {
        _stopWatch.Restart();
        
        var counter = 0;
        var nodes = new List<NetworkNode>();
        
        try
        {
            var batchOfTask = BatchedTaskFactoryIAsync(token);

            foreach(var batch in batchOfTask)
            {
                counter++;
                var batchedResult = await batch;

                var networkNodes = batchedResult as NetworkNode[] ?? batchedResult.ToArray();
                nodes.AddRange(networkNodes);
                _logger.LogDebug("Processed Batch #{counter} with #{itemsCount} items on Thread {threadId}", counter, networkNodes.Count(), Thread.CurrentThread.ManagedThreadId);
            }
            _stopWatch.Stop();

            return new ScanResults(nodes, _stopWatch.Elapsed, _addresses.Count(), nodes.Count(n => n.Alive), _addresses.First(), _addresses.Last());
        }
        catch (ArgumentNullException nullException)
        {
            _logger.LogCritical(nullException,"Failed testing batch of nodes");
            return ScanResults.Empty;
        }
        catch (ArgumentException argException)
        {
            _logger.LogCritical(argException, "Failed testing batch of nodes");
            return ScanResults.Empty;
        }
    }

    public async IAsyncEnumerable<NetworkNode> NetworkQueryAsync(IEnumerable<IPAddress> address, [EnumeratorCancellation] CancellationToken token = default)
    {
        foreach (var ip in address)
        {
            _logger.LogDebug("Processed Batch on Thread {threadId}", Thread.CurrentThread.ManagedThreadId);
            yield return await CheckNetworkNode(ip, token);
        }
    }

    private IEnumerable<Task<IEnumerable<NetworkNode>>> BatchedTaskFactoryIAsync(CancellationToken token = default) =>
        _addresses
            .Select((x, index) => new { Address = x, Index = index })
            .GroupBy(x => x.Index / _batchSize)
            .Select(g => ProcessBatchOfNodes(g.Select(a => a.Address), token))
            .ToList();

    private async Task<IEnumerable<NetworkNode>> ProcessBatchOfNodes(IEnumerable<IPAddress> nodesToQuery, CancellationToken token = default)
    {
        var results = new List<NetworkNode>();
        await foreach (var node in NetworkQueryAsync(nodesToQuery, token))
        {
            _logger.LogDebug("Processed Batch item {address} on Thread {threadId}",node.IpAddress, Thread.CurrentThread.ManagedThreadId);
            results.Add(node);
        }
        return results;
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
