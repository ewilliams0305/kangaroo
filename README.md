# kangaroo Network Scanner
![GitHub](https://img.shields.io/github/license/ewilliams0305/kangaroo) 
![GitHub all releases](https://img.shields.io/github/downloads/ewilliams0305/kangaroo/total) 
![Nuget](https://img.shields.io/nuget/dt/kangaroo)
[![GitHub issues](https://img.shields.io/github/issues/ewilliams0305/kangaroo)](https://github.com/ewilliams0305/kangaroo/issues)
![GitHub Repo stars](https://img.shields.io/github/stars/ewilliams0305/kangaroo?style=social)
![GitHub forks](https://img.shields.io/github/forks/ewilliams0305/kangaroo?style=social)

*Kangaroos have large, powerful hind legs, large feet adapted for leaping* and so does the Kangaroo network scanner. 

![Readme Image](./IMG_2728.jpeg)

The kangaroo network scanner supports (or will support) the following features. 

![Static Badge](https://img.shields.io/badge/IP-SCAN-blue)  
![Static Badge](https://img.shields.io/badge/PORT-SCAN-green)
![Static Badge](https://img.shields.io/badge/NODE-SCAN-blue)   
![Static Badge](https://img.shields.io/badge/PARELLEL-SCAN-blue)   

## Table of Contents
1. [Building Scanners](#Building)
2. [Scanning Networks](#Scanning-Networks)

# Building
Kangaroo leverages the builder patter to ensure its always configured correctly before usage. 

``` csharp
using var scanner = ScannerBuilder
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

var nodes = await scanner.QueryAddresses();
Console.WriteLine(nodes.Dump());
```

## IP Configuration

## Parellel Configuration

## Ping Configuration

## Logging Configuration


# Scanning Networks

## IP Scanning

## Port Scanner

## Nodes

## Scan Results


