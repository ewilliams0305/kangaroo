using Kangaroo.Platforms;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kangaroo
{
    internal sealed class ParallelScanner : IScanner
    {
        /// <summary>
        /// Factory used to create a new instance of the scanner.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="addresses"></param>
        /// <param name="timeout"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        internal static ParallelScanner CreateScanner(
            ILogger logger,
            IEnumerable<IPAddress> addresses,
            int batchSize,
            int timeout
            )
        {
            return new ParallelScanner(logger, addresses, batchSize, timeout);
        }

        private readonly ILogger _logger;
        private readonly IEnumerable<IPAddress> _addresses;
        private readonly Stopwatch _stopWatch = new();
        private readonly PingOptions _pingOptions;
        private readonly int _batchSize;
        private readonly int _timeout;

        private ParallelScanner(ILogger logger, IEnumerable<IPAddress> addresses, int batchSize, int timeout)
        {
            _logger = logger;
            _addresses = addresses;
            _batchSize = batchSize;
            _timeout = timeout;
            _pingOptions = new PingOptions(ttl: 5, false);
        }

        

        public async Task<ScanResults> QueryAddresses(CancellationToken token = default)
        {
            _stopWatch.Restart();
            
            var counter = 0;
            var results = new List<NetworkNode>();
            
            try
            {
                var batchOfTask = BatchedTaskFactoryIAsync(token);

                foreach(var batch in batchOfTask)
                {
                    counter++;
                    var batchedResult = await batch;

                    var networkNodes = batchedResult as NetworkNode[] ?? batchedResult.ToArray();
                    results.AddRange(networkNodes);
                    _logger.LogDebug("Processed Batch #{counter} with #{itemsCount} items on Thread {threadId}", counter, networkNodes.Count(), Thread.CurrentThread.ManagedThreadId);
                }

                _stopWatch.Stop();

                return new ScanResults(results, _stopWatch.Elapsed, _addresses.Count(), _addresses.First(), _addresses.Last());
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

        public async Task<NetworkNode> CheckNetworkNode(IPAddress ipAddress, CancellationToken token = default)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            try
            {
                var reply = await PingNode(ipAddress, token);

                if (reply is not { Status: IPStatus.Success })
                {
                    stopwatch.Stop();
                    var badNode = NetworkNode.BadNode(ipAddress, stopwatch.Elapsed);
                    _logger.LogInformation("{node}", badNode);
                    return badNode;
                }

                var mac = await GetMacAddressAsync(ipAddress, token);
                var host = await GetHostname(ipAddress, token);

                stopwatch.Stop();
                var node = new NetworkNode(
                    ipAddress,
                    mac, 
                    host != null ? host.HostName : "N/A",
                    TimeSpan.FromMilliseconds(reply.RoundtripTime),
                    stopwatch.Elapsed,
                    true);

                _logger.LogDebug("Processed Batch item {address} on Thread {threadId}", node.IpAddress, Thread.CurrentThread.ManagedThreadId);
                _logger.LogInformation("{node}", node);
                return node;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Failed testing node {ipAddress}", ipAddress);
                return NetworkNode.BadNode(ipAddress, stopwatch.Elapsed);
            }
        }

        public async Task<PingReply?> PingNode(IPAddress ipAddress, CancellationToken token = default)
        {
            try
            {
                using var ping = new Ping();
                var result = await ping.SendPingAsync(ipAddress, _timeout, new byte[32], _pingOptions);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Ping failed for {ipAddress}", ipAddress);
                return null;
            }
        }

        private async Task<IPHostEntry?> GetHostname(IPAddress ipAddress, CancellationToken token = default)
        {
            try
            {
                var ipHostEntry = await Dns.GetHostEntryAsync(ipAddress);
                return ipHostEntry;
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogCritical(argumentException, "Failed obtaining the DNS name {ipAddress}", ipAddress);
            }
            catch (SocketException socketError)
            {
                _logger.LogCritical(socketError, "Failed obtaining the DNS name {ipAddress}", ipAddress);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Failed obtaining the DNS name {ipAddress}", ipAddress);
            }

            return null;
        }

        public Task<MacAddress> GetMacAddressAsync(IPAddress ipAddress, CancellationToken token)
        {
            try
            {
                //return await Task.Run(() =>
                //{
                    var macAddr = new byte[6];
                    var macAddrLen = macAddr.Length;
                    var macAddrLenUlong = (uint)macAddrLen;

                    if (WindowsArp.SendARP(BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0), 0, macAddr, ref macAddrLenUlong) != 0)
                    {
                        _logger.LogDebug("Failed obtaining the MAC address for {ipAddress}", ipAddress);
                        return Task.FromResult(MacAddress.Empty);
                    }

                    return Task.FromResult(new MacAddress(macAddr));

                    //var macAddress = new StringBuilder();
                    //for (var i = 0; i < macAddrLen; i++)
                    //{
                    //    macAddress.Append(macAddr[i].ToString("X2"));
                    //    if (i != macAddrLen - 1)
                    //        macAddress.Append(':');
                    //}

                    //return macAddress.ToString();
                //}, token);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to obtaining the MAC address for node {ipAddress}", ipAddress);
                return Task.FromResult(MacAddress.Empty);
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
}