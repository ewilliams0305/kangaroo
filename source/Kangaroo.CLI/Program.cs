using Cocona;
using Kangaroo;
using Kangaroo.CLI.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;

var builder = CoconaApp.CreateBuilder();

builder.Services.AddSingleton<ILogger>(x =>
{
    return LoggerFactory.Create(ops =>
    {
        ops.AddConsole();
    }).CreateLogger<Program>();
});

builder.Services.AddTransient<IScannerIpConfiguration>(sp => ScannerBuilder.Configure());
builder.Services.AddTransient<IEnumerable<IPAddress>>(sp => CreateIpAddresses());

var app = builder.Build();

app.AddCommands<IpScanCommand>();

app.Run();
return;

IEnumerable<IPAddress> CreateIpAddresses()
{
    var ipAddresses = new List<IPAddress>(254);

    for (var i = 1; i < 255; i++)
    {
        if (!IPAddress.TryParse($"172.26.6.{i}", out IPAddress? address))
        {
            continue;
        }

        ipAddresses.Add(address);
    }

    return ipAddresses;
}