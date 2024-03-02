using System.Diagnostics;
using System.Net;
using System.Text;

namespace Kangaroo;

public record NetworkNode(
    IPAddress IpAddress, 
    string? MacAddress, 
    string? HostName, 
    TimeSpan? Latency, 
    TimeSpan QueryTime, 
    bool Alive)
{
    internal static NetworkNode BadNode(IPAddress ipAddress, TimeSpan elapsedTime) => new(ipAddress, null, null, null, elapsedTime, false);

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