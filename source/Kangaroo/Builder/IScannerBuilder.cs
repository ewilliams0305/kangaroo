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
    IScannerBuilder WithLogging(Func<ILogger> loggerFactory);
    IScannerBuilder WithLogging(ILoggerProvider loggerProvider);
    IScannerBuilder WithLogging(ILoggerFactory loggerFactory);


}

public interface IScannerParallelism
{
    /// <summary>
    /// Add parallel task execution query each endpoint.
    /// When parallelism configures the scanner to query endpoints in parallel.
    /// </summary>
    /// <param name="numberOfBatches">The number of batches.  example the default value or 10 would process 254 address in 25 concurrent processes</param>
    /// <returns>the next step in the pipeline</returns>
    IScannerTimeoutOptions WithParallelism(int numberOfBatches = 10);
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