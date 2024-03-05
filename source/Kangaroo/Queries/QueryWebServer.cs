using System.Net;

namespace Kangaroo.Queries;

internal sealed class QueryWebServer: IQueryWebServer
{
    private readonly HttpClient _client;

    public QueryWebServer(HttpClient? client = null)
    {
        _client = client ?? new HttpClient();
    }

    /// <inheritdoc />
    public async Task<string> Query(IPAddress ipAddress, CancellationToken token = default)
    {
        try
        {
            _client.BaseAddress = new Uri($"http://{ipAddress}");
            _client.Timeout = TimeSpan.FromMilliseconds(500);

            var response = await _client.GetAsync("/", token);
            return response.Headers.Server.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return string.Empty;
        }
        
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _client.Dispose();
    }

}