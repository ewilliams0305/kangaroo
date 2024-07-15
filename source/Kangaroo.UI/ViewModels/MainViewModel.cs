#pragma warning disable IDE0028

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kangaroo.UI.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;

namespace Kangaroo.UI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isPaneOpen = true;

    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty]
    private MenuItemTemplate? _selectedMenuItem;

    partial void OnSelectedMenuItemChanged(MenuItemTemplate? value)
    {
        if (value == null)
        {
            return;
        }

        var obj = App.Services!.GetService(value.ModelType);

        if (obj is not ViewModelBase vm)
        {
            return;
        }

        CurrentPage = vm;
    }

    [ObservableProperty]
    private ObservableCollection<MenuItemTemplate> _menuItems = new()
    {
        new MenuItemTemplate(typeof(HomePageViewModel), "Dashboard", "IconHome", true),
        new MenuItemTemplate(typeof(IpScannerViewModel), "IP Scanner", "IconScan", true),
        new MenuItemTemplate(typeof(PortScannerViewModel), "Port Scanner", "IconPort", true),
        new MenuItemTemplate(typeof(PortScannerViewModel), "Compliance Testing", "IconClipboard", true),
        new MenuItemTemplate(typeof(ConfigurationViewModel), "Configuration", "IconSettings", true)
    };

    public MainViewModel()
    {
        CurrentPage = App.Services!.GetRequiredService<HomePageViewModel>();
    }

    public MainViewModel(IServiceProvider provider)
    {
        CurrentPage = provider.GetRequiredService<HomePageViewModel>();
    }

    [RelayCommand]
    public void TogglePaneOpen()
    {
        IsPaneOpen = !IsPaneOpen;
    }
}