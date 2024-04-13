namespace Kangaroo;

/// <summary>
/// The options provided to the with options pipeline 
/// </summary>
public sealed class WithOptions
{
    /// <summary>
    /// Number of hops used for the query.
    /// </summary>
    public int Ttl { get; set; } = 10;

    /// <summary>
    /// Timeout for queries, how long before the query should fail
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// enabled web server scans
    /// </summary>
    public bool EnableHttpScan { get; set; } = true;

    /// <summary>
    /// Creates a new client with the provided factory
    /// </summary>
    public Func<HttpClient>? HttpClientFactory { get; set; }
}