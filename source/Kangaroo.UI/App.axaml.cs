using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using Kangaroo.UI.ViewModels;
using Kangaroo.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kangaroo.UI;

public partial class App : Application
{
    public static IServiceProvider? Services;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddTransient<MainWindow>();
        builder.Services.AddTransient<MainView>();
        builder.Services.AddTransient<MainViewModel>();
        
        builder.Services.AddTransient<HomePageView>();
        builder.Services.AddTransient<HomePageViewModel>();

        builder.Services.AddTransient<IpScannerView>();
        builder.Services.AddSingleton<IpScannerViewModel>();

        builder.Services.AddTransient<PortScannerView>();
        builder.Services.AddTransient<PortScannerViewModel>();

        builder.Services.AddTransient<ConfigurationView>();
        builder.Services.AddTransient<ConfigurationViewModel>();

        builder.Services.AddTransient<IScannerBuilder, ScannerBuilder>();

        var app = builder.Build();
        Services = app.Services;

        this.Resources[typeof(IHost)] = app;
        this.Resources[typeof(IServiceProvider)] = app.Services;

        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = app.Services.GetRequiredService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = app.Services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
