using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;

namespace Kangaroo;

public interface IScannerIpConfiguration : IScannerSubnet, IScannerRange, IScannerSpecific, IScannerInterface
{

}

public interface IScannerOptions : IScannerTimeoutOptions, IScannerParallelismOptions
{

}

public interface IScannerLoggingOptions : IScannerLogging, IScannerBuilder
{

}

public interface IScannerTimeoutOptions : IScannerTimeout, IScannerLoggingOptions
{

}

public interface IScannerParallelismOptions : IScannerParallelism, IScannerLoggingOptions
{

}

public interface IScannerLogging
{
    IScannerBuilder WithLogging(ILogger logger);
    //IScannerBuilder WithLogging(Action<Exception>? exception = null, Action<string>? message = null);
}

public interface IScannerParallelism
{
    /// <summary>
    /// Add parallel task execution query each endpoint.
    /// When parallelism configures the scanner to query endpoints in parallel.
    /// </summary>
    /// <param name="ipAddressPerBatch">Max number of async Task used to query the provided IPs.</param>
    /// <returns>the next step in the pipeline</returns>
    IScannerTimeoutOptions WithParallelism(int ipAddressPerBatch = 10);
}

public interface IScannerTimeout
{
    IScannerParallelismOptions WithNodeTimeout(TimeSpan timeout);

}

public interface IScannerRange
{
    IScannerOptions WithRange(IPAddress begin, IPAddress end);
}

public interface IScannerSubnet
{
    IScannerOptions WithSubnet(byte subnet, IPAddress? address = null);
}

public interface IScannerInterface
{
    IScannerOptions WithInterface(NetworkInterface @interface);
}

public interface IScannerSpecific
{
    IScannerOptions WithAddresses(IEnumerable<IPAddress> addresses);
}


public interface IScannerBuilder
{
    IScanner Build();
}