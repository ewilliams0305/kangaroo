using System;
using System.Net;
using Kangaroo.UI.Controls;
using Kangaroo.UI.Models;

namespace Kangaroo.UI.Services.Database;

public sealed class RecentScan
{
    public Guid Id { get; set; }
    public ScanMode ScanMode { get; set; }
    public string? StartAddress { get; set; }
    public string? EndAddress { get; set; }
    public string? SubnetMask { get; set; }
    public string? SpecifiedAddresses { get; set; }
    public string? Adapter { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    public int OnlineDevices { get; set; }

    public static RecentScan FromResults(ScanResults? results, ScanConfiguration config, TimeProvider time) =>
        results == null
            ? new RecentScan()
            : new RecentScan 
            {
                Id = Guid.NewGuid(),
                ScanMode = config.ScanMode,
                StartAddress = results.StartAddress.ToString(),
                EndAddress = results.EndAddress.ToString(),
                CreatedDateTime = time.GetLocalNow().DateTime,
                ElapsedTime = results.ElapsedTime,
                OnlineDevices = results.NumberOfAliveNodes
            };
    

    public static RecentScan FromRange(string start, string end, TimeProvider time, TimeSpan elapsedTime, int onlineDevices) =>
        new()
        {
            Id = Guid.NewGuid(),
            ScanMode = ScanMode.AddressRange,
            StartAddress = start,
            EndAddress = end,
            CreatedDateTime = time.GetLocalNow().DateTime,
            ElapsedTime = elapsedTime,
            OnlineDevices = onlineDevices
        };

    public static RecentScan FromAdapter(string adapter, IPAddress address, IPAddress subnet, TimeProvider time, TimeSpan elapsedTime, int onlineDevices) =>
        new()
        {
            Id = Guid.NewGuid(),
            ScanMode = ScanMode.NetworkAdapter,
            StartAddress = address.ToString(),
            SubnetMask = subnet.ToString(),
            CreatedDateTime = time.GetLocalNow().DateTime,
            ElapsedTime = elapsedTime,
            OnlineDevices = onlineDevices
        };

    public static RecentScan FromSubnet(IPAddress address, IPAddress subnet, TimeProvider time, TimeSpan elapsedTime, int onlineDevices) =>
        new()
        {
            Id = Guid.NewGuid(),
            ScanMode = ScanMode.NetworkSubnet,
            StartAddress = address.ToString(),
            SubnetMask = subnet.ToString(),
            CreatedDateTime = time.GetLocalNow().DateTime,
            ElapsedTime = elapsedTime,
            OnlineDevices = onlineDevices
        };
}