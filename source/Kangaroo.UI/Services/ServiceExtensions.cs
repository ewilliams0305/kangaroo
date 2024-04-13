using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Kangaroo.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kangaroo.UI.Services;

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