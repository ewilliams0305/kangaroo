using System.Net;
using System.Net.NetworkInformation;

namespace Kangaroo;

internal sealed class AddressFactory
{

    internal static IEnumerable<IPAddress> CreateAddressesFromRange(IPAddress begin, IPAddress end)
    {

        var startBytes = begin.GetAddressBytes();
        var endBytes = end.GetAddressBytes();

        if (startBytes[0] != endBytes[0] || startBytes[1] != endBytes[1])
        {
            throw new InvalidIpRangeException(begin, end);
        }

        if (startBytes[2] == endBytes[2])
        {
            return CreateAddresses24(startBytes, endBytes);
        }

        if (startBytes[2] > endBytes[2])
        {
            throw new InvalidIpRangeException(begin, end);
        }

        return CreateAddresses16(startBytes, endBytes);
    }

    internal static IEnumerable<IPAddress> CreateAddressesFromSubnet(IPAddress ipAddress, IPAddress subnetMask)
    {
        if (Equals(ipAddress, IPAddress.Any))
        {
            throw new InvalidSubnetException(ipAddress, subnetMask);
        }

        if (Equals(ipAddress, IPAddress.Broadcast))
        {
            throw new InvalidSubnetException(ipAddress, subnetMask);
        }

        if (Equals(ipAddress, IPAddress.Loopback))
        {
            throw new InvalidSubnetException(ipAddress, subnetMask);
        }

        var maskBytes = subnetMask.GetAddressBytes();

        if (maskBytes[0] != 255)
        {
            throw new InvalidSubnetException(ipAddress, subnetMask);
        }
        
        if (maskBytes[1] != 255)
        {
            throw new InvalidSubnetException(ipAddress, subnetMask);
        }

        var ipList = new List<IPAddress>();
        var ipBytes = ipAddress.GetAddressBytes();

        if (ipBytes.Length != maskBytes.Length)
        {
            throw new ArgumentException("IP address and subnet mask lengths do not match.");
        }

        var networkBytes = new byte[ipBytes.Length];
        for (var i = 0; i < ipBytes.Length; i++)
        {
            networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
        }

        ipList.Add(new IPAddress(networkBytes));

        var bitsToInvert = 32 - GetBitCount(BitConverter.ToUInt32(maskBytes, 0));
        var numHosts = (int)Math.Pow(2, bitsToInvert) - 2;

        for (var i = 1; i <= numHosts; i++)
        {
            var nextIpBytes = new byte[ipBytes.Length];
            nextIpBytes[0] = ipBytes[0];
            for (var j = 1; j < ipBytes.Length; j++)
            {
                nextIpBytes[j] = (byte)((networkBytes[j] & 0xFF) + i >> (24 - 8 * j));
            }
            ipList.Add(new IPAddress(nextIpBytes));
        }

        return ipList;
    }

    internal static IEnumerable<IPAddress> CreateAddressesFromInterface(NetworkInterface @interface)
    {
        var ip = GetIpFromInterface(@interface);
        return ip == null
            ? Array.Empty<IPAddress>()
            : CreateAddressesFromSubnet(ip.Address, ip.IPv4Mask);
    }

    internal static IEnumerable<IPAddress> CreateAddressesFromInterfaces()
    {
        var ipList = new List<IPAddress>();
        var interfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (var @interface in interfaces)
        {
            if (@interface.OperationalStatus != OperationalStatus.Up &&
                @interface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                @interface.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            {
                continue;
            }

            var ipProps = @interface.GetIPProperties();
            foreach (var ip in ipProps.UnicastAddresses)
            {
                if (ip.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    continue;
                }
                ipList.Add(ip.Address);
            }
        }
        return ipList;
    }

    private static IEnumerable<IPAddress> CreateAddresses16(IReadOnlyList<byte> startBytes, IReadOnlyList<byte> endBytes)
    {
        var ips = new List<IPAddress>();

        for (var i = startBytes[2]; i <= endBytes[2]; i++)
        {
            if (i < endBytes[2])
            {
                for (var j = startBytes[3]; j <= 254; j++)
                {
                    ips.Add(new IPAddress(new byte[] { startBytes[0], startBytes[2], i, j }));
                }
                continue;
            }

            for (var j = startBytes[3]; j <= endBytes[3]; j++)
            {
                ips.Add(new IPAddress(new byte[] { startBytes[0], startBytes[2], i, j }));
            }
        }

        return ips;
    }

    private static IEnumerable<IPAddress> CreateAddresses24(byte[] startBytes, byte[] endBytes)
    {
        var ips = new List<IPAddress>();

        for (var i = startBytes[3]; i <= endBytes[3]; i++)
        {
            ips.Add(new IPAddress(new byte[] { startBytes[0], startBytes[1], startBytes[2], (byte)i }));
        }

        return ips;
    }

    internal static NetworkInterface? GetFirstInterface()
    {
        var interfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (var @interface in interfaces)
        {
            if (@interface.OperationalStatus != OperationalStatus.Up && 
                @interface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                @interface.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            {
                continue;
            }

            var ipProps = @interface.GetIPProperties();
            foreach (var ip in ipProps.UnicastAddresses)
            {
                if (ip.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    continue;
                }
                return @interface;
            }
        }
        return null;
    }

    internal static UnicastIPAddressInformation? GetIpFromInterface(NetworkInterface @interface)
    {
        if (@interface.OperationalStatus != OperationalStatus.Up)
        {
            throw new InvalidNetworkAdapterException(@interface);
        }

        var ipProps = @interface.GetIPProperties();
        foreach (var ip in ipProps.UnicastAddresses)
        {
            if (ip.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                continue;
            }
            return ip;
        }

        throw new InvalidNetworkAdapterException(@interface);
    }

    internal static int GetBitCount(uint value)
    {
        var count = 0;
        while (value != 0)
        {
            count++;
            value &= value - 1;
        }
        return count;
    }
}


