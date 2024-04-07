using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kangaroo.UI.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;

namespace Kangaroo.UI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IScannerBuilder _scanner;

    [ObservableProperty]
    private bool _isPaneOpen = true;

    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty]
    private MenuItemTemplate _selectedMenuItem;

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
        new MenuItemTemplate(typeof(HomePageViewModel), "Home", "IconHome"),
        new MenuItemTemplate(typeof(IpScannerViewModel), "IP Scanner", "IconScan"),
        new MenuItemTemplate(typeof(PortScannerViewModel), "Port Scanner", "IconPort"),
        new MenuItemTemplate(typeof(ConfigurationViewModel), "Configuration", "IconSettings")
    };

    public MainViewModel()
    {
        CurrentPage = new HomePageViewModel();
    }

    public MainViewModel(IScannerBuilder scanner, IServiceProvider provider)
    {
        _scanner = scanner;
        CurrentPage = provider.GetRequiredService<HomePageViewModel>();
    }

    [RelayCommand]
    public void TogglePaneOpen()
    {
        IsPaneOpen = !IsPaneOpen;
    }
}