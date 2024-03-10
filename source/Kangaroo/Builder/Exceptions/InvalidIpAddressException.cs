using System.Net;

namespace Kangaroo;

/// <summary>
/// Exception thrown when an IP address is not a valid endpoint
/// <remarks>Broadcast addresses are invalid</remarks>
/// </summary>
public sealed class InvalidIpAddressException : Exception
{
    /// <summary>
    /// Creates a new ip address exception
    /// </summary>
    /// <param name="address">IP address</param>
    public InvalidIpAddressException(IPAddress address)
        : base($"The IP Address provided is not a network endpoint {address}") { }
}