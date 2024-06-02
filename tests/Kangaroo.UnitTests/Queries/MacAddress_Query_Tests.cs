using System.Net;
using System.Runtime.InteropServices;
using FluentAssertions;
using Kangaroo.Platforms;
using Kangaroo.Queries;

namespace Kangaroo.UnitTests.Queries;

// ReSharper disable once InconsistentNaming
public class MacAddress_Query_Tests
{

    [Fact]
    public async Task Query_Returns_MacAddress_WhenIpIsValid()
    {
        // Arrange
        IQueryMacAddress query;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            query = new LinuxQueryMacAddress(new DefaultLogger());
        }
        else
        {
            query = new WindowsQueryMacAddress(new DefaultLogger());
        }

        // Act
        var result = await query.Query(IPAddress.Loopback, CancellationToken.None);

        // Assert

        result.Should().BeEquivalentTo(MacAddress.Empty);
    }
    
    [Fact]
    public async Task Query_Returns_IpStatusFailure_WhenIpIsInvalid()
    {
        // Arrange
        IQueryMacAddress query;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            query = new LinuxQueryMacAddress(new DefaultLogger());
        }
        else
        {
            query = new WindowsQueryMacAddress(new DefaultLogger());
        }

        // Act
        var result = await query.Query(IPAddress.Parse("192.168.55.237"), CancellationToken.None);

        // Assert

        result.Should().BeEquivalentTo(MacAddress.Empty);
    }
}