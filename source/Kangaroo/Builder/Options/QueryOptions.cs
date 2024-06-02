using System.Security.AccessControl;

namespace Kangaroo;

/// <summary>
/// Options used for network queries
/// </summary>
/// <param name="Ttl">Number of hops used for the query.</param>
/// <param name="Timeout">Timeout for queries, how long before the query should fail</param>
internal sealed record QueryOptions(int Ttl, TimeSpan Timeout)
{
    /// <summary>
    /// Creates a default value option
    /// </summary>
    public static QueryOptions Default => new(64, TimeSpan.FromMilliseconds(3000));

    /// <summary>
    /// Returns the timeout value from the options
    /// </summary>
    /// <param name="options">options</param>
    public static implicit operator TimeSpan(QueryOptions options) => options.Timeout;

    /// <summary>
    /// Returns the time to live value from the options
    /// </summary>
    /// <param name="options">options</param>
    public static implicit operator int(QueryOptions options) => options.Ttl;
}