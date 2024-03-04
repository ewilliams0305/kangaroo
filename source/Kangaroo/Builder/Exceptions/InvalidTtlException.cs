namespace Kangaroo;

public sealed class InvalidTtlException : ArgumentOutOfRangeException
{
    public InvalidTtlException(int ttl)
        : base($"{ttl} is out of range") { }
}