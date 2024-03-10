using Microsoft.Extensions.Logging;
using System.Net;

namespace Kangaroo.Queries;

internal sealed class QueryWebServer: IQueryWebServer
{
    private readonly ILogger _logger;
    private readonly Func<HttpClient> _clientFactory;

    public QueryWebServer(ILogger logger, Func<HttpClient> clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    /// <inheritdoc />
    public async Task<string> Query(IPAddress ipAddress, CancellationToken token = default)
    {
        try
        {
            using var client = _clientFactory.Invoke();
            
            client.BaseAddress = new Uri($"http://{ipAddress}");
            client.Timeout = TimeSpan.FromMilliseconds(1000);

            var response = await client.GetAsync("/", token);
            return response.Headers.Server.ToString();
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("{IpAddress} is not hosting a web server", ipAddress);
        }
        catch (HttpRequestException requestException) when(requestException.Message.Contains("Connection refused"))
        {
            _logger.LogInformation("{IpAddress} is not hosting a web server", ipAddress);
        }
        catch (HttpRequestException requestException) when (requestException.Message.Contains("The SSL connection could not be established"))
        {
            _logger.LogInformation("{IpAddress} is not hosting a web server", ipAddress);
        }
        catch (TimeoutException)
        {
            _logger.LogInformation("{IpAddress} is not hosting a web server", ipAddress);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{IpAddress} failed to query web server", ipAddress);
        }
        
        return string.Empty;
    }
}