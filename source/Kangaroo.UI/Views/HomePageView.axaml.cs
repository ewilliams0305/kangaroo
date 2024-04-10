using Avalonia.Controls;
using Avalonia.Interactivity;
using Kangaroo.UI.Services.Database;
using Kangaroo.UI.ViewModels;
using System.Collections.Generic;

namespace Kangaroo.UI.Views;
public partial class HomePageView : UserControl
{
    private HomePageViewModel? _vm;

    public HomePageView()
    {
        InitializeComponent();
    }

    #region Overrides of Control

    /// <inheritdoc />
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is HomePageViewModel vm)
        {
            _vm = vm;
        }
    }

    #endregion

    private void DataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_vm == null)
        {
            return;
        }

        if (e.Source is not DataGrid grid)
        {
            return;
        }

        var scans = new List<RecentScan>(grid.SelectedItems.Count);

        foreach (var item in grid.SelectedItems)
        {
            if (item is not RecentScan scan)
            {
                continue;
            }
            scans.Add(scan);
        }

        _vm.CompareSelectedItems(scans);
    }
}
