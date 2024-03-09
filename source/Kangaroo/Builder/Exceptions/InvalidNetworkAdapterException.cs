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
    public InvalidNetworkAdapterException(NetworkInterface @interface)
        : base($"Invalid network interface {@interface.Name}") { }
}