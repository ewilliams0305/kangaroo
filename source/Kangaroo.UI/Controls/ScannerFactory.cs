using Kangaroo.UI.Models;
using System;

namespace Kangaroo.UI.Controls;

public interface IScannerFactory
{
    Action<IScanner?, ScannerOptions?, bool>? OnScannerCreated { get; set; }

    void CreateScanner(ScannerOptions options);
}


public sealed class ScannerFactory : IScannerFactory
{

    public Action<IScanner?, ScannerOptions?, bool>? OnScannerCreated { get; set; }

    public void CreateScanner(ScannerOptions options)
    {
        switch (options.ScanMode)
        {
            case ScanMode.AddressRange:
                CreateRangeScanner(options);
                break;
            case ScanMode.NetworkSubnet:
                CreateSubnetScanner(options);
                break;
            case ScanMode.NetworkAdapter:
                CreateAdapterScanner(options);
                break;
            case ScanMode.SingleAddress:
                CreateSingleScanner(options);
                break;
            case ScanMode.SpecifiedAddresses:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CreateRangeScanner(ScannerOptions options)
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

        OnScannerCreated?.Invoke(scanner, options, true);
    }

    private void CreateSingleScanner(ScannerOptions options)
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

        OnScannerCreated?.Invoke(scanner, options, true);
    }
    private void CreateSubnetScanner(ScannerOptions options)
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

        OnScannerCreated?.Invoke(scanner, options, true);
    }

    private void CreateAdapterScanner(ScannerOptions options)
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

        OnScannerCreated?.Invoke(scanner, options, true);
    }
}