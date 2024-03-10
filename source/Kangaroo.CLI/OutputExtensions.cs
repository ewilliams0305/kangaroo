using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Dumpify;
using Kangaroo;

namespace Kangaroo.CLI;

public static class OutputExtensions
{
    public static void DumpNode(this IEnumerable<NetworkNode> nodes)
    {
        var output = nodes
            .Where(n => n.Alive)
            .Select(n => new
            {
                IPAddress = n.IpAddress.ToString(),
                MacAddress = n.MacAddress.ToString(),
                Hostname = n.HostName,
                WebServer = n.WebServer,
                Latency = n.Latency.ToString(),
                QueryTime = n.QueryTime.ToString(),
            })
            .ToList();

        output.Dump("NETWORK NODES LOCATED", typeNames: new TypeNamingConfig { ShowTypeNames = false });
    }

    public static void DumpResults(this ScanResults results)
    {
        var output = results.Nodes
            .Where(n => n.Alive)
            .Select(n => new
            {
                IPAddress = n.IpAddress.ToString(),
                MacAddress = n.MacAddress.ToString(),
                Hostname = n.HostName,
                WebServer = n.WebServer,
                Latency = n.Latency.ToString(),
                QueryTime = n.QueryTime.ToString(),
            })
            .ToList();

        output.Dump("NETWORK NODES LOCATED", typeNames: new TypeNamingConfig { ShowTypeNames = false });
        var global = new
        {
            Scanned = $"{results.NumberOfAliveNodes} UP / {results.NumberOfAddressesScanned}",
            ElapsedTime = results.ElapsedTime.ToString(),
            StartAddress = results.StartAddress.ToString(),
            EndAddress = results.EndAddress.ToString()

        };

        global.Dump("SCANNER RESULTS", typeNames: new TypeNamingConfig { ShowTypeNames = false });
    }

    public static void DumpInterface(this NetworkInterface adapter, UnicastIPAddressInformation ip)
    {
        new
        {
            Adapter = adapter.Name,
            adapter.Description,
            Address = ip.Address.ToString(),
            NetMask = ip.IPv4Mask.ToString()
        }.Dump("ADAPTERS", typeNames: new TypeNamingConfig { ShowTypeNames = false });
    }
}