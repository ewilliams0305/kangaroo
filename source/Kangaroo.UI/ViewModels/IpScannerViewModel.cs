using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kangaroo.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Avalonia.Controls.Shapes;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

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

    [ObservableProperty]
    private int _progress;

    [ObservableProperty]
    private ObservableCollection<ISeries> _scannedDeviceChart = new ObservableCollection<ISeries>()
    {
        new PieSeries<double> { Values = new double[] { 254 } },
        new PieSeries<double> { Values = new double[] { 0 } },
    };

    [ObservableProperty]
    private ObservableCollection<ISeries> _scannedStatistics = new ObservableCollection<ISeries>()
        {
            //Query Time
            new ColumnSeries<double>
            {
                Values = new List<double> { 0 }
            },
            //Latency
            new LineSeries<double>
            {
                Values = new List<double> { 0 }
            }
        };

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
        NetworkNodes = new ObservableCollection<NetworkNodeModel>();

        var scanner = new ScannerBuilder()
            .WithRange(IPAddress.Parse(BeginIpAddress), IPAddress.Parse(EndIpAddress))
            .WithHttpScan()
            .WithMaxTimeout(TimeSpan.FromMilliseconds(250))
            .WithMaxHops(10)
            .WithParallelism()
            .Build();

        //var result = await scanner.QueryNetwork();

        //ScannedDeviceChart[0] = new PieSeries<double> { Values = new double[] { result.NumberOfAddressesScanned } };
        //ScannedDeviceChart[1] = new PieSeries<double> { Values = new double[] { result.NumberOfAliveNodes } };

        // base.OnPropertyChanged(nameof(ScannedDeviceChart));

        //NetworkNodes = new ObservableCollection<NetworkNodeModel>(result.Nodes.Select(n => new NetworkNodeModel(n)));

        var queryTimes = new List<double>();
        var latencyTimes = new List<double>();

        await foreach (var node in scanner.QueryNetworkNodes(live =>
                       {
                           queryTimes.Add(live.QueryTime.TotalMilliseconds);
                           latencyTimes.Add(live.Latency.TotalMilliseconds * 100);
                           
                           ScannedStatistics[1].Values = latencyTimes;
                           ScannedStatistics[0].Values = queryTimes;
                           ScannedDeviceChart[0].Values = new double[] { live.NumberOfAddressesScanned };
                           ScannedDeviceChart[1].Values = new double[] { live.NumberOfAliveNodes };
                           base.OnPropertyChanged(nameof(ScannedDeviceChart));

                       }))
        {
            NetworkNodes.Add(new NetworkNodeModel(node));
            base.OnPropertyChanged(nameof(NetworkNodes));
        }

        IsScanning = false;
    }
}
