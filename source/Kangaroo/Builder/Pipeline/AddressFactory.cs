using System.Dynamic;
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
        var ipBytes = ipAddress.ThrowIfAddressIsNotEndpoint();
        var maskBytes = subnetMask.ThrowIfAddressLessThen16();

        var (networkBytes, numHosts) = DetermineSubnetAddress(ipBytes, maskBytes);

        var end = DetermineLastAvailableAddress(networkBytes, maskBytes);
        networkBytes[3]++;
        
        return numHosts <= 254 
            ? CreateAddresses24(networkBytes, end) 
            : CreateAddresses16(networkBytes, end);
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

    internal static (byte[] networkAddress, int numberOfAddresses) DetermineSubnetAddress(IReadOnlyList<byte> ipAddress, IReadOnlyList<byte> subnetMask)
    {
        var networkBytes = new byte[ipAddress.Count];
        for (var i = 0; i < ipAddress.Count; i++)
        {
            networkBytes[i] = (byte)(ipAddress[i] & subnetMask[i]);
        }

        var bitsToInvert = 32 - GetBitCount(BitConverter.ToUInt32((byte[])subnetMask, 0));
        var numHosts = (int)Math.Pow(2, bitsToInvert) - 2;

        return (networkBytes, numHosts);
    }

    internal static byte[] DetermineLastAvailableAddress(IReadOnlyList<byte> networkAddressBytes, IReadOnlyList<byte> subnetMaskBytes)
    {
        var invertedSubnetMaskBytes = new byte[4];
        for (var i = 0; i < 4; i++)
        {
            invertedSubnetMaskBytes[i] = (byte)~subnetMaskBytes[i];
        }

        var broadcastAddressBytes = new byte[4];
        for (var i = 0; i < 4; i++)
        {
            broadcastAddressBytes[i] = (byte)(networkAddressBytes[i] | invertedSubnetMaskBytes[i]);
        }

        broadcastAddressBytes[3] -= 1;

        //if (broadcastAddressBytes[2] == 255)
        //{
        //    broadcastAddressBytes[2] -= 1;

        //}

        //if (broadcastAddressBytes[3] == 255)
        //{
        //    broadcastAddressBytes[2] -= 1;

        //}

        return broadcastAddressBytes;

        //var invertedSubnetMaskBytes = new byte[4];
        //for (var i = 0; i < 4; i++)
        //{
        //    invertedSubnetMaskBytes[i] = (byte)~subnetMaskBytes[i];
        //}

        //var broadcastAddressBytes = new byte[4];
        //for (var i = 0; i < 4; i++)
        //{
        //    broadcastAddressBytes[i] = (byte)(networkAddressBytes[i] | invertedSubnetMaskBytes[i]);
        //}

        //// Adjust the broadcast address to find the last available address
        //for (var i = 3; i >= 0; i--)
        //{
        //    if (broadcastAddressBytes[i] != 255)
        //    {
        //        broadcastAddressBytes[i] -= 1;
        //        break;
        //    }
        //}

        //return broadcastAddressBytes;
    }

    private static IEnumerable<IPAddress> CreateAddresses16(IReadOnlyList<byte> startBytes, IReadOnlyList<byte> endBytes)
    {
        var ips = new List<IPAddress>();
        var first = true; 
        
        for (var i = startBytes[2]; i <= endBytes[2]; i++)
        {

            if (i == 255)
            {
                for (byte j = 0x01; j <= endBytes[3]; j++)
                {
                    ips.Add(new IPAddress(new byte[] { startBytes[0], startBytes[1], i, j }));
                }
                break;
            }

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
                
                for (byte j = 0x01; j < 255; j++)
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


