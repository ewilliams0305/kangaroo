namespace Kangaroo;

/// <summary>
/// Ping timeout is too long or too short
/// </summary>
public sealed class InvalidTimeoutException : ArgumentOutOfRangeException
{
    /// <summary>
    /// Creates a new timeout exception with the invalid value
    /// </summary>
    /// <param name="timeout">The invalid value</param>
    public InvalidTimeoutException(TimeSpan timeout)
        : base($"Invalid {timeout} is out of range") { }
}