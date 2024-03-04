using Cocona;
using Dumpify;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Kangaroo.CLI.Commands
{
    public sealed class IpScanCommand
    {
        private readonly ILogger _logger;
        private readonly IScannerIpConfiguration _config;
        private readonly IEnumerable<IPAddress> _addresses;

        public IpScanCommand(ILogger logger, IScannerIpConfiguration config, IEnumerable<IPAddress> addresses)
        {
            _logger = logger;
            _config = config;
            _addresses = addresses;
        }

        [Command("scan", Description = "Scans the configured range of IP addresses")]
        public async Task ScanNetwork()
        {
            var scanner = _config
                .WithSubnet("10.0.0.0", "255.255.255.0")
                .WithMaxTimeout(TimeSpan.FromMilliseconds(250))
                .WithMaxHops(4)
                .WithParallelism(10)
                .WithLogging(_logger)
                .Build();

            var results = await scanner.QueryAddresses();
            var output = results.Nodes
                .Where(n => n.Alive)
                .Select(n => new
                {
                    IPAddress = n.IpAddress.ToString(),
                    MacAddress = n.MacAddress.ToString(),
                    Hostname = n.HostName,
                    Latency = n.Latency.ToString(),
                    QueryTime = n.QueryTime.ToString(),
                })
                .ToList();

            new
            {
                Scanned = $"{results.NumberOfAliveNodes} UP / {results.NumberOfAddressesScanned}" ,
                ElapsedTime = results.ElapsedTime.ToString(),
                StartAddress = results.StartAddress.ToString(), 
                EndAddress = results.EndAddress.ToString()

            }.Dump("SCANNER RESULTS", typeNames: new TypeNamingConfig { ShowTypeNames = false });

            output.Dump("NETWORK NODES LOCATED", typeNames: new TypeNamingConfig { ShowTypeNames = false });
        }
    }
}
