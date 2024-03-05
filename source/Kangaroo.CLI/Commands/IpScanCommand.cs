using Cocona;
using Dumpify;
using Microsoft.Extensions.Logging;

namespace Kangaroo.CLI.Commands;

public sealed class IpScanCommand(ILogger logger, IScannerIpConfiguration config)
{
    [Command("scan", Description = "Scans the configured range of IP addresses")]
    public async Task<int> ScanNetwork([Option(shortName: 's')] string start, [Option(shortName: 'e')] string end, [Option(shortName: 't')] int? timeout)
    {
        var scanner = config

            .WithRange(start, end)
            .WithMaxTimeout(TimeSpan.FromMilliseconds(timeout ?? 1000))
            .WithMaxHops(4)
            .WithParallelism(10)
            .WithLogging(logger)
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

        return 0;
    }
}