using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Kangaroo.UI.Models;
using Kangaroo.UI.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Kangaroo.UI.Services;

namespace Kangaroo.UI.Controls;
public partial class ScanConfiguratorViewModel : ViewModelBase
{
    private readonly IScannerFactory _factory;

    public ScanConfiguratorViewModel(IScannerFactory factory)
    {
        _factory = factory;
    }

    [ObservableProperty] 
    private ScanMode _selectedMode = ScanMode.AddressRange;

    [ObservableProperty] 
    private ObservableCollection<ScanMode> _scanModes = new()
    {
        ScanMode.AddressRange,
        ScanMode.SingleAddress,
        ScanMode.NetworkSubnet,
        ScanMode.NetworkAdapter,
    };

    [ObservableProperty] 
    private ObservableCollection<NetworkAdapter> _adapters = new(AddressFactory.GetInterfaces().Select(i => new NetworkAdapter()
    {
        IpAddress = i.GetIPProperties().UnicastAddresses.Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault().Address.ToString(),
        Name = i.Name,
        MacAddress = i.GetPhysicalAddress().ToString()

    }));

    [ObservableProperty]
    private NetworkAdapter? _adapter;

    partial void OnAdapterChanged(NetworkAdapter? value)
    {
        if (SelectedMode != ScanMode.NetworkAdapter)
        {
            return;
        }

        if (Adapter != null)
        {
            _factory.CreateScanner(new ScanConfiguration()
            {
                NetworkInterface = AddressFactory.GetInterfaces().First(i => i.Name == Adapter.Name),
                ScanMode = ScanMode.NetworkAdapter,
                Timeout = TimeSpan.FromSeconds(1),
                Ttl = 10,
                WithHttp = true
            });
        }
    }

    [ObservableProperty]
    private bool _showRangeFields = true;

    [ObservableProperty]
    private bool _showSingleFields = false;

    [ObservableProperty]
    private bool _showSubnetFields = false;

    [ObservableProperty]
    private bool _showAdapterFields = false;

    [ObservableProperty]
    private string _startAddress;

    [ObservableProperty]
    private string _endAddress;

    partial void OnStartAddressChanged(string value)
    {
        if (SelectedMode != ScanMode.AddressRange)
        {
            return;
        }

        if (IPAddress.TryParse(StartAddress, out var start) &&
            IPAddress.TryParse(EndAddress, out var end))
        {
            _factory.CreateScanner(new ScanConfiguration()
            {
                ScanMode = ScanMode.AddressRange,
                Timeout = TimeSpan.FromSeconds(1),
                Ttl = 10,
                WithHttp = true,
                StartAddress = start,
                EndAddress = end
            });
        }
    }
    partial void OnEndAddressChanged(string value)
    {
        if (SelectedMode != ScanMode.AddressRange)
        {
            return;
        }

        if (IPAddress.TryParse(StartAddress, out var start) &&
            IPAddress.TryParse(EndAddress, out var end))
        {
            
            _factory.CreateScanner(new ScanConfiguration()
            {
                ScanMode = ScanMode.AddressRange,
                Timeout = TimeSpan.FromSeconds(1),
                Ttl = 10,
                WithHttp = true,
                StartAddress = start,
                EndAddress = end
            });
        }
    }

    [ObservableProperty]
    private string _ipAddress;

    [ObservableProperty]
    private string _netmaskAddress;

    partial void OnIpAddressChanged(string value)
    {
        if (SelectedMode == ScanMode.SingleAddress)
        {
            if (IPAddress.TryParse(IpAddress, out var singleAddress))
            {
                _factory.CreateScanner(new ScanConfiguration()
                {
                    ScanMode = SelectedMode,
                    Timeout = TimeSpan.FromSeconds(1),
                    Ttl = 10,
                    WithHttp = true,
                    SpecificAddress = singleAddress,
                });
            }
        }

        if (SelectedMode != ScanMode.NetworkSubnet)
        {
            return;
        }

        if (IPAddress.TryParse(IpAddress, out var ip) &&
            IPAddress.TryParse(NetmaskAddress, out var mask))
        {
            _factory.CreateScanner(new ScanConfiguration()
            {
                ScanMode = SelectedMode,
                Timeout = TimeSpan.FromSeconds(1),
                Ttl = 10,
                WithHttp = true,
                SpecificAddress = ip,
                NetmaskAddress = mask
            });
        }
    }
    partial void OnNetmaskAddressChanged(string value)
    {
        if (SelectedMode != ScanMode.NetworkSubnet)
        {
            return;
        }

        if (IPAddress.TryParse(IpAddress, out var ip) &&
            IPAddress.TryParse(NetmaskAddress, out var mask))
        {
            
            _factory.CreateScanner(new ScanConfiguration()
            {
                ScanMode = SelectedMode,
                Timeout = TimeSpan.FromSeconds(1),
                Ttl = 10,
                WithHttp = true,
                SpecificAddress = ip,
                NetmaskAddress = mask
            });
        }
    }

    partial void OnSelectedModeChanged(ScanMode selectedMode)
    {
        ShowRangeFields = selectedMode == ScanMode.AddressRange;
        ShowSubnetFields = selectedMode == ScanMode.NetworkSubnet;
        ShowAdapterFields = selectedMode == ScanMode.NetworkAdapter;
        ShowSingleFields = selectedMode == ScanMode.SingleAddress;

        if (selectedMode == ScanMode.NetworkAdapter)
        {
            Adapter = Adapters.FirstOrDefault();
        }
    }

}