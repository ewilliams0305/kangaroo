using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cocona;
using Dumpify;
using Dumpify.Descriptors;
using Microsoft.Extensions.Logging;

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
        public async Task Scan()
        {
            var scanner = _config
                .WithAddresses(_addresses)
                .WithParallelism()
                .WithNodeTimeout(TimeSpan.FromMilliseconds(250))
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
                Nodes = results.NumberOfAddressesScanned,
                ElapsedTime = results.ElapsedTime.ToString(),
                StartAddress = results.StartAddress.ToString(), 
                EndAddress = results.EndAddress.ToString()

            }.Dump("SCANNER RESULTS", typeNames: new TypeNamingConfig { ShowTypeNames = false });
            output.Dump("NETWORK NODES LOCATED", typeNames: new TypeNamingConfig { ShowTypeNames = false });
        }
    }
}
