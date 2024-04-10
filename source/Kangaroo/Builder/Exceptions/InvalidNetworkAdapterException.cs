using System.Net.NetworkInformation;

namespace Kangaroo;

/// <summary>
/// Exception thrown when an adapter is determined to be invalid
/// </summary>
public sealed class InvalidNetworkAdapterException : Exception
{
    /// <summary>
    /// Creates a new invalid adapter exception.
    /// </summary>
    /// <param name="interface">The interface that was invalid</param>
    /// <param name="reason"></param>
    public InvalidNetworkAdapterException(NetworkInterface @interface, AdapterFailureCode reason)
        : base( reason == AdapterFailureCode.LinkDown 
            ? $"Network Adapter {@interface.Name} Link is Down ⬇"
            : $"Network Adapter {@interface.Name} Has no valid IP Address") { }

    /// <summary>
    /// Creates a new invalid adapter exception.
    /// </summary>
    /// <param name="interface">The interface that was invalid</param>
    /// <param name="reason"></param>
    public InvalidNetworkAdapterException(string @interface, AdapterFailureCode reason)
        : base( reason == AdapterFailureCode.LinkDown 
            ? $"Network Adapter {@interface} Link is Down ⬇"
            : $"Network Adapter {@interface} Has no valid IP Address") { }
}

/// <summary>
/// Reason the network adapter couldn't be used for a scan
/// </summary>
public enum AdapterFailureCode
{
    /// <summary>
    /// The adapter was not found
    /// </summary>
    Unknown,
    /// <summary>
    /// The link is down
    /// </summary>
    LinkDown,
    /// <summary>
    /// The adapter has no IP address
    /// </summary>
    NoIpAddress
}