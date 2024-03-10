using FluentAssertions;
using Kangaroo.Queries;
using System.Net;
using System.Net.NetworkInformation;

namespace Kangaroo.UnitTests.Queries;
// ReSharper disable once InconsistentNaming

public class OrderlyPing_Query_Tests
{

    [Fact]
    public async Task Query_Returns_IpStatusSuccess_WhenIpIsValid()
    {
        // Arrange
        using var pingQuery = new QueryPingResultsOrderly(new DefaultLogger(), new Ping(),
            new QueryOptions(1, TimeSpan.FromSeconds(1)));

        // Act
        var ping = await pingQuery.Query(IPAddress.Loopback);

        // Assert

        ping!.Status.Should().Be(IPStatus.Success);
    }
    
    [Fact]
    public async Task Query_Returns_IpStatusFailure_WhenIpIsInvalid()
    {
        // Arrange
        using var pingQuery = new QueryPingResultsOrderly(new DefaultLogger(), new Ping(),
            new QueryOptions(1, TimeSpan.FromSeconds(1)));

        // Act
        var ping = await pingQuery.Query(IPAddress.Parse("192.168.55.237"));

        // Assert

        ping!.Status.Should().NotBe(IPStatus.Success);
    }

}