using Cocona;
using Dumpify;
using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;

namespace Kangaroo.CLI.Commands;

public sealed class AdapterScanCommand()
{

    [Command("adapter", Aliases = ["a"],Description = "Scans IP address using the subnet on the provided adapter")]
    public static async ValueTask<int> QueryAdapter(
        [Option(
            shortName: 'a', 
            Description = "The network adapter to scan",
            ValueName = "adapter")] string? adapter , 
        [Option(
            shortName: 't',
            Description = "Query timeout in milliseconds",
            ValueName = "timeout")] int? timeout)=>
        adapter == null 
            ? QueryAdapters() 
            : await ScanAdapter(adapter, timeout);

    private static int QueryAdapters()
    {
        var interfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (var @interface in interfaces)
        {
            if (@interface.OperationalStatus != OperationalStatus.Up &&
                @interface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                @interface.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            {
                continue;
            }

            var ipProps = @interface.GetIPProperties();
            foreach (var ip in ipProps.UnicastAddresses)
            {
                if (ip.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    continue;
                }

                @interface.DumpInterface(ip);
            }
        }
        return 0;
    }

    public static async Task<int> ScanAdapter(string adapterName, int? timeout)
    {
        try
        {
            var adapter = NetworkInterface
                .GetAllNetworkInterfaces()
                .FirstOrDefault(n =>
                    n.Name.ToLower().Replace(" ", "") ==
                    adapterName.ToLower().Replace(" ", ""));

            if (adapter == null)
            {
                Console.WriteLine($"Invalid Network Adapter Specified {adapterName}");
                return -1;
            }
            var scanner = ScannerBuilder
                .Configure()
                .WithInterface(adapter)
                .WithHttpScan(() =>
                {
                    return new HttpClient(new HttpClientHandler()
                    {
                        ClientCertificateOptions = ClientCertificateOption.Manual,
                        ServerCertificateCustomValidationCallback =
                            (httpRequestMessage, cert, cetChain, policyErrors) => true
                    });
                })
                .WithMaxTimeout(TimeSpan.FromMilliseconds(timeout ?? 1000))
                .WithMaxHops(4)
                .WithParallelism(10)
                .WithLogging(LoggerFactory.Create(ops =>
                {
                    ops.AddConsole();
                }))
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