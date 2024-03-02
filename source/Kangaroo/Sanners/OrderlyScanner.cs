using Kangaroo.Platforms;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Kangaroo
{
    internal sealed class OrderlyScanner : IScanner
    {
        /// <summary>
        /// Starts the scanner configuration process.
        /// </summary>
        /// <returns>Returns a new scanner builder and begins the build pipeline</returns>
        public static IScannerIpConfiguration Configure()
        {
            return new ScannerBuilder();
        }

        /// <summary>
        /// Factory used to create a new instance of the scanner.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="addresses"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        internal static OrderlyScanner CreateScanner(
            ILogger logger,
            IEnumerable<IPAddress> addresses,
            int timeout)
        {
            return new OrderlyScanner(logger, addresses, timeout);
        }

        private readonly ILogger _logger;
        private readonly IEnumerable<IPAddress> _addresses;
        private readonly Ping _ping = new();
        private readonly Stopwatch _stopWatch = new();
        private readonly int _timeout;

        private OrderlyScanner(ILogger logger, IEnumerable<IPAddress> addresses, int timeout)
        {
            _logger = logger;
            _addresses = addresses;
            _timeout = timeout;
        }

        public async Task<ScanResults> QueryAddresses(CancellationToken token = default)
        {
            _stopWatch.Restart();

            var nodes = new List<NetworkNode>();

            await foreach (var node in NetworkQueryAsync(token))
            {
                nodes.Add(node);
            }

            _stopWatch.Stop();
            return new ScanResults(nodes, _stopWatch.Elapsed, 0, IPAddress.Any, IPAddress.Any);
        }

        public async IAsyncEnumerable<NetworkNode> NetworkQueryAsync([EnumeratorCancellation] CancellationToken token = default)
        {
            foreach (var ip in _addresses)
            {
                yield return await CheckNetworkNode(ip, token);
            }
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
                    mac ?? "00:00:00:00:00",
                    host != null ? host.HostName : "N/A",
                    TimeSpan.FromMilliseconds(reply.RoundtripTime),
                    stopwatch.Elapsed, 
                    true);

                _logger.LogInformation("{node}", node);
                return node;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Failed testing node {ipAddress}", ipAddress);
                return new NetworkNode(ipAddress, null, null, null, stopwatch.Elapsed, false);
            }
        }

        private async Task<PingReply?> PingNode(IPAddress ipAddress, CancellationToken token = default)
        {
            try
            {
                var options = new PingOptions(ttl: 5, false);
                var result = await _ping.SendPingAsync(ipAddress, _timeout, new byte[32], options);
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

        public async Task<string?> GetMacAddressAsync(IPAddress ipAddress, CancellationToken token)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var macAddr = new byte[6];
                    var macAddrLen = macAddr.Length;
                    var macAddrLenUlong = (uint)macAddrLen;

                    if (WindowsArp.SendARP(BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0), 0, macAddr, ref macAddrLenUlong) != 0)
                    {
                        _logger.LogDebug("Failed obtaining the MAC address for {ipAddress}", ipAddress);
                        return null;
                    }

                    var macAddress = new StringBuilder();
                    for (var i = 0; i < macAddrLen; i++)
                    {
                        macAddress.Append(macAddr[i].ToString("X2"));
                        if (i != macAddrLen - 1)
                            macAddress.Append(':');
                    }

                    return macAddress.ToString();
                }, token);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to obtaining the MAC address for node {ipAddress}", ipAddress);
                return null;
            }
        }

        #region IDisposable

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _ping?.Dispose();
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