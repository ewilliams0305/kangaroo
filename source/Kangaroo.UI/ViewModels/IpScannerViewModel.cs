﻿using AsyncAwaitBestPractices;
using Avalonia.Controls.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kangaroo.UI.Models;
using Kangaroo.UI.Services.Database;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Kangaroo.UI.ViewModels;

public partial class IpScannerViewModel : ViewModelBase
{
    private readonly RecentScansRepository _recentScans;

    public IpScannerViewModel(RecentScansRepository recentScans)
    {
        _recentScans = recentScans;
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

        using var scanner = new ScannerBuilder()
            .WithRange(IPAddress.Parse(BeginIpAddress), IPAddress.Parse(EndIpAddress))
            .WithHttpScan()
            .WithMaxTimeout(TimeSpan.FromMilliseconds(1000))
            .WithMaxHops(10)
            //.WithParallelism()
            .Build();

        


        var queryTimes = new List<double>();
        var latencyTimes = new List<double>();
        var axisLabels = new List<string>( );

        scanner.NodeStatusUpdate = (node, status) =>
        {
            if (status == LiveUpdateStatus.Started)
            {
                NetworkNodes.Add(new NetworkNodeModel(node));
            }

            if (status == LiveUpdateStatus.Completed)
            {
            }

            queryTimes.Add(node.QueryTime.TotalMilliseconds);
            latencyTimes.Add(node.Latency != null 
                ? node.Latency!.Value.TotalMilliseconds 
                : 0);
            axisLabels.Add(node.IpAddress.ToString());

            LatencyStatistics[0].Values = latencyTimes;
            QueryStatistics[0].Values = queryTimes;
            IpAddressAxis[0].Labels = axisLabels;
        };

        scanner.ScanStatusUpdate = (results, status) =>
        {
            ScannedDeviceChart[0].Values = new int[] { results.NumberOfAddressesScanned };
            ScannedDeviceChart[1].Values = new int[] { results.NumberOfAliveNodes };
            base.OnPropertyChanged(nameof(ScannedDeviceChart));
        };

        var results = await scanner.QueryNetwork();

        await _recentScans.CreateAsync(
            RecentScan.FromRange(BeginIpAddress, EndIpAddress, TimeProvider.System, results.ElapsedTime, results.NumberOfAliveNodes));

        IsScanning = false;
    }

    private async Task LoadRecent()
    {
        var scans = await _recentScans.GetAsync();

        var last = scans.LastOrDefault();
        if (last != null)
        {
            BeginIpAddress = last.StartAddress?? string.Empty;
            EndIpAddress = last.EndAddress ?? string.Empty;
        }
    }
}
