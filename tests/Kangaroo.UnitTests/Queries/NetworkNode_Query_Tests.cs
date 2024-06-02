using System.Net;
using System.Net.NetworkInformation;
using FluentAssertions;
using Kangaroo.Queries;

namespace Kangaroo.UnitTests.Queries;

public class NetworkNode_Query_Tests
{

    [Fact]
    public async Task Query_Returns_MacAddress_WhenIpIsValid()
    {
        // Arrange
        var factory = new NetworkQuerierFactory(
            new DefaultLogger(),
            new QueryPingResultsOrderly(
                new DefaultLogger(), new Ping(), new QueryOptions(1, TimeSpan.FromMilliseconds(250))));

        var query = factory.CreateQuerier();

        // Act
        var result = await query.Query(IPAddress.Loopback);

        // Assert

        result.Alive.Should().BeTrue();
    }
    
    [Fact]
    public async Task Query_Returns_IpStatusFailure_WhenIpIsInvalid()
    {
        // Arrange
        var factory = new NetworkQuerierFactory(
            new DefaultLogger(),
            new QueryPingResultsOrderly(
                new DefaultLogger(), new Ping(), new QueryOptions(1, TimeSpan.FromMilliseconds(250))));

        var query = factory.CreateQuerier();

        // Act
        var result = await query.Query(IPAddress.Parse("192.168.55.237"));

        // Assert

        result.Alive.Should().BeFalse();
    }
}