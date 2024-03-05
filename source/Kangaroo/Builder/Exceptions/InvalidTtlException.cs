namespace Kangaroo;

public sealed class InvalidTtlException : ArgumentOutOfRangeException
{
    public InvalidTtlException(int ttl)
        : base($"Invalid {ttl} is out of range") { }
}