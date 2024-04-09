using System;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using Kangaroo.UI.Services.Database;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;

namespace Kangaroo.UI.ViewModels;

public partial class HomePageViewModel : ViewModelBase
{
    private readonly RecentScansRepository _recentScansRepository;

    public HomePageViewModel(RecentScansRepository recentScansRepository)
    {
        _recentScansRepository = recentScansRepository;
        
        LoadRecent().SafeFireAndForget(ex =>
        {
            Console.WriteLine(ex);
        });
    }

    [ObservableProperty]
    private ObservableCollection<RecentScan> _recentScans;

    [ObservableProperty]
    private ObservableCollection<ISeries> _scannedDeviceChart = new() { };

    private async Task LoadRecent()
    {
        var scans = await _recentScansRepository.GetAsync();
        RecentScans = new ObservableCollection<RecentScan>(scans);

        foreach (var recentScan in scans)
        {
            ScannedDeviceChart.Add(new PieSeries<int>
            {
                Values = new int[] { recentScan.OnlineDevices },
                Name = recentScan.CreatedDateTime.ToString(CultureInfo.InvariantCulture),
                DataLabelsFormatter = data => $"{data} NODES"
            });
        }
        
    }
}
