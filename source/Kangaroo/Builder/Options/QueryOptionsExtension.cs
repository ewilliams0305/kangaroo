using System.Net.NetworkInformation;

namespace Kangaroo;

internal static class QueryOptionsExtension
{
    public static PingOptions ToPingOptions(this QueryOptions options) =>
        new (options.Ttl, true);

    public static QueryOptions ToQueryOptions(this PingOptions options, TimeSpan timeout) =>
        new (options.Ttl, timeout);

    public static int ToTimeout(this QueryOptions options) =>
        (int)options.Timeout.TotalMilliseconds;
}