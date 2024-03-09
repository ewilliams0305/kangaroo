using System.Net;

namespace Kangaroo;

/// <summary>
/// Exception thrown when a subnet is calculated invalid
/// </summary>
public sealed class InvalidSubnetException : Exception
{
    /// <summary>
    /// Creates a new subnet exception
    /// </summary>
    /// <param name="address">IP address</param>
    /// <param name="subnet">Subnet mask</param>
    public InvalidSubnetException(IPAddress address, IPAddress subnet)
        : base($"Invalid IP address subnet {address}/{subnet}") { }
}