using System.Net;

namespace Kangaroo;

internal static class IpAddressExtensions
{

    public static byte[] ThrowIfAddressIsNotEndpoint(this IPAddress address)
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

        var bytes = address.GetAddressBytes();
        if (bytes.Any(b => b == 255))
        {
            throw new InvalidIpAddressException(address);
        }

        return bytes;
    }
    
    public static byte[] ThrowIfAddressLessThen16(this IPAddress address)
    {
        var bytes = address.GetAddressBytes();
        if (bytes[0] != 255)
        {
            throw new InvalidIpAddressException(address);
        }

        if (bytes[1] != 255)
        {
            throw new InvalidIpAddressException(address);
        }

        return bytes;
    }
}