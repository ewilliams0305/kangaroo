using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kangaroo.UI.Controls;
using Kangaroo.UI.Models;
using Kangaroo.UI.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Kangaroo.Compliance;
using Kangaroo.UI.Database;
using Microsoft.Extensions.Logging;

namespace Kangaroo.UI.ViewModels;

public partial class ComplianceViewModel : ViewModelBase
{
    private CancellationTokenSource _cts;
    private IScanner? _scanner;
    private ScanConfiguration? _configuration;
    private readonly ILogger<ComplianceViewModel> _logger;
    private readonly RecentScansRepository _recentScansRepository;
    private readonly ComplianceService _service;
    private readonly IScannerFactory _factory;
    
    private ConcurrentDictionary<string, NetworkNode> _nodeToCheck = new ();

    /// <summary>
    /// Design view constructor
    /// </summary>
    public ComplianceViewModel()
    {
        _cts = new CancellationTokenSource();
    }

    public ComplianceViewModel(ILogger<ComplianceViewModel> logger, RecentScansRepository recentScans, ComplianceService service, IScannerFactory factory)
    {
        _cts = new CancellationTokenSource();
        _logger = logger;
        _recentScansRepository = recentScans;
        _service = service;
        _factory = factory;

        _factory.OnScannerCreated = scannerData =>
        {
            _scanner?.Dispose();
            ScanEnabled = scannerData is { valid: true, scanner: not null };
            _scanner = scannerData.scanner;
            _configuration = scannerData.configuration;
        };
        
        LoadRecent().SafeFireAndForget();
    }
    
    [ObservableProperty] 
    private RecentScan _selectedScan;

    [ObservableProperty] 
    private ObservableCollection<RecentScan> _recentScans = new ObservableCollection<RecentScan>();

    [ObservableProperty]
    private ObservableCollection<CompliantViewNodeModel> _networkNodes = new();

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private bool _scanEnabled;

    [ObservableProperty]
    private int _progress;

    [ObservableProperty]
    private Vector _scrollPosition;

    [ObservableProperty]
    private ObservableCollection<ISeries> _scannedDeviceChart =
        [
            new PieSeries<int>
            {
                Values = [254],
                Name = "FAILED NODES",
                DataLabelsFormatter = data => $"{data} NODES",
                Fill = new SolidColorPaint(new SKColor(239, 68, 56))
            },

            new PieSeries<int>
            {
                Values = [0],
                Name = "COMPLIANT NODES",
                DataLabelsFormatter = data => $"{data} NODES",
                Fill = new SolidColorPaint(new SKColor(33, 150, 243))
            }
        ];

    [ObservableProperty]
    private ObservableCollection<ISeries> _latencyStatistics =
        [
            new LineSeries<double> { Values = new List<double> { 0 } }
        ];

    [ObservableProperty]
    private ObservableCollection<ISeries> _queryStatistics =
        [
            new ColumnSeries<double> { Values = new List<double> { 0 } }
        ];

    [ObservableProperty]
    private ObservableCollection<Axis> _ipAddressAxis =
        [
            new Axis { Name = "IP ADDRESS", LabelsRotation = 45, TextSize = 10, Labels = [] }
        ];

    [ObservableProperty]
    private ObservableCollection<Axis> _latencyAxis =
        [
            new Axis { Name = "MILLISECONDS", MinStep = 1, TextSize = 10, Labeler = d => $"{d} ms." }
        ];

    [ObservableProperty]
    private ObservableCollection<Axis> _queryTimeAxis =
        [
            new Axis { Name = "SECONDS", TextSize = 10, Labeler = d => $"{d / 1000:N2} sec." }
        ];

    [RelayCommand]
    public void StopScan()
    {
        if (!IsScanning)
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
                ScannedDeviceChart[0].Values = new int[] { items - counter };

                UpdateStartingNetworkNode(node);
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
                var orderedNodes = NetworkNodes.OrderBy(c => c.IsCompliant.Equals(IsCompliant.Compliant));
                NetworkNodes = new ObservableCollection<CompliantViewNodeModel>(orderedNodes);
            }
        };

        try
        {
            var results = await _scanner.QueryNetwork(_cts.Token).ConfigureAwait(false);
            var compliance = ScanChecks.CheckForCompliance(results, results, Options.CreateDefaultOptions());
            
            UpdateAliveChartData(compliance, results, queryTimes, latencyTimes, axisLabels);
            IsScanning = false;
        }
        catch (OperationCanceledException)
        {
            IsScanning = false;
        }
    }

    private async Task LoadRecent()
    {
        var scans = await _service.GetRecentScans();
        var items = scans.Reverse();
        var recentScans = items as RecentScan[] ?? items.ToArray();

        RecentScans = new ObservableCollection<RecentScan>(recentScans);
    }

    partial void OnSelectedScanChanged(RecentScan value)
    {
        LoadScanData(value).SafeFireAndForget(ex =>
        {
            _logger.LogError(ex, "Failed to convert recent scan to data");
        });
    }

    private async Task LoadScanData(RecentScan scan)
    {
        var data = await _service.SelectRecentScanResult(scan).ConfigureAwait(false);

        if (data is null)
        {
            ScanEnabled = false;
            return;
            // TODO: show some sort of error here communicating the scan cant be used
        }

        _factory.CreateScanner(
            ScanConfiguration.Nodes(
                data.Nodes.Select(n =>
                IPAddress.Parse(n.IpAddress))));

        ScanEnabled = true;
        
        _nodeToCheck.Clear();
        NetworkNodes.Clear();
        
        foreach (var node in data.Nodes.Where(n => n.Alive))
        {
            var complianceNode = node.ToNetworkNode();
            
            _nodeToCheck.TryAdd(node.IpAddress, complianceNode);
            NetworkNodes.Add(new CompliantViewNodeModel(node: complianceNode));
        }
    }

    private void UpdateStartingNetworkNode(NetworkNode node)
    {
        var index = NetworkNodes.ToList().FindIndex(n => Equals(n.Node!.IpAddress, node.IpAddress));

        NetworkNodes.RemoveAt(index);
        NetworkNodes.Insert(index, new CompliantViewNodeModel(node)
        {
            IsRunning = true,
        });
    }
    
    private void UpdateActiveNetworkNode(NetworkNode node, List<double> queryTimes, List<double> latencyTimes, List<string> axisLabels)
    {
        try
        {
            var index = NetworkNodes.ToList().FindIndex(n => Equals(n.Node!.IpAddress, node.IpAddress));
            var originalNode = NetworkNodes.First(n => Equals(n.Node!.IpAddress, node.IpAddress));
            
            var compliance = NodeChecks.CheckNetworkNode(
                _nodeToCheck[node.IpAddress.ToString()], 
                node, 
                Options.CreateDefaultNodeOptions());

            NetworkNodes.RemoveAt(index);
            NetworkNodes.Insert(index, new CompliantViewNodeModel(compliance));

            queryTimes.Add(node.QueryTime.TotalMilliseconds);

            latencyTimes.Add(node.Latency != null
                ? node.Latency.Value.TotalMilliseconds
                : 0);

            axisLabels.Add(node.IpAddress.ToString());

            LatencyStatistics[0].Values = latencyTimes;
            QueryStatistics[0].Values = queryTimes;
            IpAddressAxis[0].Labels = axisLabels;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void UpdateAliveChartData(Compliance.Compliance? compliance, ScanResults results, List<double> queryTimes, List<double> latencyTimes, List<string> axisLabels)
    {
        // TODO: Update charts with compliance

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
