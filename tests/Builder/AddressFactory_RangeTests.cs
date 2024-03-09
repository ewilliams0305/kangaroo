using FluentAssertions;
using System.Net;

namespace Kangaroo.UnitTests.Builder;


public class AddressFactory_RangeTests
{

    [Theory]
    [InlineData("10.0.1.12", "192.168.1.10")]
    [InlineData("172.22.10.12", "192.168.1.254")]
    [InlineData("192.168.1.11", "172.26.55.8")]
    [InlineData("4.4.4.4", "8.8.8.8")]
    public void CreateAddressesFromRange_WhenSubnet_DoesNoteMatch_ThrowsInvalid(string startVal, string endVal)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var end = IPAddress.Parse(endVal);

        // Act

        Assert.Throws<InvalidIpRangeException>(() => AddressFactory.CreateAddressesFromRange(start, end));
    }
    
    [Theory]
    [InlineData("192.168.1.12", "192.168.1.10")]
    [InlineData("192.168.2.12", "192.168.1.254")]
    [InlineData("192.168.1.11", "192.168.1.11")]
    public void CreateAddressesFromRange_WhenEndLessThenStart_ThrowsInvalid(string startVal, string endVal)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var end = IPAddress.Parse(endVal);

        // Act

        Assert.Throws<InvalidIpRangeException>(() => AddressFactory.CreateAddressesFromRange(start, end));
    }
    
    [Theory]
    [InlineData("192.168.1.1", "192.168.1.254")]
    [InlineData("10.0.1.240", "10.0.1.245")]
    [InlineData("172.26.6.32", "172.26.6.37")]
    public void CreateAddressesFromRange_WhenValid24_CreatesValidStart(string startVal, string endVal)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var end = IPAddress.Parse(endVal);

        // Act
        var addresses = AddressFactory.CreateAddressesFromRange(start, end);

        // Assert

        addresses.First().Should().BeEquivalentTo(start);
    }
    
    [Theory]
    [InlineData("192.168.1.1", "192.168.1.254")]
    [InlineData("10.0.1.240", "10.0.1.245")]
    [InlineData("172.26.6.32", "172.26.6.37")]
    public void CreateAddressesFromRange_WhenValid24_CreatesValidEnd(string startVal, string endVal)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var end = IPAddress.Parse(endVal);

        // Act
        var addresses = AddressFactory.CreateAddressesFromRange(start, end);

        // Assert

        addresses.Last().Should().BeEquivalentTo(end);
    }
    
    [Theory]
    [InlineData("192.168.1.1", "192.168.1.254", 254)]
    [InlineData("10.0.1.240", "10.0.1.245", 6)]
    [InlineData("172.26.6.5", "172.26.6.37", 33)]
    public void CreateAddressesFromRange_WhenValid24_CreatesValidQuantity(string startVal, string endVal, int count)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var end = IPAddress.Parse(endVal);

        // Act
        var addresses = AddressFactory.CreateAddressesFromRange(start, end);

        // Assert

        addresses.Count().Should().Be(count);
    }
    
    
    [Theory]
    [InlineData("192.168.1.")]
    [InlineData("10.0.1.")]
    [InlineData("172.26.7.")]
    public void CreateAddressesFromRange_WhenValid24_CreatesValidAddresses(string part)
    {
        // Arrange
        var start = IPAddress.Parse($"{part}1");
        var end = IPAddress.Parse($"{part}254");

        // Act
        var addresses = AddressFactory.CreateAddressesFromRange(start, end);

        // Assert

        var ipAddresses = addresses as IPAddress[] ?? addresses.ToArray();
        for (var i = 1; i == ipAddresses.Count(); i++)
        {
            ipAddresses[i - 1].Should().BeEquivalentTo(IPAddress.Parse($"{part}{i}"));
        }
    }
    
    [Theory]
    [InlineData("192.168.1.1", "192.168.254.254")]
    [InlineData("10.0.1.240", "10.0.7.245")]
    [InlineData("172.26.6.32", "172.26.7.2")]
    public void CreateAddressesFromRange_WhenValid16_CreatesValidStart(string startVal, string endVal)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var end = IPAddress.Parse(endVal);

        // Act
        var addresses = AddressFactory.CreateAddressesFromRange(start, end);

        // Assert

        addresses.First().Should().BeEquivalentTo(start);
    }
    
    [Theory]
    [InlineData("192.168.1.1", "192.168.254.254")]
    [InlineData("10.0.1.240", "10.0.7.245")]
    [InlineData("172.26.6.252", "172.26.7.2")]
    public void CreateAddressesFromRange_WhenValid16_CreatesValidEnd(string startVal, string endVal)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var end = IPAddress.Parse(endVal);

        // Act
        var addresses = AddressFactory.CreateAddressesFromRange(start, end);

        // Assert

        addresses.Last().Should().BeEquivalentTo(end);
    }
    
    [Theory]
    [InlineData("192.168.1.1", "192.168.254.254", 64516)]
    [InlineData("192.168.0.1", "192.168.254.254", 64770)]
    [InlineData("192.168.1.1", "192.168.2.254", 508)]
    [InlineData("192.168.1.1", "192.168.3.254", 762)]
    [InlineData("192.168.1.1", "192.168.4.254", 1016)]
    [InlineData("10.0.1.240", "10.0.7.245", 1530)]
    [InlineData("172.26.6.252", "172.26.7.2", 5)]
    public void CreateAddressesFromRange_WhenValid16_CreatesValidQuantity(string startVal, string endVal, int count)
    {
        // Arrange
        var start = IPAddress.Parse(startVal);
        var end = IPAddress.Parse(endVal);

        // Act
        var addresses = AddressFactory.CreateAddressesFromRange(start, end);

        // Assert

        addresses.Count().Should().Be(count);
    }
    
    
    [Theory]
    [InlineData("192.168.")]
    [InlineData("10.0.")]
    [InlineData("172.26.")]
    public void CreateAddressesFromRange_WhenValid16_CreatesValidAddresses(string part)
    {
        // Arrange
        var start = IPAddress.Parse($"{part}1");
        var end = IPAddress.Parse($"{part}254.254");

        // Act
        var addresses = AddressFactory.CreateAddressesFromRange(start, end);

        // Assert

        var ipAddresses = addresses as IPAddress[] ?? addresses.ToArray();
        for (var i = 1; i == ipAddresses.Count(); i++)
        {
            for (var j = 1; i == ipAddresses.Count(); i++)
            {
                ipAddresses[i - 1].Should().BeEquivalentTo(IPAddress.Parse($"{part}{i}.{j}"));
            }
        } 
    }
}
