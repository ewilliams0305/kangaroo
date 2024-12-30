using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using Kangaroo.UI.Controls;
using Kangaroo.UI.Database;
using Kangaroo.UI.Models;
using Kangaroo.UI.Services;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace Kangaroo.UI.ViewModels;
public partial class ComplianceConfigurationViewModel : ViewModelBase
{
    private readonly ComplianceService _service;
    
    public ComplianceConfigurationViewModel(ComplianceService service)
    {
        _service = service;
        LoadRecent().SafeFireAndForget(ex =>
        {
            //logger.LogError(ex, "Failed to load recent scans");
        });
    }

    [ObservableProperty] 
    private RecentScan _selectedScan;

    [ObservableProperty] 
    private ObservableCollection<RecentScan> _recentScans = new ObservableCollection<RecentScan>();
    
    private async Task LoadRecent()
    {
        var scans = await _service.GetRecentScans();
        var items = scans.Reverse();
        var recentScans = items as RecentScan[] ?? items.ToArray();
    
        RecentScans = new ObservableCollection<RecentScan>(recentScans);
    }
}