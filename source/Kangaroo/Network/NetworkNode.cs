using System.Net;
using System.Text;

namespace Kangaroo;

public sealed class NetworkNode
{
    public IPAddress Address { get; internal set; }
    public string? Mac { get; internal set; }
    public string? Hostname { get; internal set; }
    public TimeSpan? Latency { get; internal set; }

    public bool IsConnected { get; internal set; } = false;

    public NetworkNode(IPAddress address)
    {
        Address = address;
    }


    /// <inheritdoc />
    public override string ToString()
    {
        return new StringBuilder()
            .Append("NODE: ").Append(Address).Append(" | ")
            .Append("MAC: ").Append(Mac).Append(" | ")
            .Append("CONNECTED: ").Append(IsConnected).Append(" | ")
            .Append("LATENCY: ").Append(Latency).Append(" | ")
            .ToString();
    }

}