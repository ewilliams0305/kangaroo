using System.Net;

namespace Kangaroo.Queries
{
    internal interface IMacAddressLocator
    {
        Task<MacAddress> GetMacAddressAsync(IPAddress ipAddress, CancellationToken token);
    }
}
