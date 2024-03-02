using System.Net;
using System.Net.NetworkInformation;

namespace Kangaroo;

public interface IScannerIpConfiguration : IScannerSubnet, IScannerRange, IScannerSpecific, IScannerInterface
{

}

public interface IScannerOptions : IScannerTimeoutOptions, IScannerConcurrentOptions
{

}

public interface IScannerLoggingOptions : IScannerLogging, IScannerBuilder
{

}

public interface IScannerTimeoutOptions : IScannerTimeout, IScannerLoggingOptions
{

}

public interface IScannerConcurrentOptions : IScannerConcurrent, IScannerLoggingOptions
{

}

public interface IScannerLogging
{
    IScannerBuilder WithLogging(Action<Exception>? exception = null, Action<string>? message = null);
}

public interface IScannerConcurrent
{
    IScannerTimeoutOptions WithConcurrency(bool concurrent = false);
}

public interface IScannerTimeout
{
    IScannerConcurrentOptions WithNodeTimeout(TimeSpan timeout);

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