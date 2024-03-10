using Cocona;
using Dumpify;
using Microsoft.Extensions.Logging;

namespace Kangaroo.CLI.Commands;

public sealed class SubnetScanCommand(ILogger logger, IScannerIpConfiguration config)
{
    [Command("subnet", Description = "Scans the configured subnet, note only /16 - /24 is currently available")]
    public async Task<int> ScanNetwork(
        [Option(
            shortName: 'i',
            Description = "IP Address matching the subnet mask provided",
            ValueName = "i")] string ip, 
        [Option(
            shortName: 'm',
            Description = "Subnet mask to determine IP range scope",
            ValueName = "end")] string mask,
        [Option(
            shortName: 't',
            Description = "Query timeout in milliseconds",
            ValueName = "timeout")] int? timeout)
    {

        try
        {
            var scanner = config

                .WithSubnet(ip, mask)
                .WithHttpScan()
                .WithMaxTimeout(TimeSpan.FromMilliseconds(timeout ?? 1000))
                .WithMaxHops(4)
                .WithParallelism(10)
                .WithLogging(logger)
                .Build();

            var results = await scanner.QueryNetwork();
            results.DumpResults();
            return 0;
        }
        catch (InvalidSubnetException e)
        {
            e.Message.Dump();
        }

        return -1;

    }
}