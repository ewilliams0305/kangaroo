using AsyncAwaitBestPractices;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kangaroo.UI.Controls;
using Kangaroo.UI.Models;
using Kangaroo.UI.Services.Database;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kangaroo.UI.ViewModels;

public partial class IpScannerViewModel : ViewModelBase
{
    private IScanner? _scanner;
    private readonly IScannerFactory _factory;
    private readonly RecentScansRepository _recentScans;

    /// <summary>
    /// Design view constructor
    /// </summary>
    public IpScannerViewModel()
    {

    }

    public IpScannerViewModel(RecentScansRepository recentScans, IScannerFactory factory)
    {
        _recentScans = recentScans;
        _factory = factory;
        factory.OnScannerCreated = (scanner, valid) =>
        {
            ScanEnabled = valid && scanner is not null;
            _scanner = scanner;
        };
        LoadRecent().SafeFireAndForget();
    }

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
    private Vector _scrollPosition;

    [ObservableProperty]
    private ObservableCollection<ISeries> _scannedDeviceChart = new()
    {
        new PieSeries<int>
        {
            Values = new int[] { 254 },
            Name = "ADDRESSES SCANNED",
            DataLabelsFormatter = data => $"{data} NODES",
            Fill = new SolidColorPaint(new SKColor(239,68, 56))
        },
        new PieSeries<int>
        {
            Values = new int[] { 0 },
            Name = "ADDRESSES LOCATED",
            DataLabelsFormatter = data => $"{data} NODES",
            Fill = new SolidColorPaint(new SKColor(33,150, 243))
        },
    };

    [ObservableProperty]
    private ObservableCollection<ISeries> _latencyStatistics = new()
        {
            new LineSeries<double> { Values = new List<double> { 0 }}
        };

    [ObservableProperty]
    private ObservableCollection<ISeries> _queryStatistics = new()
        {
            new ColumnSeries<double> { Values = new List<double> { 0 }},
        };

    [ObservableProperty]
    private ObservableCollection<Axis> _ipAddressAxis = new()
        {
            new Axis { Name = "IP ADDRESS", LabelsRotation = 45, TextSize = 10, Labels = new string[] { }  }
        };

    [ObservableProperty]
    private ObservableCollection<Axis> _latencyAxis = new()
        {
            new Axis { Name = "MILLISECONDS", MinStep = 1, TextSize = 10, Labeler = d => $"{d} ms."}
        };

    [ObservableProperty]
    private ObservableCollection<Axis> _queryTimeAxis = new()
        {
            new Axis { Name = "SECONDS", TextSize = 10, Labeler = d => $"{d / 1000:N2} sec." }
        };

    private CancellationTokenSource _cts;

    [RelayCommand]
    public void StopScan()
    {
        if(!IsScanning)
        {
            return;
        }

        _cts.Cancel();
        IsScanning = false;
    }

    [RelayCommand]
    public async Task StartScan()
    {
        if (!ScanEnabled)
        {
            return;
        }
        
        if (_scanner == null)
        {
            IsScanning = false;
            return;
        }

        _cts = new CancellationTokenSource();
        IsScanning = true;
        NetworkNodes = new ObservableCollection<NetworkNodeModel>();

        var queryTimes = new List<double>();
        var latencyTimes = new List<double>();
        var axisLabels = new List<string>();
        var counter = 0;
        var items = 0;

        _scanner.NodeStatusUpdate = (node, status) =>
        {
            if (status == LiveUpdateStatus.Started)
            {
                counter++;
                ScannedDeviceChart[1].Values = new int[] { counter };
                ScannedDeviceChart[0].Values = new int[] { items - counter};

                AddInitialNetworkNode(node, queryTimes, latencyTimes, axisLabels);
            }

            if (status == LiveUpdateStatus.Completed)
            {
                UpdateActiveNetworkNode(node, queryTimes, latencyTimes, axisLabels);
            }
        };

        _scanner.ScanStatusUpdate = (results, status) =>
        {
            if (status == LiveUpdateStatus.Started)
            {
                items = results.NumberOfAddressesScanned;

                ScannedDeviceChart[1].Values = new int[] { 0 };
                ScannedDeviceChart[0].Values = new int[] { results.NumberOfAddressesScanned };
            }

            if (status == LiveUpdateStatus.Completed)
            {
                ScannedDeviceChart[1].Values = new int[] { results.NumberOfAliveNodes };
                ScannedDeviceChart[0].Values = new int[] { results.NumberOfAddressesScanned };
                var orderedNodes = NetworkNodes.OrderBy(c => !c.IsAlive);
                NetworkNodes = new ObservableCollection<NetworkNodeModel>(orderedNodes);
            }
        };

        try
        {
            var results = await _scanner.QueryNetwork(_cts.Token);

            UpdateAliveChartData(results, queryTimes, latencyTimes, axisLabels);

            await _recentScans.CreateAsync(
                RecentScan.FromRange(BeginIpAddress, EndIpAddress, TimeProvider.System, results.ElapsedTime, results.NumberOfAliveNodes), _cts.Token);

            IsScanning = false;
        }
        catch (OperationCanceledException)
        {
            IsScanning = false;
        }
    }

    private async Task LoadRecent()
    {
        var scans = await _recentScans.GetAsync();

        var last = scans.LastOrDefault();
        if (last != null)
        {
            BeginIpAddress = last.StartAddress ?? string.Empty;
            EndIpAddress = last.EndAddress ?? string.Empty;
        }
    }

    private void AddInitialNetworkNode(NetworkNode node, List<double> queryTimes, List<double> latencyTimes, List<string> axisLabels)
    {
        NetworkNodes.Insert(0,new NetworkNodeModel(node));
 
        queryTimes.Add(node.QueryTime.TotalMilliseconds);
        latencyTimes.Add(node.Latency != null
            ? node.Latency!.Value.TotalMilliseconds
            : 0);
        axisLabels.Add(node.IpAddress.ToString());

        LatencyStatistics[0].Values = latencyTimes;
        QueryStatistics[0].Values = queryTimes;
        IpAddressAxis[0].Labels = axisLabels;
    }

    private void UpdateActiveNetworkNode(NetworkNode node, List<double> queryTimes, List<double> latencyTimes, List<string> axisLabels)
    {
        if (node.Alive)
        {
            var nodeToRemove = NetworkNodes.First(n => n.IpAddress == node.IpAddress.ToString());
            NetworkNodes.Remove(nodeToRemove);
            NetworkNodes.Insert(0, new NetworkNodeModel(node));

            queryTimes.Add(node.QueryTime.TotalMilliseconds);
            latencyTimes.Add(node.Latency != null
                ? node.Latency!.Value.TotalMilliseconds
                : 0);
            axisLabels.Add(node.IpAddress.ToString());

            LatencyStatistics[0].Values = latencyTimes;
            QueryStatistics[0].Values = queryTimes;
            IpAddressAxis[0].Labels = axisLabels;
        }
    }

    private void UpdateAliveChartData(ScanResults results, List<double> queryTimes, List<double> latencyTimes, List<string> axisLabels)
    {
        var updatedChart = results.Nodes
            .Where(n => n.Alive)
            .Select(n => new NetworkNodeModel(n));

        queryTimes.RemoveAll(t => t == 0);
        latencyTimes.RemoveAll(t => t == 0);
        axisLabels.Clear();

        foreach (var updatedNode in updatedChart)
        {
            axisLabels.Add(updatedNode.IpAddress);
        }

        LatencyStatistics[0].Values = latencyTimes;
        QueryStatistics[0].Values = queryTimes;
        IpAddressAxis[0].Labels = axisLabels;
    }

}
