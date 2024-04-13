using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Kangaroo.UI.Controls;
using Kangaroo.UI.Services;
using Kangaroo.UI.Services.Database;
using Kangaroo.UI.ViewModels;
using Kangaroo.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Path = System.IO.Path;

namespace Kangaroo.UI.Services;

public class ServiceOptions
{
    public string DatabaseConnection { get; set; }

    public ServiceOptions()
    {
        DatabaseConnection = $"Data Source={Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}kangaroo_scanner.db";
    }
}

public static class ContainerExtensions
{
    public static HostApplicationBuilder AddKangaroo(this HostApplicationBuilder builder, Action<ServiceOptions>? options = null)
    {
        var ops = new ServiceOptions();
        options?.Invoke(ops);

        return builder
            .AddDatabaseServices(ops)
            .AddScannerServices(ops)
            .AddViewModels(ops);
    }

    public static HostApplicationBuilder AddDatabaseServices(this HostApplicationBuilder builder, ServiceOptions options)
    {
        builder.Services.AddTransient<IDbConnectionFactory, SqliteDbConnectionFactory>(sp => new SqliteDbConnectionFactory(options.DatabaseConnection));
        builder.Services.AddTransient<IDbInitializer, SqliteDbInitializer>();

        builder.Services.AddTransient<RecentScansRepository>();
        return builder;
    }
    public static HostApplicationBuilder AddScannerServices(this HostApplicationBuilder builder, ServiceOptions options)
    {
        builder.Services.AddTransient<IScannerBuilder, ScannerBuilder>();
        return builder;
    }

    public static HostApplicationBuilder AddViewModels(this HostApplicationBuilder builder, ServiceOptions options)
    {
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


        builder.Services.AddTransient<ScanConfiguratorView>();
        builder.Services.AddTransient<ScanConfiguratorViewModel>();

        return builder;
    }
}

public static class ServiceExtensions
{
    public static IServiceProvider GetServiceProvider(this Control control)
    {
        var provider = control.FindResource(typeof(IServiceProvider));

        if (provider is not IServiceProvider services)
        {
            throw new InvalidOperationException("Cannot obtain the service provider");
        }

        return services;
    }

    public static T GetRequiredService<T>(this Control control) where T : notnull
    {
        var provider = control.FindResource(typeof(IServiceProvider));

        if (provider is not IServiceProvider services)
        {
            throw new InvalidOperationException("Cannot obtain the service provider");
        }

        return services.GetRequiredService<T>();
    }
}