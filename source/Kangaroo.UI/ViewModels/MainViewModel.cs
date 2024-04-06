using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kangaroo.UI.Models;
using Kangaroo.UI.Views;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Kangaroo.UI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IScannerBuilder _scanner;

    [ObservableProperty]
    private bool _isPaneOpen = true;

    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty]
    private ObservableCollection<MenuItemTemplate> _menuItems = new()
    {
        new MenuItemTemplate(typeof(HomePageViewModel), "Home"),
        new MenuItemTemplate(typeof(IpScannerViewModel), "IP Scanner"),
        new MenuItemTemplate(typeof(PortScannerViewModel), "Port Scanner"),
        new MenuItemTemplate(typeof(ConfigurationViewModel), "Configuration")
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