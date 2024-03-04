using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Kangaroo.Queries;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading;

namespace Kangaroo;

public sealed class ScannerBuilder : IScannerIpConfiguration, IScannerOptions
{
    /// <summary>
    /// Starts the scanner configuration process.
    /// </summary>
    /// <returns>Returns a new scanner builder and begins the build pipeline</returns>
    public static IScannerIpConfiguration Configure()
    {
        return new ScannerBuilder();
    }

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

    public IScannerQueryOptions WithParallelism(int numberOfBatches = 10)
    {
        if (numberOfBatches == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfBatches));
        }
        _options.Concurrent = true;
        _options.ItemsPerBatch = numberOfBatches;
        return this;
    }

    /// <inheritdoc />
    public IScannerParallelismOptions WithMaxTimeout(TimeSpan timeout)
    {
        _options.Timeout = timeout;
        return this;
    }

    /// <inheritdoc />
    public IScannerParallelismOptions WithMaxHops(int ttl = 10)
    {
        _options.TimeToLive = ttl;
        return this;
    }

    public IScannerBuilder WithLogging(ILogger logger)
    {
        _options.Logger = logger;
        return this;
    }

    /// <inheritdoc />
    public IScannerBuilder WithLogging(Func<ILogger> loggerFactory)
    {
        _options.Logger = loggerFactory.Invoke();
        return this;
    }

    /// <inheritdoc />
    public IScannerBuilder WithLogging(ILoggerProvider loggerProvider)
    {
        ILoggerFactory fac = new NullLoggerFactory();
        fac.AddProvider(provider: loggerProvider);
        _options.Logger = fac.CreateLogger<IScanner>();
        return this;
    }

    /// <inheritdoc />
    public IScannerBuilder WithLogging(ILoggerFactory loggerFactory)
    {
        _options.Logger = loggerFactory.CreateLogger<IScanner>();
        return this;
    }

    /// <inheritdoc />
    public IScanner Build()
    {
        var addresses = CreateIpAddress();
        var queryOptions = new QueryOptions(_options.TimeToLive, _options.Timeout);
        
        var querier = new NetworkQuerierFactory(
            logger: _options.Logger,
            ping: _options.Concurrent
                ? new QueryPingResultsParallel(_options.Logger, queryOptions)
                : new QueryPingResultsOrderly(_options.Logger, new Ping(), queryOptions))
            .CreateQuerier();

        if (!_options.Concurrent)
        {
            return CreateOrderlyScanner(querier, addresses);
        }

        var ipAddresses = addresses as IPAddress[] ?? addresses.ToArray();
        return CreateParallelScanner(querier, ipAddresses, _options.ItemsPerBatch);
    }

    private IScanner CreateParallelScanner(
        IQueryNetworkNode querier,
        IEnumerable<IPAddress> addresses, 
        int batchSize) =>
        ParallelScanner.CreateScanner(
            _options.Logger,
            querier,
            addresses,
            batchSize);

    private IScanner CreateOrderlyScanner(
        IQueryNetworkNode querier,
        IEnumerable<IPAddress> addresses) =>
        OrderlyScanner.CreateScanner(
            _options.Logger,
            querier,
            addresses);

    private IEnumerable<IPAddress> CreateIpAddress()
    {
        if (_options.IpAddresses.Any())
        {
            return _options.IpAddresses;
        }

        if (_options is { RangeStart: not null, RangeStop: not null })
        {
            return CreateRange();
        }

        return new List<IPAddress>(1) { _options.IpAddress! };
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

    private IPAddress GetIpAddress()
    {
        return IPAddress.Any;
    }
    
}