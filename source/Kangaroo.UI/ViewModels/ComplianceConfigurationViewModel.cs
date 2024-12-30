using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using Kangaroo.UI.Database;
using Kangaroo.UI.Services;

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