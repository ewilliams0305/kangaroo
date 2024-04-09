using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
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

        base.PropertyChanged += HomePageViewModel_PropertyChanged;
    }

    private void HomePageViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedScan))
        {
            _selectedScans.Add(SelectedScan);

        }
    }

    private List<RecentScan> _selectedScans = new List<RecentScan>();

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

    private async Task LoadRecent()
    {
        var scans = await _recentScansRepository.GetAsync();
        var recentScans = scans as RecentScan[] ?? scans.ToArray();
        var reversed = recentScans.Reverse();
        var collection = reversed as RecentScan[] ?? reversed.ToArray();
        RecentScans = new ObservableCollection<RecentScan>(collection);

        foreach (var recentScan in collection.Take(collection.Count() >= 10 ? 10 : collection.Count()))
        {
            ScannedDeviceChart.Add(new PieSeries<int>
            {
                Values = new int[] { recentScan.OnlineDevices },
                Name = $"{recentScan.CreatedDateTime:MM/dd/yyyy h:mm:ss}",
                DataLabelsFormatter = data => $"{data} ALIVE",
            });
        }
    }
}
