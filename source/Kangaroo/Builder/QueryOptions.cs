namespace Kangaroo;

/// <summary>
/// Options used for network queries
/// </summary>
/// <param name="Ttl">Number of hops used for the query.</param>
/// <param name="Timeout">Timeout for queries, how long before the query should fail</param>
internal sealed record QueryOptions(int Ttl, TimeSpan Timeout)
{
    public static QueryOptions Default => new(64, TimeSpan.FromMilliseconds(3000));
};