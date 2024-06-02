using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
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

    [ObservableProperty]
    private string _validationError = string.Empty;

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
            ValidationError = TryCreateScanner(new ScanConfiguration()
            {
                NetworkInterface = AddressFactory.GetInterfaces().First(i => i.Name == Adapter.Name),
                ScanMode = ScanMode.NetworkAdapter,
                Timeout = TimeSpan.FromSeconds(1),
                Ttl = 10,
                WithHttp = true
            }) ?? string.Empty;
        }
    }

    partial void OnStartAddressChanged(string value)
    {
        if (SelectedMode != ScanMode.AddressRange)
        {
            return;
        }

        if (!IsValidIpAddress(StartAddress, out var start))
        {
            ValidationError = $"{value} is not a valid starting IP address";
            _factory.OnScannerCreated?.Invoke((null, null, false));
            return;
        }
        
        if (!IsValidIpAddress(EndAddress, out var end))
        {
            ValidationError = "";
            _factory.OnScannerCreated?.Invoke((null, null, false));
            return;
        }

        ValidationError = TryCreateScanner(new ScanConfiguration()
        {
            ScanMode = ScanMode.AddressRange,
            Timeout = TimeSpan.FromSeconds(1),
            Ttl = 10,
            WithHttp = true,
            StartAddress = start,
            EndAddress = end
        }) ?? string.Empty;

    }

    partial void OnEndAddressChanged(string value)
    {
        if (SelectedMode != ScanMode.AddressRange)
        {
            return;
        }

        if (!IsValidIpAddress(EndAddress, out var end))
        {
            ValidationError = $"{value} is not a valid ending IP address";
            _factory.OnScannerCreated?.Invoke((null, null, false));

            return;
        }
        
        if (!IsValidIpAddress(StartAddress, out var start))
        {
            ValidationError = $"{StartAddress} is not a valid starting IP address";
            _factory.OnScannerCreated?.Invoke((null, null, false));

            return;
        }
        
        ValidationError = TryCreateScanner(new ScanConfiguration()
        {
            ScanMode = ScanMode.AddressRange,
            Timeout = TimeSpan.FromSeconds(1),
            Ttl = 10,
            WithHttp = true,
            StartAddress = start,
            EndAddress = end
        }) ?? string.Empty;
    }

    partial void OnIpAddressChanged(string value)
    {
        if (SelectedMode == ScanMode.SingleAddress)
        {
            if (!IsValidIpAddress(IpAddress, out var singleAddress))
            {
                ValidationError = $"{value} is not a valid IP address";
                _factory.OnScannerCreated?.Invoke((null, null, false));
            }
            ValidationError = TryCreateScanner(new ScanConfiguration()
            {
                ScanMode = SelectedMode,
                Timeout = TimeSpan.FromSeconds(1),
                Ttl = 10,
                WithHttp = true,
                SpecificAddress = singleAddress,
            }) ?? string.Empty;

            return;
        }

        if (SelectedMode != ScanMode.NetworkSubnet)
        {
            return;
        }

        if (!IsValidIpAddress(IpAddress, out var ip))
        {
            ValidationError = $"{value} is not a valid IP address";
            _factory.OnScannerCreated?.Invoke((null, null, false));
            return;
        }

        if (!IsValidIpAddress(NetmaskAddress, out var mask))
        {
            _factory.OnScannerCreated?.Invoke((null, null, false));
            ValidationError = "";
            return;
        }

        ValidationError = TryCreateScanner(new ScanConfiguration()
        {
            ScanMode = SelectedMode,
            Timeout = TimeSpan.FromSeconds(1),
            Ttl = 10,
            WithHttp = true,
            SpecificAddress = ip,
            NetmaskAddress = mask
        })?? string.Empty;
    }
    partial void OnNetmaskAddressChanged(string value)
    {
        if (SelectedMode != ScanMode.NetworkSubnet)
        {
            return;
        }

        if (!IsValidIpAddress(NetmaskAddress, out var mask))
        {
            ValidationError = $"{value} invalid subnet mask IP address";
            _factory.OnScannerCreated?.Invoke((null, null, false));
            return;
        }

        if (!IsValidIpAddress(IpAddress, out var ip))
        {
            ValidationError = $"{IpAddress} invalid IP address";
            _factory.OnScannerCreated?.Invoke((null, null, false));
            return;
        }

        ValidationError = TryCreateScanner(new ScanConfiguration()
        {
            ScanMode = SelectedMode,
            Timeout = TimeSpan.FromSeconds(1),
            Ttl = 10,
            WithHttp = true,
            SpecificAddress = ip,
            NetmaskAddress = mask
        })?? string.Empty;
    }

    private string? TryCreateScanner(ScanConfiguration configuration)
    {
        try
        {
            _factory.CreateScanner(configuration);
            return null;
        }
        catch (Exception e)
        {
            return e switch
            {
                InvalidIpAddressException ex => ex.Message,
                InvalidIpRangeException ex => ex.Message,
                InvalidNetworkAdapterException ex => ex.Message,
                InvalidSubnetException ex => ex.Message,
                InvalidTimeoutException ex => ex.Message,
                InvalidTtlException ex => ex.Message,
                _ => e.Message
            };
        }
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