using Kangaroo.UI.Controls;
using Kangaroo.UI.Models;
using System;

namespace Kangaroo.UI.Services;

/// <inheritdoc />
public sealed class ScannerFactory : IScannerFactory
{
    /// <inheritdoc />
    public Action<(IScanner? scanner, ScanConfiguration? configuration, bool valid)>? OnScannerCreated { get; set; }

    /// <inheritdoc />
    public void CreateScanner(ScanConfiguration options)
    {
        var scanData = options.ScanMode switch
        {
            ScanMode.AddressRange => CreateRangeScanner(options),
            ScanMode.NetworkSubnet => CreateSubnetScanner(options),
            ScanMode.NetworkAdapter => CreateAdapterScanner(options),
            ScanMode.SingleAddress => CreateSingleScanner(options),
            ScanMode.SpecifiedAddresses => CreateAddressScanner(options),
            _ => throw new ArgumentOutOfRangeException()
        };

        OnScannerCreated?.Invoke(scanData);
    }

    private (IScanner scanner, ScanConfiguration config, bool isValid) CreateRangeScanner(ScanConfiguration options)
    {
        ArgumentNullException.ThrowIfNull(options.StartAddress);
        ArgumentNullException.ThrowIfNull(options.EndAddress);

        var scanner = new ScannerBuilder()
            .WithRange(options.StartAddress, options.EndAddress)
            .WithOptions(ops =>
            {
                ops.EnableHttpScan = options.WithHttp;
                ops.Ttl = options.Ttl;
                ops.Timeout = options.Timeout;
            })
            .WithParallelism()
            .Build();

        return (scanner, options, true);
    }

    private (IScanner scanner, ScanConfiguration config, bool isValid) CreateSingleScanner(ScanConfiguration options)
    {
        ArgumentNullException.ThrowIfNull(options.SpecificAddress);

        var scanner = new ScannerBuilder()
            .WithAddress(options.SpecificAddress)
            .WithOptions(ops =>
            {
                ops.EnableHttpScan = options.WithHttp;
                ops.Ttl = options.Ttl;
                ops.Timeout = options.Timeout;
            })
            .WithParallelism()
            .Build();

        return (scanner, options, true);
    }
    
    private (IScanner scanner, ScanConfiguration config, bool isValid) CreateAddressScanner(ScanConfiguration options)
    {
        ArgumentNullException.ThrowIfNull(options.SpecificAddresses);

        var scanner = new ScannerBuilder()
            .WithAddresses(options.SpecificAddresses!)
            .WithOptions(ops =>
            {
                ops.EnableHttpScan = options.WithHttp;
                ops.Ttl = options.Ttl;
                ops.Timeout = options.Timeout;
            })
            .WithParallelism()
            .Build();

        return (scanner, options, true);
    }

    private (IScanner scanner, ScanConfiguration config, bool isValid) CreateSubnetScanner(ScanConfiguration options)
    {
        ArgumentNullException.ThrowIfNull(options.SpecificAddress);
        ArgumentNullException.ThrowIfNull(options.NetmaskAddress);

        var scanner = new ScannerBuilder()
            .WithSubnet(options.SpecificAddress, options.NetmaskAddress)
            .WithOptions(ops =>
            {
                ops.EnableHttpScan = options.WithHttp;
                ops.Ttl = options.Ttl;
                ops.Timeout = options.Timeout;
            })
            .WithParallelism()
            .Build();

        return (scanner, options, true);
    }

    private (IScanner scanner, ScanConfiguration config, bool isValid) CreateAdapterScanner(ScanConfiguration options)
    {
        ArgumentNullException.ThrowIfNull(options.NetworkInterface);

        var scanner = new ScannerBuilder()
            .WithInterface(options.NetworkInterface)
            .WithOptions(ops =>
            {
                ops.EnableHttpScan = options.WithHttp;
                ops.Ttl = options.Ttl;
                ops.Timeout = options.Timeout;
            })
            .WithParallelism()
            .Build();

        return (scanner, options, true);
    }
}