using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kangaroo.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Kangaroo.UI.ViewModels;

public partial class IpScannerViewModel : ViewModelBase
{

    [ObservableProperty]
    private ObservableCollection<NetworkNodeModel> _networkNodes = new();

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private bool _scanEnabled;

    [ObservableProperty]
    private string _beginIpAddress = string.Empty;

    [ObservableProperty]
    private string _endIpAddress = string.Empty;

    partial void OnBeginIpAddressChanged(string value)
    {
        ScanEnabled = IPAddress.TryParse(BeginIpAddress, out var _) && IPAddress.TryParse(EndIpAddress, out var _);
    }
    partial void OnEndIpAddressChanged(string value)
    {
        ScanEnabled = IPAddress.TryParse(BeginIpAddress, out var _) && IPAddress.TryParse(EndIpAddress, out var _);
    }

    [RelayCommand]
    public async Task StartScan()
    {
        if (!ScanEnabled)
        {
            return;
        }

        IsScanning = true;

        var scanner = new ScannerBuilder()
            .WithRange(IPAddress.Parse(BeginIpAddress), IPAddress.Parse(EndIpAddress))
            .WithHttpScan()
            .WithMaxTimeout(TimeSpan.FromSeconds(1))
            .WithMaxHops(10)
            .WithParallelism()
            .Build();


        await foreach (var node in scanner.QueryNetworkNodes())
        {
            nodes.Add(node);

            NetworkNodes = new ObservableCollection<NetworkNodeModel>(nodes.Select(n => new NetworkNodeModel(n)));
        }

        IsScanning = false;
    }

    private List<NetworkNode> nodes = new List<NetworkNode>();

}
