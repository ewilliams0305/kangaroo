using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace Kangaroo.Queries;

internal sealed class QueryHostname : IQueryHostname
{
    private readonly ILogger _logger;

    internal QueryHostname(ILogger logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IPHostEntry?> Query(IPAddress ipAddress, CancellationToken token = default)
    {
        try
        {
            var ipHostEntry = await Dns.GetHostEntryAsync(ipAddress.ToString(), AddressFamily.NetBios , token);
            return ipHostEntry;
        }
        catch (ArgumentException argumentException)
        {
            _logger.LogCritical(argumentException, "Failed obtaining the DNS name {ipAddress}", ipAddress);
        }
        catch (SocketException socketError) when (socketError.NativeErrorCode == 11001)
        {
            _logger.LogInformation("Failed obtaining the DNS name {ipAddress} reason {message}", ipAddress, socketError.Message);
        }
        
        catch (SocketException socketError) when (socketError.Message.Contains("Name or service not known"))
        {
            _logger.LogInformation("Failed obtaining the DNS name {ipAddress} reason {message}", ipAddress, socketError.Message);
        }
        catch (SocketException socketError)
        {
            _logger.LogCritical(socketError, "Failed obtaining the DNS name {ipAddress}", ipAddress);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed obtaining the DNS name {ipAddress}", ipAddress);
        }

        return null;
    }
}