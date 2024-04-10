using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Kangaroo;

/// <summary>
/// Describes a scanned network endpoint.
/// </summary>
/// <param name="IpAddress">The endpoints IP address</param>
/// <param name="MacAddress">The endpoints queried MAC Address</param>
/// <param name="HostName">The endpoints dns or hostname</param>
/// <param name="WebServer">The endpoints queried web server</param>
/// <param name="Latency">The endpoints queried latency</param>
/// <param name="QueryTime">The elapsed time the entire endpoint scan took</param>
/// <param name="Alive">True if the endpoint is determined to be UP</param>
public record NetworkNode(
    IPAddress IpAddress, 
    MacAddress MacAddress, 
    string? HostName, 
    string? WebServer,
    TimeSpan? Latency, 
    TimeSpan QueryTime, 
    bool Alive)
{

    internal static NetworkNode BadNode(IPAddress ipAddress, TimeSpan elapsedTime) => 
        new(ipAddress, MacAddress.Empty, null, null, null, elapsedTime, false);
    
    internal static NetworkNode InProgress(IPAddress ipAddress) => 
        new(ipAddress, MacAddress.Empty, null, null, TimeSpan.Zero, TimeSpan.Zero, false);

    /// <inheritdoc />
    public override string ToString()
    {
        return new StringBuilder()
            .Append("IP: ").Append(IpAddress).Append(" | ")
            .Append("ALIVE: ").Append(Alive).Append(" | ")
            .Append("ELAPSED: ").Append(QueryTime).Append(" | ")
            .Append("MAC ADDRESS: ").Append(MacAddress).Append(" | ")
            .Append("LATENCY: ").Append(Latency).Append(" | ")
            .ToString();
    }
}