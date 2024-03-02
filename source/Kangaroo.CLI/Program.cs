using Kangaroo;
using Microsoft.Extensions.Logging;
using System.Net;

var ips = CreateIpAddresses();

using var parallelScanner = ScannerBuilder
    .Configure()
    .WithAddresses(ips)
    .WithParallelism(numberOfBatches: 10)
    .WithNodeTimeout(TimeSpan.FromMilliseconds(250))
    .WithLogging(
        LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }))
    .Build();

Console.WriteLine("Starting Scanner 1");
var nodes1 = await parallelScanner.QueryAddresses();
Console.WriteLine(nodes1.Dump());
Console.WriteLine(nodes1.Dump(true));

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