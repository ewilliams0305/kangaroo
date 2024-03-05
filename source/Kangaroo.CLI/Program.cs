using Cocona;
using Kangaroo;
using Kangaroo.CLI.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = CoconaApp.CreateBuilder();

builder.Services.AddSingleton<ILogger>(x =>
{
    return LoggerFactory.Create(ops =>
    {
        ops.AddConsole();
    }).CreateLogger<Program>();
});

builder.Services.AddTransient(sp => ScannerBuilder.Configure());

var app = builder.Build();

app.AddCommands<IpScanCommand>();

app.Run();