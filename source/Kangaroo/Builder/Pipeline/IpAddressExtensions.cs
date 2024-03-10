using System.Net;

namespace Kangaroo;

internal static class IpAddressExtensions
{

    public static void ThrowIfAddressIsNotEndpoint(this IPAddress address)
    {
        if (Equals(address, IPAddress.Any))
        {
            throw new InvalidIpAddressException(address);
        }

        if (Equals(address, IPAddress.Loopback))
        {
            throw new InvalidIpAddressException(address);
            
        }

        if (Equals(address, IPAddress.Broadcast))
        {
            throw new InvalidIpAddressException(address);
        }

        if (address.GetAddressBytes().Any(b => b == 255))
        {
            throw new InvalidIpAddressException(address);
        }
    }
}