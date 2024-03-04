using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;

namespace Kangaroo;

#region STEP 1 ADDRESS CONFIGURATION

/// <summary>
/// The first step in the build pipeline configures the IP addresses used for the scans.
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
    IScannerOptions WithRange(IPAddress begin, IPAddress end);

    /// <summary>
    /// Configures the scanner to use a range of IP addresses.
    /// <exception cref="InvalidIpRangeException">Throws if the two IP addresses are not within the subnet boundary</exception>
    /// </summary>
    /// <param name="begin">IP Address to start scan from</param>
    /// <param name="end">IP Address to stop scans at</param>
    /// <returns>Next step in the pipeline</returns>
    IScannerOptions WithRange(string begin, string end);
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
    IScannerOptions WithSubnet(IPAddress address, IPAddress subnetMask);

    /// <summary>
    /// Configures the scanner to use the provided subnet.
    /// <exception cref="InvalidSubnetException">Throws if the subnet cannot be calculate from the IP address and cider notation.</exception>
    /// </summary>
    /// <param name="address">IP address <example>"192.168.254.0"</example></param>
    /// <param name="subnetMask">The subnet of the ip address <remarks>Cannot be less than \16 255.255.0.0</remarks></param>
    /// <returns>Next step in the pipeline</returns>
    IScannerOptions WithSubnet(string address, string subnetMask);
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
    IScannerOptions WithInterface(NetworkInterface? @interface = null);
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
    IScannerOptions WithAddresses(IEnumerable<IPAddress> addresses);
}

#endregion

#region STEP 2 OPTION CONFIGURATION

public interface IScannerOptions : IScannerQueryTimeout, IScannerQueryTtl, IScannerParallelism, IScannerLogging, IScannerBuilder { }
public interface IScannerTimeoutNext : IScannerQueryTtl, IScannerParallelism, IScannerLogging, IScannerBuilder { }
public interface IScannerTtlNext : IScannerParallelism, IScannerLogging, IScannerBuilder { }
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

public interface IScannerLoggingOptions : IScannerLogging, IScannerBuilder { }

public interface IScannerLogging
{
    IScannerBuilder WithLogging(ILogger logger);
    IScannerBuilder WithLogging(Func<ILogger> loggerFactory);
    IScannerBuilder WithLogging(ILoggerProvider loggerProvider);
    IScannerBuilder WithLogging(ILoggerFactory loggerFactory);
}

#endregion

#region BUILD SCANNER

public interface IScannerBuilder
{
    IScanner Build();
}

#endregion




public interface IScannerLoggingParallel : IScannerLogging, IScannerParallelism
{
}




public interface IScannerQueryOptions : IScannerQueryTimeout, IScannerQueryTtl, IScannerParallelismOptions
{

}


public interface IScannerParallelismOptions : IScannerParallelism, IScannerLoggingOptions
{

}








