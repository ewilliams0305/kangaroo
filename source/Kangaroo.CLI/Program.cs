using System.Diagnostics;
using System.Net;
using Kangaroo;

var ips = CreateIpAddresses();


using var parallelScanner = ScannerBuilder
    .Configure()
    .WithAddresses(ips)
    .WithParallelism(numberOfBatches: 120)
    .WithNodeTimeout(TimeSpan.FromMilliseconds(250))
    //.WithLogging()
    .Build();

//using var orderlyScanner = ScannerBuilder
//    .Configure()
//    .WithAddresses(ips)
//    .WithNodeTimeout(TimeSpan.FromMilliseconds(250))
//    //.WithLogging()
//    .Build();



Console.WriteLine("Starting Scanner 1");
var nodes1 = await parallelScanner.QueryAddresses();

foreach (var node in nodes1.Nodes)
{
    Console.WriteLine(node.ToString());
}

Console.WriteLine($"\n\nTOTAL ELAPSED TIME: {nodes1.ElapsedTime}");

//Console.WriteLine("Starting Scanner 2");
//var nodes2 = await orderlyScanner.QueryAddresses();

//foreach (var node in nodes2)
//{
//    Console.WriteLine(node.ToString());
//}

//Console.WriteLine($"\n\nTOTAL ELAPSED TIME: {stopwatch.Elapsed}");

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