using System;
using Kangaroo.UI.Controls;
using Kangaroo.UI.Database;
using Kangaroo.UI.ViewModels;
using Kangaroo.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScanConfiguratorView = Kangaroo.UI.Views.ScanConfiguratorView;
using ScanConfiguratorViewModel = Kangaroo.UI.ViewModels.ScanConfiguratorViewModel;

namespace Kangaroo.UI.Services;

public static class ContainerExtensions
{
    public static HostApplicationBuilder AddKangaroo(this HostApplicationBuilder builder, Action<ServiceOptions>? options = null)
    {
        var ops = new ServiceOptions();
        options?.Invoke(ops);

        builder.Services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });
        
        return builder
            .AddDatabaseServices(ops)
            .AddScannerServices(ops)
            .AddViewModels(ops);
    }

    private static HostApplicationBuilder AddDatabaseServices(this HostApplicationBuilder builder, ServiceOptions options)
    {
        builder.Services.AddTransient<IDbConnectionFactory, SqliteDbConnectionFactory>(sp => new SqliteDbConnectionFactory(options.DatabaseConnection));
        builder.Services.AddTransient<IDbInitializer, SqliteDbInitializer>();

        builder.Services.AddTransient<RecentScansRepository>();
        builder.Services.AddTransient<ComplianceRepository>();
        builder.Services.AddTransient<StoredResultsRepository>();
        return builder;
    }

    private static HostApplicationBuilder AddScannerServices(this HostApplicationBuilder builder, ServiceOptions options)
    {
        builder.Services.AddTransient<IScannerBuilder, ScannerBuilder>();
        builder.Services.AddSingleton<IScannerFactory, ScannerFactory>();

        builder.Services.AddTransient<ComplianceService>();
        return builder;
    }

    private static HostApplicationBuilder AddViewModels(this HostApplicationBuilder builder, ServiceOptions options)
    {
        builder.Services.AddTransient<MainWindow>();
        builder.Services.AddTransient<MainView>();
        builder.Services.AddTransient<MainViewModel>();

        builder.Services.AddTransient<HomePageView>();
        builder.Services.AddTransient<HomePageViewModel>();

        builder.Services.AddTransient<IpScannerView>();
        builder.Services.AddSingleton<IpScannerViewModel>();
        
        builder.Services.AddTransient<ComplianceView>();
        builder.Services.AddSingleton<ComplianceViewModel>();

        builder.Services.AddTransient<PortScannerView>();
        builder.Services.AddTransient<PortScannerViewModel>();

        builder.Services.AddTransient<ConfigurationView>();
        builder.Services.AddTransient<ConfigurationViewModel>();


        builder.Services.AddTransient<ScanConfiguratorView>();
        builder.Services.AddTransient<ScanConfiguratorViewModel>();

        return builder;
    }
}