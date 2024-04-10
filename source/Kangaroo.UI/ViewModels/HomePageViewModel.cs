using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kangaroo.UI.Services.Database;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Kangaroo.UI.ViewModels;

public partial class HomePageViewModel : ViewModelBase
{
    private readonly RecentScansRepository _recentScansRepository;

    public HomePageViewModel(RecentScansRepository recentScansRepository)
    {
        _recentScansRepository = recentScansRepository;
        LoadRecent().SafeFireAndForget();

        //base.PropertyChanged += HomePageViewModel_PropertyChanged;
    }

    //private List<double> _items = new List<double>();
    //private List<double> _times = new List<double>();

    //private void HomePageViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //    if (e.PropertyName == nameof(SelectedScan))
    //    {
    //        if (SelectedScan == null)
    //        {
    //            return;
    //        }
    //        _selectedScans.Add(SelectedScan);
    //        _items.Add(SelectedScan.OnlineDevices);
    //        _times.Add(SelectedScan.ElapsedTime.TotalSeconds);
    //        RecentStatistics[1].Values = _items;
    //        RecentStatistics[0].Values = _times;
    //    }
    //}

    public void CompareSelectedItems(IList<RecentScan> scans)
    {
        _selectedScans = new List<RecentScan>(scans);
        //_times.Add(SelectedScan.ElapsedTime.TotalSeconds);
        RecentStatistics[1].Values = new List<double>(scans.Select(s => (double)s.OnlineDevices).ToList());
        RecentStatistics[0].Values = new List<double>(scans.Select(s => s.ElapsedTime.TotalSeconds).ToList()); 
    }


    private List<RecentScan> _selectedScans = new List<RecentScan>();


    [ObservableProperty]
    private ObservableCollection<ISeries> _recentStatistics = new()
    {
        new ColumnSeries<double> { Values = new List<double> { 0 }},
        new LineSeries<double> { Values = new List<double> { 0 }},
    };

    [ObservableProperty]
    private ObservableCollection<Axis> _recentAxis = new()
    {
        new Axis { Name = "DATE & TIME", MinStep = 1, TextSize = 10, Labeler = d => $"{d}"}
    };

    [ObservableProperty]
    private SolidColorPaint _legendTextPaint  = new SolidColorPaint
        {
            Color = new SKColor(120, 120, 120),
            SKTypeface = SKTypeface.FromFamilyName("Courier New")
        };

    [ObservableProperty]
    private ObservableCollection<RecentScan> _recentScans;

    [ObservableProperty]
    private RecentScan _selectedScan;

    [ObservableProperty]
    private ObservableCollection<ISeries> _scannedDeviceChart = new() { };

    [RelayCommand]
    public void TestCommand()
    {

    }

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
}
