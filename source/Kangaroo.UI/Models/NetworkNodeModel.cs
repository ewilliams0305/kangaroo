namespace Kangaroo.UI.Models;

public class NetworkNodeModel
{
    public string IpAddress { get; set; }
    public string MacAddress { get; set; }
    public string Latency { get; set; }
    public string QueryTime { get; set; }
    public bool IsAlive { get; set; }
    public string WebServer { get; set; }
    public string  DnsName { get; set; }

    public NetworkNodeModel(NetworkNode node)
    {
        IpAddress = node.IpAddress.ToString();
        MacAddress = node.MacAddress.ToString();
        IsAlive = node.Alive;

        WebServer = node.WebServer ?? "";
        DnsName = node.HostName ?? "";
        Latency = node.Latency.ToString() ?? "";
        QueryTime = node.QueryTime.ToString();
    }
}