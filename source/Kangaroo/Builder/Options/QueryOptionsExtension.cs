using System.Net.NetworkInformation;

namespace Kangaroo;

/// <summary>
/// Extension methods to convert options.
/// </summary>
internal static class QueryOptionsExtension
{
    /// <summary>
    /// Converts a query options to ping options.
    /// </summary>
    /// <param name="options">the options to convert</param>
    /// <returns>new ping options.</returns>
    public static PingOptions ToPingOptions(this QueryOptions options) =>
        new (options.Ttl, true);

    /// <summary>
    /// Converts ping options to a new query option
    /// </summary>
    /// <param name="options">the options to convert</param>
    /// <param name="timeout">the timeout</param>
    /// <returns>the new query options.</returns>
    public static QueryOptions ToQueryOptions(this PingOptions options, TimeSpan timeout) =>
        new (options.Ttl, timeout);

    /// <summary>
    /// Extracts the timeout from the options.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static int ToTimeout(this QueryOptions options) =>
        (int)options.Timeout.TotalMilliseconds;
}