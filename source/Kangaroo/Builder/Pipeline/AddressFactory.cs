using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Kangaroo;

internal sealed class AddressFactory
{

    internal static IEnumerable<IPAddress> CreateAddressesFromRange(IPAddress begin, IPAddress end)
    {
        begin.ThrowIfAddressIsNotEndpoint();
        end.ThrowIfAddressIsNotEndpoint();

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
        ipAddress.ThrowIfAddressIsNotEndpoint();
        var maskBytes = subnetMask.ThrowIfAddressLessThen16();

        var ipList = new List<IPAddress>();
        var ipBytes = ipAddress.GetAddressBytes();

        var networkBytes = new byte[ipBytes.Length];
        for (var i = 0; i < ipBytes.Length; i++)
        {
            networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
        }

        //networkBytes[3] += 1;

        var bitsToInvert = 32 - GetBitCount(BitConverter.ToUInt32(maskBytes, 0));
        var numHosts = (int)Math.Pow(2, bitsToInvert) - 2;

        if (numHosts <= 254)
        {
            for (var i = 1; i <= numHosts; i++)
            {
                var j = networkBytes[3] + i;
                var nextIpBytes = new byte[] { networkBytes[0], networkBytes[1], networkBytes[2], (byte)j };
                ipList.Add(new IPAddress(nextIpBytes));
            }
            
            return ipList;
        }

        networkBytes[3]++;

        var end = GetLastAvailableAddress(networkBytes, maskBytes);

        ipList.AddRange(CreateAddresses16(networkBytes, end));

        var firstPass = true;
        for (var i = 1; i <= numHosts; i++)
        {

            if (firstPass)
            {
                for (var j = networkBytes[3]; j <= 254; j++)
                {
                    var nextIpBytes = new byte[] { networkBytes[0], networkBytes[1], networkBytes[2], (byte)j };
                    ipList.Add(new IPAddress(nextIpBytes));
                }

                firstPass = false;
                continue;
            }

            for (byte j = 0x01; j <= 254; j++)
            {
                var nextIpBytes = new byte[] { networkBytes[0], networkBytes[1], networkBytes[2], j };
                ipList.Add(new IPAddress(nextIpBytes));
            }
            continue;

        }
        
        return ipList;
        





        throw new NotSupportedException($"Number or host calculated exceeds the current subnet scan of 254 addresses");
    }

    internal static byte[] GetLastAvailableAddress(IReadOnlyList<byte> networkAddressBytes, IReadOnlyList<byte> subnetMaskBytes)
    {
        // Calculate broadcast address by performing bitwise OR operation between inverted subnet mask and network address
        byte[] invertedSubnetMaskBytes = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            invertedSubnetMaskBytes[i] = (byte)~subnetMaskBytes[i];
        }

        byte[] broadcastAddressBytes = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            broadcastAddressBytes[i] = (byte)(networkAddressBytes[i] | invertedSubnetMaskBytes[i]);
        }

        // Subtract 1 from the last octet of the broadcast address to find the last valid IP address
        broadcastAddressBytes[3] -= 1;

        return broadcastAddressBytes;
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
        var first = true; 
        
        for (var i = startBytes[2]; i <= endBytes[2]; i++)
        {
            if (i < endBytes[2])
            {
                if (first)
                {
                    for (var j = startBytes[3]; j <= 254; j++)
                    {
                        ips.Add(new IPAddress(new byte[] { startBytes[0], startBytes[1], i, j }));
                    }

                    first = false;
                    continue;
                }
                
                for (byte j = 0x01; j <= 254; j++)
                {
                    ips.Add(new IPAddress(new byte[] { startBytes[0], startBytes[1], i, j }));
                }
                continue;
            }
            for (byte j = 0x01; j <= endBytes[3]; j++)
            {
                ips.Add(new IPAddress(new byte[] { startBytes[0], startBytes[1], i, j }));
            }
        }

        return ips;
    }

    private static IEnumerable<IPAddress> CreateAddresses24(byte[] startBytes, byte[] endBytes)
    {
        if (startBytes[3] >= endBytes[3])
        {
            throw new InvalidIpRangeException(startBytes, endBytes);
        }

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


