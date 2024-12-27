using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Kangaroo.UI.Database;

namespace Kangaroo.UI.ViewModels;

public partial class HomePageViewModel : ViewModelBase
{
    private readonly RecentScansRepository _recentScansRepository;

    /// <summary>
    /// Design view constructor
    /// </summary>
    public HomePageViewModel()
    {
        
    }

    public HomePageViewModel(RecentScansRepository recentScansRepository)
    {
        _recentScansRepository = recentScansRepository;
        LoadRecent().SafeFireAndForget();
    }

    [ObservableProperty]
    private List<RecentScan> _selectedScans = new();

    [ObservableProperty]
    private ObservableCollection<RecentScan> _recentScans = new();

    [ObservableProperty]
    private ObservableCollection<ISeries> _scannedDeviceChart = new();

    [ObservableProperty]
    private ObservableCollection<ISeries> _recentStatistics = new()
    {
        new ColumnSeries<double> { Values = new List<double> { 0 }, YToolTipLabelFormatter = (point)=> $"{point.Model:N2} sec. query time"},
        new LineSeries<double> { Values = new List<double> { 0 }, YToolTipLabelFormatter = (point)=> $"{point.Model} devices online" },
    };

    [ObservableProperty]
    private ObservableCollection<Axis> _recentChartXAxis = new()
    {
        new Axis { Name = "DATE & TIME", LabelsRotation = 45, TextSize = 10, Labels = new string[]{}}
    };

    [ObservableProperty]
    private ObservableCollection<Axis> _recentChartYAxis = new()
    {
        new Axis { Name = "SECONDS", TextSize = 10, MinStep = 1, Labeler = d => $"{d:N2} sec." },
        new Axis { Name = "DEVICES", TextSize = 10, MinStep = 1, Labeler = d => $"{d} online" },
    };

    [ObservableProperty]
    private SolidColorPaint _legendTextPaint = new SolidColorPaint
    {
        Color = new SKColor(120, 120, 120),
        SKTypeface = SKTypeface.FromFamilyName("Courier New")
    };

    private async Task LoadRecent()
    {
        var scans = await _recentScansRepository.GetAsync();
        var items = scans.Reverse();
        var recentScans = items as RecentScan[] ?? items.ToArray();

        RecentScans = new ObservableCollection<RecentScan>(recentScans);

        foreach (var recentScan in recentScans.Take(10))
        {
            if (recentScan.OnlineDevices == 0)
            {
                continue;
            }

            ScannedDeviceChart.Add(new PieSeries<int>
            {
                Values = new List<int>(){ recentScan.OnlineDevices, (int)recentScan.ElapsedTime.TotalSeconds },
                Name = $"{recentScan.CreatedDateTime:MM/dd/yyyy h:mm:ss}",
                DataLabelsPaint = new SolidColorPaint(SKColor.Parse("1e1e1e")),
                DataLabelsSize = 12,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = data => data.Index == 0 ? $"{data.Model} ALIVE" :  $"{data.Model} SEC",
            });
        }
    }

    /// <summary>
    /// This function is called from the view
    /// The recent scan selected items are passed into the chart and updated via observable properties.
    /// </summary>
    /// <param name="scans">The selected items</param>
    public void CompareSelectedItems(IList<RecentScan> scans)
    {
        SelectedScans = new List<RecentScan>(scans);
        RecentStatistics[1].Values = new List<double>(scans.Select(s => (double)s.OnlineDevices).ToList());
        RecentStatistics[0].Values = new List<double>(scans.Select(s => s.ElapsedTime.TotalSeconds).ToList());

        ScannedDeviceChart = new ObservableCollection<ISeries>();
        var axisLabels = new List<string>(scans.Count);

        foreach (var recentScan in scans)
        {
            axisLabels.Add(recentScan.CreatedDateTime.ToString("MM/dd/yyyy h:mm:ss"));
            ScannedDeviceChart.Add(new PieSeries<int>
            {
                Values = new List<int>() { recentScan.OnlineDevices, (int)recentScan.ElapsedTime.TotalSeconds },
                Name = $"{recentScan.CreatedDateTime:MM/dd/yyyy h:mm:ss}",
                DataLabelsPaint = new SolidColorPaint(SKColor.Parse("1e1e1e")),
                DataLabelsSize = 12,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = data => data.Index == 0 ? $"{data.Model} ALIVE" : $"{data.Model} SEC",
            });
        }

        RecentChartXAxis[0].Labels = axisLabels;
    }
}
