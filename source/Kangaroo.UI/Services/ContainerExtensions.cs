using System;
using Kangaroo.UI.Controls;
using Kangaroo.UI.Services.Database;
using Kangaroo.UI.ViewModels;
using Kangaroo.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScanConfiguratorView = Kangaroo.UI.Views.ScanConfiguratorView;
using ScanConfiguratorViewModel = Kangaroo.UI.ViewModels.ScanConfiguratorViewModel;

namespace Kangaroo.UI.Services;

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
        builder.Services.AddSingleton<IScannerFactory, ScannerFactory>();
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