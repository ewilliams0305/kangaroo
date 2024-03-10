using System.Net;
using System.Text;
using FluentAssertions;

namespace Kangaroo.UnitTests.Addresses;

// ReSharper disable once InconsistentNaming
public class AddressFactory_SubnetTests
{
    [Theory]
    [InlineData("255.255.255.0", "255.255.255.0")]
    [InlineData("0.0.0.0", "255.255.255.0")]
    [InlineData("127.0.0.1", "255.255.255.0")]
    public void CreateAddressesFromSubnet_IpIsNotEndpoint_ThrowsInvalid(string startVal, string subnet)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var mask = IPAddress.Parse(subnet);

        // Act

        Assert.Throws<InvalidIpAddressException>(() => AddressFactory.CreateAddressesFromSubnet(start, mask));
    }
    
    [Theory]
    [InlineData("192.168.1.1", "255.0.255.0")]
    [InlineData("192.168.1.1", "255.248.0.0")]
    [InlineData("192.168.1.1", "0.0.0.0")]
    public void CreateAddressesFromSubnet_SubnetIsLessThan16_ThrowsInvalid(string startVal, string subnet)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var mask = IPAddress.Parse(subnet);

        // Act

        Assert.Throws<InvalidIpAddressException>(() => AddressFactory.CreateAddressesFromSubnet(start, mask));
    }

    [Theory]
    [InlineData("192.168.1.10", "255.255.255.0", "192.168.1.1")]
    [InlineData("10.0.0.17", "255.255.255.0", "10.0.0.1")]
    [InlineData("172.26.6.32", "255.255.255.0", "172.26.6.1")]
    [InlineData("172.26.6.12", "255.255.255.128", "172.26.6.1")]
    [InlineData("10.0.17.230", "255.255.255.240", "10.0.17.225")]
    public void CreateAddressesFromRange_WhenValid24_CreatesValidStart(string startVal, string subnet, string excepted)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var mask = IPAddress.Parse(subnet);

        // Act
        var addresses = AddressFactory.CreateAddressesFromSubnet(start, mask);

        // Assert

        addresses.First().Should().BeEquivalentTo(IPAddress.Parse(excepted));
    }
    
    [Theory]
    [InlineData("192.168.1.10", "255.255.255.0", "192.168.1.254")]
    [InlineData("10.0.0.17", "255.255.255.0", "10.0.0.254")]
    [InlineData("172.26.6.32", "255.255.255.0", "172.26.6.254")]
    [InlineData("172.26.6.12", "255.255.255.128", "172.26.6.126")]
    [InlineData("10.0.17.230", "255.255.255.240", "10.0.17.238")]
    public void CreateAddressesFromRange_WhenValid24_CreatesValidEnd(string startVal, string subnet, string expected)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var mask = IPAddress.Parse(subnet);

        // Act
        var addresses = AddressFactory.CreateAddressesFromSubnet(start, mask);

        // Assert

        addresses.Last().Should().BeEquivalentTo(IPAddress.Parse(expected));
    }

    [Theory]
    [InlineData("192.168.1.10", "255.255.255.0", 254)]
    [InlineData("10.0.0.17", "255.255.255.0", 254)]
    [InlineData("172.26.6.32", "255.255.255.0", 254)]
    [InlineData("172.26.6.200", "255.255.255.224", 30)]
    public void CreateAddressesFromSubnet_WhenValid24_CreatesValidQuantity(string startVal, string endVal, int quantity)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var end = IPAddress.Parse(endVal);

        // Act
        var addresses = AddressFactory.CreateAddressesFromSubnet(start, end);

        // Assert

        addresses.Count().Should().Be(quantity);
    }


    [Theory]
    [InlineData("192.168.1.")]
    [InlineData("10.0.0.")]
    [InlineData("172.26.6.")]
    public void CreateAddressesFromSubnet_WhenValid24_CreatesValidAddresses(string part)
    {
        // Arrange
        var start = IPAddress.Parse($"{part}1");
        var mask = IPAddress.Parse("255.255.255.0");

        // Act
        var addresses = AddressFactory.CreateAddressesFromSubnet(start, mask);

        // Assert

        var ipAddresses = addresses as IPAddress[] ?? addresses.ToArray();
        for (var i = 1; i == ipAddresses.Length; i++)
        {
            ipAddresses[i - 1].Should().BeEquivalentTo(IPAddress.Parse($"{part}{i}"));
        }
    }
    
    [Theory]
    [InlineData("192.168.1.10", "255.255.248.0", "192.168.0.1")]
    [InlineData("10.0.0.17", "255.255.248.0", "10.0.0.1")]
    [InlineData("172.26.6.32", "255.255.240.0", "172.26.0.1")]
    [InlineData("172.26.6.32", "255.255.0.0", "172.26.0.1")]
    public void CreateAddressesFromRange_WhenValid16_CreatesValidStart(string startVal, string subnet, string expected)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var mask = IPAddress.Parse(subnet);

        // Act
        var addresses = AddressFactory.CreateAddressesFromSubnet(start, mask);

        // Assert

        addresses.First().Should().BeEquivalentTo(IPAddress.Parse(expected));
    }

    [Theory]
    [InlineData("192.168.1.10", "255.255.248.0", "192.168.7.254")]
    [InlineData("10.0.0.17", "255.255.248.0", "10.0.7.254")]
    [InlineData("172.26.6.32", "255.255.240.0", "172.26.15.254")]
    [InlineData("172.26.6.32", "255.255.0.0", "172.26.255.254")]
    public void CreateAddressesFromRange_WhenValid16_CreatesValidEnd(string startVal, string subnet, string expected)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var mask = IPAddress.Parse(subnet);

        // Act
        var addresses = AddressFactory.CreateAddressesFromSubnet(start, mask);

        // Assert

        addresses.Last().Should().BeEquivalentTo(IPAddress.Parse(expected));
    }

    [Theory]
    [InlineData("192.168.1.10", "255.255.248.0", 2032)]
    public void CreateAddressesFromSubnet_WhenValid16_CreatesValidQuantity(string startVal, string maskVal, int quantity)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var mask = IPAddress.Parse(maskVal);

        // Act
        var addresses = AddressFactory.CreateAddressesFromSubnet(start, mask);

        // Assert
        addresses.Count().Should().Be(quantity);
    }

}