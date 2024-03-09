using System.Net;

namespace Kangaroo;

/// <summary>
/// Exception thrown when an IP range is invalid
/// </summary>
public sealed class InvalidIpRangeException : Exception
{
    /// <summary>
    /// Creates a new IP range exception
    /// </summary>
    /// <param name="start">The value that was used to create the range resulting in an exception</param>
    /// <param name="end">The value that was used to create the range resulting in an exception</param>
    public InvalidIpRangeException(IPAddress start, IPAddress end)
        : base($"Invalid IP address range {start} - {end}") { }

    /// <summary>
    /// Creates a new IP range exception
    /// </summary>
    /// <param name="start">The value that was used to create the range resulting in an exception</param>
    /// <param name="end">The value that was used to create the range resulting in an exception</param>
    public InvalidIpRangeException(byte[] start, byte[] end)
        : base($"Invalid IP address range {start} - {end}") { }
}