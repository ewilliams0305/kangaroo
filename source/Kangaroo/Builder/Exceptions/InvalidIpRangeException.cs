using System.Net;

namespace Kangaroo;

public sealed class InvalidIpRangeException : Exception
{
    public InvalidIpRangeException(IPAddress start, IPAddress end)
        : base($"{start} - {end}") { }
}