using System.Net;
using Kangaroo.Queries;
using Microsoft.Extensions.Logging;

namespace Kangaroo.Platforms;

internal sealed class LinuxQueryMacAddress : IQueryMacAddress
{
    private readonly ILogger _logger;

    public LinuxQueryMacAddress(ILogger logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<MacAddress> Query(IPAddress ipAddress, CancellationToken token)
    {
        try
        {
            var arpResult = await LinuxCommand.RunCommandAsync($"arp -n {ipAddress}", token);

            var lines = arpResult.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
            {
                _logger.LogDebug("Failed obtaining the MAC address for {ipAddress}", ipAddress);
                return MacAddress.Empty;
            }

            var parts = lines[1].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            return parts.Length >= 3
                ? new MacAddress(parts[2])
                : MacAddress.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to obtaining the MAC address for node {ipAddress}", ipAddress);
            return MacAddress.Empty;
        }
    }
}