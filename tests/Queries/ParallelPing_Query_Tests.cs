using System.Net;
using System.Net.NetworkInformation;
using FluentAssertions;
using Kangaroo.Queries;

namespace Kangaroo.UnitTests.Queries;

// ReSharper disable once InconsistentNaming
public class ParallelPing_Query_Tests
{

    [Fact]
    public async Task Query_Returns_IpStatusSuccess_WhenIpIsValid()
    {
        // Arrange
        using var pingQuery = new QueryPingResultsParallel(new DefaultLogger(), new QueryOptions(1, TimeSpan.FromSeconds(1)));

        // Act
        var ping = await pingQuery.Query(IPAddress.Loopback);

        // Assert

        ping!.Status.Should().Be(IPStatus.Success);
    }
    
    [Fact]
    public async Task Query_Returns_IpStatusFailure_WhenIpIsInvalid()
    {
        // Arrange
        using var pingQuery = new QueryPingResultsParallel(new DefaultLogger(), new QueryOptions(1, TimeSpan.FromSeconds(1)));

        // Act
        var ping = await pingQuery.Query(IPAddress.Parse("192.168.55.237"));

        // Assert

        ping!.Status.Should().NotBe(IPStatus.Success);
    }

}