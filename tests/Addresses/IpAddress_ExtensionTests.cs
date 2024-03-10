using System.Net;

namespace Kangaroo.UnitTests.Addresses;
// ReSharper disable once InconsistentNaming

public class IpAddress_ExtensionTests
{
    [Fact]
    public void IpAddress_Throws_WhenAny()
    {
        // Arrange
        var ip = IPAddress.Parse("0.0.0.0");

        // Act

        // Assert

        Assert.Throws<InvalidIpAddressException>(() => ip.ThrowIfAddressIsNotEndpoint());
    }
    
    [Theory]
    [InlineData("192.168.254.255")]
    [InlineData("255.255.255.255")]
    [InlineData("10.1.255.255")]
    [InlineData("172.26.6.255")]
    [InlineData("255.26.6.2")]
    [InlineData("172.26.255.2")]
    [InlineData("172.255.6.2")]
    public void IpAddress_Throws_WhenBroadcast(string ipValue)
    {
        // Arrange
        var ip = IPAddress.Parse(ipValue);

        // Act

        // Assert

        Assert.Throws<InvalidIpAddressException>(() => ip.ThrowIfAddressIsNotEndpoint());
    }
    
    [Fact]
    public void IpAddress_Throws_WhenLoopback()
    {
        // Arrange
        var ip = IPAddress.Parse("127.0.0.1");

        // Act

        // Assert

        Assert.Throws<InvalidIpAddressException>(() => ip.ThrowIfAddressIsNotEndpoint());
    }

}
