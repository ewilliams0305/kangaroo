using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace Kangaroo.Queries;

internal sealed class QueryHostname : IQueryHostname
{
    private readonly ILogger _logger;

    public QueryHostname(ILogger logger)
    {
        _logger = logger;
    }
    #region Implementation of IQueryHostname

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

    #endregion
}