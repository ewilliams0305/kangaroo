namespace Kangaroo;

public sealed class InvalidTimeoutException : ArgumentOutOfRangeException
{
    public InvalidTimeoutException(TimeSpan timeout)
        : base($"{timeout} is out of range") { }
}