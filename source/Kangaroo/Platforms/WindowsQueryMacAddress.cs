using System.Net;
using Kangaroo.Queries;
using Microsoft.Extensions.Logging;

namespace Kangaroo.Platforms;

internal sealed class WindowsQueryMacAddress : IQueryMacAddress
{
    private readonly ILogger _logger;

    public WindowsQueryMacAddress(ILogger logger)
    {
        _logger = logger;
    }
    /// <inheritdoc />
    public async Task<MacAddress> Query(IPAddress ipAddress, CancellationToken token)
    {
        try
        {
            return await Task.Run(() =>
            {
                var macAddr = new byte[6];
                var macAddrLenUlong = (uint)macAddr.Length;

                if (WindowsArp.SendARP(
                        BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0),
                        0, macAddr,
                        ref macAddrLenUlong) == 0)
                {
                    return new MacAddress(macAddr);
                }

                _logger.LogDebug("Failed obtaining the MAC address for {ipAddress}", ipAddress);
                return MacAddress.Empty;

            }, token);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to obtaining the MAC address for node {ipAddress}", ipAddress);
            return MacAddress.Empty;
        }
    }
}