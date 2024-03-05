using System.Net.NetworkInformation;

namespace Kangaroo;

public sealed class InvalidNetworkAdapterException : Exception
{
    public InvalidNetworkAdapterException(NetworkInterface @interface)
        : base($"Invalid network interface {@interface.Name}") { }
}