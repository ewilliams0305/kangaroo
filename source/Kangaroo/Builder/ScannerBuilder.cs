using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Kangaroo;

internal sealed class ScannerBuilder : IScannerBuilder, IScannerConcurrent, IScannerIpConfiguration, IScannerOptions
{
    private readonly ScannerOptions _options = new();


    /// <inheritdoc />
    public IScannerOptions WithSubnet(byte subnet, IPAddress? address = null)
    {
        switch (subnet)
        {
            case <= 0:
                throw new ArgumentOutOfRangeException(nameof(subnet));
            case > 0x24:
                throw new ArgumentOutOfRangeException(nameof(subnet));
        }

        _options.NetMask = subnet;

        if (address == null)
        {
            var ip = GetIpAddress();
            _options.IpAddress = ip;
            return this;
        }

        _options.IpAddress = address;
        return this;
    }

    private IPAddress GetIpAddress()
    {
        return IPAddress.Any;
    }

    /// <inheritdoc />
    public IScannerOptions WithRange(IPAddress begin, IPAddress end)
    {
        _options.RangeStart = begin;
        _options.RangeStop = begin;
        return this;
    }

    /// <inheritdoc />
    public IScannerOptions WithAddresses(IEnumerable<IPAddress> addresses)
    {
        _options.IpAddresses = addresses;
        return this;
    }

    /// <inheritdoc />
    public IScannerOptions WithInterface(NetworkInterface @interface)
    {
        var ipInterfaceProperties = @interface.GetIPProperties();
        foreach (var addr in ipInterfaceProperties.UnicastAddresses)
        {
            if (addr.Address.AddressFamily != AddressFamily.InterNetwork)
            {
                continue;
            }

            _options.IpAddress = addr.Address;
        }

        return this;
    }

    /// <inheritdoc />
    public IScannerTimeoutOptions WithConcurrency(bool concurrent = false)
    {
        _options.Concurrent = concurrent;
        return this;
    }



    /// <inheritdoc />
    public IScanner Build()
    {
        if (_options.IpAddresses.Any())
        {
            return Scanner.CreateScanner(
                _options.IpAddresses,
                _options.Concurrent, 
                (int)_options.Timeout.TotalMilliseconds,
                exception: _options.ExceptionHandler,
                message: _options.MessageHandler);
        }

        if (_options is { RangeStart: not null, RangeStop: not null })
        {
            return Scanner.CreateScanner(
                CreateRange(),
                _options.Concurrent, 
                (int)_options.Timeout.TotalMilliseconds,
                exception: _options.ExceptionHandler,
                message: _options.MessageHandler);
        }

        return Scanner.CreateScanner(
            new List<IPAddress>(1) { _options.IpAddress! }, 
            _options.Concurrent, 
            (int)_options.Timeout.TotalMilliseconds,
            exception: _options.ExceptionHandler,
            message: _options.MessageHandler);
    }

    private IEnumerable<IPAddress> CreateRange()
    {
        var startBytes = _options.RangeStart!.GetAddressBytes();
        var endBytes = _options.RangeStart!.GetAddressBytes();

        try
        {
            if (BitConverter.ToUInt32(startBytes, 0) > BitConverter.ToUInt32(endBytes, 0))
            {
                (startBytes, endBytes) = (endBytes, startBytes);
            }

            var ipList = new List<IPAddress>();

            for (var i = BitConverter.ToUInt32(startBytes, 0); i <= BitConverter.ToUInt32(endBytes, 0); i++)
            {
                var bytes = BitConverter.GetBytes(i);
                Array.Reverse(bytes);
                ipList.Add(new IPAddress(bytes));
            }

            return ipList;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Enumerable.Empty<IPAddress>();
        }
    }

    #region Implementation of IScannerTimeout

    /// <inheritdoc />
    public IScannerConcurrentOptions WithNodeTimeout(TimeSpan timeout)
    {
        _options.Timeout = timeout;
        return this;
    }

    #endregion

    #region Implementation of IScannerLogging

    /// <inheritdoc />
    public IScannerBuilder WithLogging(Action<Exception>? exception, Action<string>? message)
    {
        if (exception != null)
        {
            _options.ExceptionHandler = exception;
        }

        if (message != null)
        {
            _options.MessageHandler = message;
        }

        return this;
    }

    #endregion
}