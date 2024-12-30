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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kangaroo.Compliance;
using Kangaroo.UI.Database;

namespace Kangaroo.UI.ViewModels;

public partial class ComplianceViewModel : ViewModelBase
{
    private CancellationTokenSource _cts;
    private IScanner? _scanner;
    private ScanConfiguration? _configuration;
    private readonly RecentScansRepository _recentScans;
    private readonly ComplianceService _service;

    /// <summary>
    /// Design view constructor
    /// </summary>
    public ComplianceViewModel()
    {
        _cts = new CancellationTokenSource();
    }

    public ComplianceViewModel(RecentScansRepository recentScans, ComplianceService service, IScannerFactory factory)
    {
        _cts = new CancellationTokenSource();
        _recentScans = recentScans;
        _service = service;
        factory.OnScannerCreated = scannerData =>
        {
            _scanner?.Dispose();
            ScanEnabled = scannerData is { valid: true, scanner: not null };
            _scanner = scannerData.scanner;
            _configuration = scannerData.configuration;
        };
        
        //LoadRecent().SafeFireAndForget();
    }

    [ObservableProperty]
    private ObservableCollection<NodeComplianceData> _networkNodes = new();

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
    private ObservableCollection<ISeries> _scannedDeviceChart =
        [
            new PieSeries<int>
            {
                Values = [254],
                Name = "ADDRESSES SCANNED",
                DataLabelsFormatter = data => $"{data} NODES",
                Fill = new SolidColorPaint(new SKColor(239, 68, 56))
            },

            new PieSeries<int>
            {
                Values = [0],
                Name = "ADDRESSES LOCATED",
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
        NetworkNodes = new ObservableCollection<NodeComplianceData>();

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

                // AddInitialNetworkNode( 
                //     NodeChecks.CheckNetworkNode(node, node, Options.CreateDefaultNodeOptions()), 
                //     queryTimes, 
                //     latencyTimes, 
                //     axisLabels);
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
                NetworkNodes = new ObservableCollection<NodeComplianceData>(orderedNodes);
            }
        };

        try
        {
            var results = await _scanner.QueryNetwork(_cts.Token);

            try
            {
                var compliance = ScanChecks.CheckForCompliance(results, results, Options.CreateDefaultOptions());

                switch (compliance)
                {
                    case Compliance.Compliance.Compliant compliant:
                        foreach (var check in compliant.Item.Checks)
                        {
                            Console.WriteLine(check);
                        }

                        foreach (var check in compliant.Item.Nodes)
                        {
                            Console.WriteLine(check);
                        }
                        break;

                    case Compliance.Compliance.Failure failures:
                        foreach (var check in failures.Item.Errors)
                        {
                            Console.WriteLine(check);
                        }
                    
                        foreach (var check in failures.Item.Nodes)
                        {
                            Console.WriteLine(check);
                        }
                        break;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            UpdateAliveChartData(results, queryTimes, latencyTimes, axisLabels);

            await _recentScans.CreateAsync(
                RecentScan.FromResults(results, _configuration, TimeProvider.System), _cts.Token);

            IsScanning = false;
        }
        catch (OperationCanceledException)
        {
            IsScanning = false;
        }
    }
    private void UpdateActiveNetworkNode(NetworkNode node, List<double> queryTimes, List<double> latencyTimes, List<string> axisLabels)
    {
        if (node.Alive)
        {
            var nodeToRemove = NetworkNodes.First(n => Equals(n.Node.IpAddress, node.IpAddress));
            NetworkNodes.Remove(nodeToRemove);
            NetworkNodes.Insert(0, NodeChecks.CheckNetworkNode(node, node, Options.CreateDefaultNodeOptions()));

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
