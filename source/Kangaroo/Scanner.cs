using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kangaroo
{
    public sealed class Scanner : IScanner
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
        /// <param name="addresses"></param>
        /// <param name="concurrent"></param>
        /// <param name="timeout"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        internal static Scanner CreateScanner(
            IEnumerable<IPAddress> addresses, 
            bool concurrent, 
            int timeout, 
            Action<Exception> exception, 
            Action<string> message)
        {
            return new Scanner(addresses, concurrent, timeout, exception, message);
        }


        private readonly IEnumerable<IPAddress> _addresses;
        private readonly Ping _ping;

        private readonly byte[] _buffer = new byte[32];
        private readonly bool _concurrent;
        private readonly int _timeout;
        
        private readonly Action<Exception> _exception;
        private readonly Action<string> _message;
        
   
        private Scanner(IEnumerable<IPAddress> addresses, bool concurrent, int timeout, Action<Exception> exception, Action<string> message)
        {
            _addresses = addresses;
            _concurrent = concurrent;
            _timeout = timeout;
            _exception = exception;
            _message = message;

            if (!_concurrent)
            {
                _ping = new Ping();
            }
        }

        public async Task<IEnumerable<NetworkNode>> QueryAddresses(CancellationToken token = default)
        {
            if (_concurrent)
            {
                return await NetworkQueryConcurrent(token);
            }
            
            var nodes = new List<NetworkNode>();
            
            await foreach (var node in NetworkQueryAsync(token))
            {
                nodes.Add(node);
            }

            return nodes;
        }

        public async IAsyncEnumerable<NetworkNode> NetworkQueryAsync([EnumeratorCancellation] CancellationToken token = default)
        {
            foreach (var ip in _addresses)
            {
                yield return await CheckNetworkNode(ip, token);
            }
        }

        public async Task<IEnumerable<NetworkNode>> NetworkQueryConcurrent(CancellationToken token = default)
        {
            try
            {
                var tasks = _addresses
                    .Select(ip => CheckNetworkNode(ip, token))
                    .ToList();

                return await Task.WhenAll(tasks);
            }
            catch (ArgumentNullException nullException)
            {
                _exception?.Invoke(nullException);
                return Array.Empty<NetworkNode>();
            }
            catch (ArgumentException argException)
            {
                _exception?.Invoke(argException);
                return Array.Empty<NetworkNode>();
            }
        }


        Stopwatch stopWatch = new Stopwatch();
        
        public async Task<NetworkNode> CheckNetworkNode(IPAddress ipAddress, CancellationToken token = default)
        {
            try
            {
                stopWatch.Start();
                var reply = _concurrent
                    ? await ConcurrentPing(ipAddress, token)
                    : await RecycledPing(ipAddress, token);

                if (reply is not { Status: IPStatus.Success})
                {
                    var badNode = new NetworkNode(ipAddress)
                    {
                        QueryTime = stopWatch.Elapsed
                    }; 
                    _message?.Invoke(badNode.ToString());
                    return badNode;
                }

                var mac = await GetMacAddressAsync(ipAddress, token);
                var host = await GetHostname(ipAddress, token);
                

                var node =  new NetworkNode(ipAddress)
                {
                    QueryTime = stopWatch.Elapsed,
                    IsConnected = true,
                    Latency = TimeSpan.FromMilliseconds(reply.RoundtripTime),
                    Mac = mac ?? "00:00:00:00:00",
                    Hostname = host != null ? host.HostName : "N/A",
                };

                _message?.Invoke(node.ToString());
                return node;
            }
            catch (Exception e)
            {
                _exception?.Invoke(e);
                return new NetworkNode(ipAddress);
            }
        }


        public async Task<PingReply?> ConcurrentPing(IPAddress ipAddress, CancellationToken token = default)
        {

            try
            {
                var options = new PingOptions(ttl: 5, false)
                {

                };

                using var ping = new Ping();
                var result = await ping.SendPingAsync(ipAddress, _timeout, _buffer, options);
                return result;
            }
            catch (Exception e)
            {
                _exception.Invoke(e);
                return null;
            }
        }

        

        private async Task<PingReply?> RecycledPing(IPAddress ipAddress, CancellationToken token = default)
        {
            try
            {
                PingOptions options = new PingOptions(ttl: 5, false);
                
                var result = await _ping.SendPingAsync(ipAddress, _timeout, _buffer, options);
                return result;
            }
            catch (Exception e)
            {
                _exception?.Invoke(e);
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
                _exception?.Invoke(argumentException);
            }
            catch (SocketException socketError)
            {
                _exception?.Invoke(socketError);
            }
            catch (Exception e)
            {
                _exception?.Invoke(e);
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
                        // Failed to obtain MAC address
                        return null;
                    }

                    var macAddress = new StringBuilder();
                    for (int i = 0; i < macAddrLen; i++)
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
                // Handle exceptions
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        private string? GetMacAddress(IPAddress ipAddress, CancellationToken token = default)
        {
            try
            {
                var macAddr = new byte[6];
                var macAddrLen = macAddr.Length;
                var macAddrLenUlong = (uint)macAddrLen;

                if (WindowsArp.SendARP(BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0), 0, macAddr,
                        ref macAddrLenUlong) != 0)
                {
                    _message?.Invoke($"Failed to obtain the MAC address for {ipAddress}");
                    return null;
                }

                var macAddress = new StringBuilder();
                for (int i = 0; i < macAddrLen; i++)
                {
                    macAddress.Append(macAddr[i].ToString("X2"));
                    if (i != macAddrLen - 1)
                        macAddress.Append(':');
                }

                return macAddress.ToString();
            }
            catch (Exception ex)
            {
                _exception?.Invoke(ex);
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