using Cocona;
using Dumpify;
using Microsoft.Extensions.Logging;

namespace Kangaroo.CLI.Commands;

public sealed class RangeScanCommand(ILogger logger, IScannerIpConfiguration config)
{
    [Command("range", Description = "Scans the configured range of IP addresses")]
    public async Task<int> ScanNetwork(
        [Option(
            shortName: 's',
            Description = "Starting IP address to begin scan with",
            ValueName = "start")] string start, 
        [Option(
            shortName: 'e',
            Description = "Ending IP address to stop scan at",
            ValueName = "end")] string end,
        [Option(
            shortName: 't',
            Description = "Query timeout in milliseconds",
            ValueName = "timeout")] int? timeout)
    {
        try
        {
            var scanner = config

                .WithRange(start, end)
                .WithHttpScan(() => new HttpClient())
                .WithMaxTimeout(TimeSpan.FromMilliseconds(timeout ?? 1000))
                .WithMaxHops(4)
                .WithParallelism(10)
                .WithLogging(logger)
                .Build();

            var results = await scanner.QueryNetwork();
            results.DumpResults();
            return 0;
        }
        catch (InvalidIpRangeException e)
        {
            e.Message.Dump();
        }

        return -1;

    }
}