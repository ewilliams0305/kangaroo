using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Kangaroo.UI.Database;

public static class RecentScanResultMapping
{
    public static ScanResults ToResults(this RecentScanResult results) =>
        new(
            results.Nodes.ToNetworkNodes(), 
            results.ElapsedTime, 
            results.NumberOfAddressesScanned,
            results.NumberOfAliveNodes,
            IPAddress.TryParse(results.StartAddress, out var startAddress) 
                ? startAddress
                : IPAddress.None,
            IPAddress.TryParse(results.EndAddress, out var endAddress) 
                ? endAddress
                : IPAddress.None);


    public static RecentScanResult ToResults(this ScanResults results) =>
        new(
            results.Nodes.ToResults(), 
            results.ElapsedTime, 
            results.NumberOfAddressesScanned,
            results.NumberOfAliveNodes,
            results.StartAddress.ToString(),
            results.EndAddress.ToString());

    public static NetworkNode ToNetworkNode(this RecentScanNodeResult result) =>
        new(
            IPAddress.TryParse(result.IpAddress, out var ipAddress) 
                ? ipAddress 
                : IPAddress.None, 
            new MacAddress(result.MacAddress), 
            result.HostName, 
            result.WebServer, 
            result.Latency,
            result.QueryTime,
            result.Alive);

    public static IEnumerable<NetworkNode> ToNetworkNodes(this IEnumerable<RecentScanNodeResult> results) =>
        results.Select(n => ToNetworkNode(n));
    
    public static RecentScanNodeResult ToResult(this NetworkNode node) =>
        new(
            node.IpAddress.ToString(), 
            node.MacAddress.ToString(), 
            node.HostName, 
            node.WebServer, 
            node.Latency,
            node.QueryTime,
            node.Alive);

    public static IEnumerable<RecentScanNodeResult> ToResults(this IEnumerable<NetworkNode> nodes) =>
        nodes.Select(n => n.ToResult());
}