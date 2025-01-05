using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using Kangaroo.UI.Models;

namespace Kangaroo.UI.Controls;

public sealed class ScanConfiguration
{
    public ScanMode ScanMode { get; set; }
    public bool WithHttp { get; set; }
    public int Ttl { get; set; }
    public TimeSpan Timeout { get; set; }
    public IPAddress? StartAddress { get; set; }
    public IPAddress? EndAddress { get; set; }
    public IPAddress? SpecificAddress { get; set; }
    public IPAddress? NetmaskAddress { get; set; }
    public IEnumerable<IPAddress>? SpecificAddresses { get; set; }
    public NetworkInterface? NetworkInterface { get; set; }

    public static ScanConfiguration Nodes(IEnumerable<IPAddress> nodes) => new ScanConfiguration
    {
        ScanMode = ScanMode.SpecifiedAddresses,
        SpecificAddresses = nodes,
        WithHttp = true,
        Ttl = 5,
        Timeout = TimeSpan.FromSeconds(5)
    };
}