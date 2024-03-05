using System.Net;
using System.Text;

namespace Kangaroo;

/// <summary>
/// Represents a device or node on a network.
/// </summary>
/// <param name="IpAddress"></param>
/// <param name="MacAddress"></param>
/// <param name="HostName"></param>
/// <param name="Latency">The latency report during ping responses.</param>
/// <param name="QueryTime">The elapsed time the entire query took.</param>
/// <param name="Alive">True when the node is alive.</param>
public record NetworkNode(
    IPAddress IpAddress, 
    MacAddress MacAddress, 
    string? HostName, 
    string? WebServer,
    TimeSpan? Latency, 
    TimeSpan QueryTime, 
    bool Alive)
{
    internal static NetworkNode BadNode(IPAddress ipAddress, TimeSpan elapsedTime) => new(ipAddress, MacAddress.Empty, null, null, null, elapsedTime, false);

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