using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using CommunityToolkit.Mvvm.ComponentModel;
using Kangaroo.UI.Controls;
using Kangaroo.UI.Models;
using Kangaroo.UI.Services;

namespace Kangaroo.UI.ViewModels;
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
        IpAddress = GetAddressFromAdapter(i).ToString(),
        Name = i.Name,
        MacAddress = i.GetPhysicalAddress().ToString()

    }));

    [ObservableProperty]
    private NetworkAdapter? _adapter;

    [ObservableProperty]
    private bool _showRangeFields = true;

    [ObservableProperty]
    private bool _showSingleFields = false;

    [ObservableProperty]
    private bool _showSubnetFields = false;

    [ObservableProperty]
    private bool _showAdapterFields = false;

    [ObservableProperty]
    private string _startAddress = string.Empty;

    [ObservableProperty]
    private string _endAddress = string.Empty;

    [ObservableProperty]
    private string _ipAddress = string.Empty;

    [ObservableProperty]
    private string _netmaskAddress = string.Empty;

    partial void OnSelectedModeChanged(ScanMode value)
    {
        _factory.OnScannerCreated?.Invoke((null, null, false));
        StartAddress = string.Empty;
        EndAddress = string.Empty;
        NetmaskAddress = string.Empty;
        IpAddress = string.Empty;

        ShowRangeFields = value == ScanMode.AddressRange;
        ShowSubnetFields = value == ScanMode.NetworkSubnet;
        ShowAdapterFields = value == ScanMode.NetworkAdapter;
        ShowSingleFields = value == ScanMode.SingleAddress;

        if (value == ScanMode.NetworkAdapter)
        {
            Adapter = Adapters.FirstOrDefault<NetworkAdapter>();
        }
    }
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

    partial void OnStartAddressChanged(string value)
    {
        if (SelectedMode != ScanMode.AddressRange)
        {
            return;
        }

        if (IsValidIpAddress(StartAddress, out var start) &&
            IsValidIpAddress(EndAddress, out var end))
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
            return;
        }
        _factory.OnScannerCreated?.Invoke((null, null, false));
    }

    partial void OnEndAddressChanged(string value)
    {
        if (SelectedMode != ScanMode.AddressRange)
        {
            return;
        }

        if (IsValidIpAddress(StartAddress, out var start) &&
            IsValidIpAddress(EndAddress, out var end))
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

            return;
        }

        _factory.OnScannerCreated?.Invoke((null, null, false));
    }

    partial void OnIpAddressChanged(string value)
    {
        if (SelectedMode == ScanMode.SingleAddress)
        {
            if (IsValidIpAddress(IpAddress, out var singleAddress))
            {
                _factory.CreateScanner(new ScanConfiguration()
                {
                    ScanMode = SelectedMode,
                    Timeout = TimeSpan.FromSeconds(1),
                    Ttl = 10,
                    WithHttp = true,
                    SpecificAddress = singleAddress,
                });
                return;
            }
            _factory.OnScannerCreated?.Invoke((null, null, false));
        }

        if (SelectedMode != ScanMode.NetworkSubnet)
        {
            return;
        }

        if (IsValidIpAddress(IpAddress, out var ip) &&
            IsValidIpAddress(NetmaskAddress, out var mask))
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
            return;
        }
        _factory.OnScannerCreated?.Invoke((null, null, false));
    }
    partial void OnNetmaskAddressChanged(string value)
    {
        if (SelectedMode != ScanMode.NetworkSubnet)
        {
            return;
        }

        if (IsValidIpAddress(IpAddress, out var ip) &&
            IsValidIpAddress(NetmaskAddress, out var mask))
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
            return;
        }
        _factory.OnScannerCreated?.Invoke((null, null, false));
    }

    private bool IsValidIpAddress(string? ipAddressValue, out IPAddress? ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddressValue))
        {
            ipAddress = null;
            return false;
        }
        if (ipAddressValue.Length < 7)
        {
            ipAddress = null;
            return false;
        }
        if (ipAddressValue.Split('.').Length != 4)
        {
            ipAddress = null;
            return false;
        }

        return IPAddress.TryParse(ipAddressValue, out ipAddress);
    }

    private static IPAddress GetAddressFromAdapter(NetworkInterface @interface)
    {
        var props = @interface.GetIPProperties().UnicastAddresses.FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork);
        return props != null ? props.Address : IPAddress.Any;
    }

}