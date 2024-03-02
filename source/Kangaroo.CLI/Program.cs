
using Kangaroo;
using System.Diagnostics;
using System.Net;

var ips = CreateIpAddresses();

using var scanner1 = Scanner
    .Configure()
    .WithAddresses(ips)
    .WithConcurrency(concurrent: true)
    .WithNodeTimeout(TimeSpan.FromMilliseconds(250))
    .WithLogging(
        exception =>
        {
            Console.WriteLine($"SCANNER 1: {exception.Message}");
        },
        message =>
        {
            Console.WriteLine($"SCANNER 1: {message}");
        })
    .Build();

using var scanner2 = Scanner
    .Configure()
    .WithAddresses(ips)
    .WithConcurrency()
    .WithNodeTimeout(TimeSpan.FromMilliseconds(250))
    .WithLogging(
        exception =>
        {
            Console.WriteLine($"SCANNER 2: {exception.Message}");
        },
        message =>
        {
            Console.WriteLine($"SCANNER 2: {message}");
        })
    .Build();


var stopwatch = new Stopwatch();
stopwatch.Start();


Console.WriteLine("Starting Scanner 1");
var nodes1 = await scanner1.QueryAddresses();

foreach (var node in nodes1)
{
    Console.WriteLine(node.ToString());
}

Console.WriteLine($"\n\nTOTAL ELAPSED TIME: {stopwatch.Elapsed}");

Console.WriteLine("Starting Scanner 2");
var nodes2 = await scanner2.QueryAddresses();

foreach (var node in nodes2)
{
    Console.WriteLine(node.ToString());
}

Console.WriteLine($"\n\nTOTAL ELAPSED TIME: {stopwatch.Elapsed}");

Console.Read();

List<IPAddress> CreateIpAddresses()
{
    var ipAddresses = new List<IPAddress>(254);

    for (var i = 1; i < 255; i++)
    {
        if (!IPAddress.TryParse($"10.0.0.{i}", out IPAddress? address))
        {
            continue;
        }

        ipAddresses.Add(address);
    }

    return ipAddresses;
}