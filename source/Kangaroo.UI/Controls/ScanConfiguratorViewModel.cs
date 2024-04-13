using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Kangaroo.UI.Models;
using Kangaroo.UI.ViewModels;
using System.Collections.ObjectModel;

namespace Kangaroo.UI.Controls;
public partial class ScanConfiguratorViewModel : ViewModelBase
{
    [ObservableProperty] 
    private ScanMode _selectedMode = ScanMode.AddressRange;

    [ObservableProperty] 
    private ObservableCollection<ScanMode> _scanModes = new()
    {
        ScanMode.AddressRange,
        ScanMode.SingleAddress,
        ScanMode.NetworkSubnet,
        ScanMode.SpecifiedAddresses,
        ScanMode.NetworkAdapter,
    };

    [ObservableProperty] 
    private ObservableCollection<NetworkAdapter> _adapters = new()
    {
        new NetworkAdapter { IpAddress = "127.0.0.1", MacAddress = "00000000", Name = "Ethernet" },
        new NetworkAdapter { IpAddress = "192.168.5.5", MacAddress = "00000000", Name = "Wifi" },
        new NetworkAdapter { IpAddress = "127.0.0.1", MacAddress = "00000000", Name = "Ethernet" },
    };

    [ObservableProperty]
    private NetworkAdapter _adapter;

    [ObservableProperty]
    private bool _showRangeFields = true;

    [ObservableProperty]
    private bool _showSubnetFields = true;

    [ObservableProperty]
    private bool _showAdapterFields = true;

    [ObservableProperty]
    private string _startAddress;

    [ObservableProperty]
    private string _endAddress;

    partial void OnSelectedModeChanged(ScanMode mode)
    {
        ShowRangeFields = mode == ScanMode.AddressRange;
        ShowSubnetFields = mode == ScanMode.NetworkSubnet;
        ShowAdapterFields = mode == ScanMode.NetworkAdapter;
    }

    public ScanConfiguratorViewModel()
    {

    }


}

public class NetworkAdapter
{
    public string Name { get; set; }
    public string IpAddress { get; set; }
    public string MacAddress { get; set; }
}