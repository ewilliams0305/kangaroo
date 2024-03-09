namespace Kangaroo;

/// <summary>
/// Ttl is out of range
/// </summary>
public sealed class InvalidTtlException : ArgumentOutOfRangeException
{
    /// <summary>
    /// creates a new exception when the TTL value is invalid
    /// </summary>
    /// <param name="ttl">Invalid value</param>
    public InvalidTtlException(int ttl)
        : base($"Invalid {ttl} is out of range") { }
}