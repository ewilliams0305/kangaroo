using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;

namespace Kangaroo;

#region STEP 1 ADDRESS CONFIGURATION

/// <summary>
/// The first step in the build pipeline configures the IP addresses used for the scans.
/// <exception cref="InvalidIpRangeException"></exception>
/// <exception cref="InvalidIpAddressException"></exception>
/// <exception cref="InvalidSubnetException"></exception>
/// <exception cref="InvalidNetworkAdapterException"></exception>
/// <exception cref="InvalidTimeoutException"></exception>
/// <exception cref="InvalidTtlException"></exception>
/// </summary>
public interface IScannerIpConfiguration : IScannerSubnet, IScannerRange, IScannerSpecific, IScannerInterface { }

/// <summary>
/// Configures the scanner to use a range of IP addresses.
/// <exception cref="InvalidIpRangeException">Throws if the two IP addresses are not within the subnet boundary</exception>
/// </summary>
public interface IScannerRange
{
    /// <summary>
    /// Configures the scanner to use a range of IP addresses.
    /// <exception cref="InvalidIpRangeException">Throws if the two IP addresses are not within the subnet boundary</exception>
    /// </summary>
    /// <param name="begin">IP Address to start scan from</param>
    /// <param name="end">IP Address to stop scans at</param>
    /// <returns>Next step in the pipeline</returns>
    IScannerTasks WithRange(IPAddress begin, IPAddress end);

    /// <summary>
    /// Configures the scanner to use a range of IP addresses.
    /// <exception cref="InvalidIpRangeException">Throws if the two IP addresses are not within the subnet boundary</exception>
    /// </summary>
    /// <param name="begin">IP Address to start scan from</param>
    /// <param name="end">IP Address to stop scans at</param>
    /// <returns>Next step in the pipeline</returns>
    IScannerTasks WithRange(string begin, string end);
}

/// <summary>
/// Configures the scanner to use the provided subnet.
/// <exception cref="InvalidSubnetException">Throws if the subnet cannot be calculate from the IP address and cider notation.</exception>
/// </summary>
public interface IScannerSubnet
{
    /// <summary>
    /// Configures the scanner to use the provided subnet.
    /// <exception cref="InvalidSubnetException">Throws if the subnet cannot be calculate from the IP address and cider notation.</exception>
    /// </summary>
    /// <param name="address">IP address <example>"192.168.254.0"</example></param>
    /// <param name="subnetMask">The subnet of the ip address <remarks>Cannot be less than \16 255.255.0.0</remarks></param>
    /// <returns>Next step in the pipeline</returns>
    IScannerTasks WithSubnet(IPAddress address, IPAddress subnetMask);

    /// <summary>
    /// Configures the scanner to use the provided subnet.
    /// <exception cref="InvalidSubnetException">Throws if the subnet cannot be calculate from the IP address and cider notation.</exception>
    /// </summary>
    /// <param name="address">IP address <example>"192.168.254.0"</example></param>
    /// <param name="subnetMask">The subnet of the ip address <remarks>Cannot be less than \16 255.255.0.0</remarks></param>
    /// <returns>Next step in the pipeline</returns>
    IScannerTasks WithSubnet(string address, string subnetMask);
}

/// <summary>
/// Configures the scanner to use the subnet attached to a specific adapter.
/// </summary>
public interface IScannerInterface
{
    /// <summary>
    /// Configures the scanner to use the subnet attached to a specific adapter.
    /// </summary>
    /// <returns>Next step in the pipeline</returns>
    IScannerTasks WithInterface(NetworkInterface? @interface = null);
}

/// <summary>
/// Configures the scanner to use the provided IP addresses.
/// </summary>
public interface IScannerSpecific
{
    /// <summary>
    /// Configures the scanner to use the provided IP addresses.
    /// </summary>
    /// <returns>Next step in the pipeline</returns>
    IScannerTasks WithAddresses(IEnumerable<IPAddress> addresses);

    /// <summary>
    /// Creates a new scanner that can only query a single network address.
    /// </summary>
    /// <param name="address">IP Address</param>
    /// <returns>Next step in the pipeline</returns>
    IScannerTasks WithAddress(IPAddress address);
}

#endregion

#region STEP 2 OPTION CONFIGURATION

/// <summary>
/// Configures the scanners optional scan tasks
/// </summary>
public interface IScannerTasks : IScannerWebServer, IScannerOptions { }

