using System;
using Kangaroo.UI.Models;

namespace Kangaroo.UI.Services.Database;

public sealed class RecentScan
{
    public Guid Id { get; set; }
    public ScanMode ScanMode { get; set; }
    public string StartAddress { get; set; }
    public string EndAddress { get; set; }
    public string SubnetMask { get; set; }
    public string SpecifiedAddresses { get; set; }
    public string Adapter { get; set; }
    public DateTime CreatedDateTime { get; set; }

}