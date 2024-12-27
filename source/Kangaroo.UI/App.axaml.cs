using System;
using System.Linq;
using AsyncAwaitBestPractices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Kangaroo.UI.Database;
using Kangaroo.UI.Services;
using Kangaroo.UI.ViewModels;
using Kangaroo.UI.Views;
using LiveChartsCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        builder.AddKangaroo();
        builder.Services.AddSingleton(TimeProvider.System);

        var app = builder.Build();

        var dbInitializer = app.Services.GetRequiredService<IDbInitializer>();
        dbInitializer.InitializeAsync().SafeFireAndForget(ex =>
        {
            var logger = app.Services.GetRequiredService<ILogger<App>>();
            logger.LogError(ex, "Failed to initialize the DB");
        });

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
