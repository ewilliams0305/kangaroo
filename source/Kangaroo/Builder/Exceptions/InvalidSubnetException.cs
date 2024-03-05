using System.Net;

namespace Kangaroo;

public sealed class InvalidSubnetException : Exception
{
    public InvalidSubnetException(IPAddress address, IPAddress subnet)
        : base($"Invalid IP address subnet {address}/{subnet}") { }
}