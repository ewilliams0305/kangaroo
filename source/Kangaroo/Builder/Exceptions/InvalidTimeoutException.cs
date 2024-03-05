namespace Kangaroo;

public sealed class InvalidTimeoutException : ArgumentOutOfRangeException
{
    public InvalidTimeoutException(TimeSpan timeout)
        : base($"Invalid {timeout} is out of range") { }
}