/// <summary>
/// Configures the web server scanner
/// </summary>
public interface IScannerWebServer
{
    /// <summary>
    /// Provides an optional http client factory
    /// </summary>
    /// <param name="httpClientFactory">function that returns a client</param>
    /// <returns>options pipeline</returns>
    IScannerOptions WithHttpScan(Func<HttpClient>? httpClientFactory = null);
}

/// <summary>
/// Optional steps
/// </summary>
public interface IScannerOptions : IScannerQueryTimeout, IScannerQueryTtl, IScannerParallelism, IScannerLogging, IScannerBuilder { }
/// <summary>
/// From timeout to...
/// </summary>
public interface IScannerTimeoutNext : IScannerQueryTtl, IScannerParallelism, IScannerLogging, IScannerBuilder { }
/// <summary>
/// from ttl to...
/// </summary>
public interface IScannerTtlNext : IScannerParallelism, IScannerLogging, IScannerBuilder { }
/// <summary>
/// from parallel to...
/// </summary>
public interface IScannerParallelNext : IScannerLogging, IScannerBuilder { }

/// <summary>
/// Overrides the default ping timeout with the provided time.
/// </summary>
public interface IScannerQueryTimeout
{
    /// <summary>
    /// Overrides the default ping timeout with the provided time.
    /// <exception cref="InvalidTimeoutException">The timeout exceeds 20 seconds</exception>
    /// <param name="timeout">timeout time <remarks>Valid ranges are 0 milliseconds = 10 seconds.</remarks></param>
    /// </summary>
    /// <returns>Next step in the pipeline</returns>
    IScannerTimeoutNext WithMaxTimeout(TimeSpan timeout);
}

/// <summary>
/// Optionally provide a max number of network hops.
/// </summary>
public interface IScannerQueryTtl
{
    /// <summary>
    /// Optionally provide a max number of network hops.
    /// </summary>
    /// <exception cref="InvalidTtlException">Time to live is greater than 64</exception>
    /// <param name="ttl">The number of physical hops to scan.</param>
    /// <returns>Next step in the pipeline</returns>
    IScannerTtlNext WithMaxHops(int ttl = 10);
}

#endregion

#region STEP 3 THREADING

/// <summary>
/// Add parallel task execution query each endpoint.
/// When parallelism configures the scanner to query endpoints in parallel.
/// </summary>
public interface IScannerParallelism
{
    /// <summary>
    /// Add parallel task execution query each endpoint.
    /// When parallelism configures the scanner to query endpoints in parallel.
    /// </summary>
    /// <param name="numberOfBatches">The number of batches.  example the default value or 10 would process 254 address in 25 concurrent processes</param>
    /// <returns>the next step in the pipeline</returns>
    IScannerParallelNext WithParallelism(int numberOfBatches = 10);
}

#endregion

#region STEP 4 LOGGING

/// <summary>
/// The logging step in the builder pipeline
/// </summary>
public interface IScannerLoggingOptions : IScannerLogging, IScannerBuilder { }

/// <summary>
/// Logging configuration options
/// </summary>
public interface IScannerLogging
{
    /// <summary>
    /// Provide a logger instance to output logs
    /// </summary>
    /// <param name="logger"></param>
    /// <returns></returns>
    IScannerBuilder WithLogging(ILogger logger);

    /// <summary>
    /// Provide a logger factory to output logs
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <returns></returns>
    IScannerBuilder WithLogging(Func<ILogger> loggerFactory);

    /// <summary>
    /// Provide a logger provider to output logs
    /// </summary>
    /// <param name="loggerProvider"></param>
    /// <returns></returns>
    IScannerBuilder WithLogging(ILoggerProvider loggerProvider);

    /// <summary>
    /// Provide a logger factory to output logs
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <returns></returns>
    IScannerBuilder WithLogging(ILoggerFactory loggerFactory);
}

#endregion

#region BUILD SCANNER

/// <summary>
/// The file step in the pipeline builds a new scanner
/// <exception cref="InvalidIpRangeException"></exception>
/// <exception cref="InvalidIpAddressException"></exception>
/// <exception cref="InvalidSubnetException"></exception>
/// <exception cref="InvalidNetworkAdapterException"></exception>
/// <exception cref="InvalidTimeoutException"></exception>
/// <exception cref="InvalidTtlException"></exception>
/// </summary>
public interface IScannerBuilder
{
    /// <summary>
    /// Returns the configured instance of an IScanner
    /// </summary>
    /// <returns>the new scanner</returns>
    IScanner Build();
}

#endregion

