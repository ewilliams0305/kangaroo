using System.Net;

namespace Kangaroo.Queries;

internal sealed class QueryWebServer: IQueryWebServer
{
    private readonly Func<HttpClient> _clientFactory;

    public QueryWebServer(Func<HttpClient> clientFactory)
    {
        _clientFactory = clientFactory;
    }

    /// <inheritdoc />
    public async Task<string> Query(IPAddress ipAddress, CancellationToken token = default)
    {
        try
        {
            using var client = _clientFactory.Invoke();

            client.BaseAddress = new Uri($"http://{ipAddress}");
            client.Timeout = TimeSpan.FromMilliseconds(500);

            var response = await client.GetAsync("/", token);
            return response.Headers.Server.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return string.Empty;
        }
    }
}