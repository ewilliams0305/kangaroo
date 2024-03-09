using FluentAssertions;

namespace Kangaroo.UnitTests.Model;

public class MacAddressTests
{

    [Theory]
    [InlineData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 })]
    [InlineData(new byte[] { 0xAA, 0x03, 0xFF, 0x01, 0x55, 0xCC })]
    [InlineData(new byte[] { 0xFF, 0x02, 0x03, 0x04, 0x05, 0xCA })]
    [InlineData(new byte[] { 0x10, 0x02, 0x03, 0x04, 0xBB, 0x06 })]
    [InlineData(new byte[] { 0x20, 0x02, 0x04, 0x04, 0x05, 0xAB })]
    public void MacAddress_Constructor_CreatesValidMac_FromBytes(byte[] bytes)
    {

        // Arrange
        var macAddress = new MacAddress(bytes);

        // Act

        // Assert
        macAddress.FirstByte.Equals(bytes[0]).Should().BeTrue();
        macAddress.SecondByte.Equals(bytes[1]).Should().BeTrue();
        macAddress.ThirdByte.Equals(bytes[2]).Should().BeTrue();
        macAddress.ForthByte.Equals(bytes[3]).Should().BeTrue();
        macAddress.FifthByte.Equals(bytes[4]).Should().BeTrue();
        macAddress.SixthByte.Equals(bytes[5]).Should().BeTrue();
    }

    [Fact]
    public void MacAddress_Constructor_WithBytes_ThrowNullException_ForNullInput()
    {
        Assert.Throws<ArgumentNullException>(() => new MacAddress(bytes: null!));
    } 
    
    [Theory]
    [InlineData(new byte[] { 1, 2, 3, 4, 5 })]
    [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7 })]
    public void MacAddress_Constructor_WithBytes_ThrowRangeException_ForInvalidLength(byte[] bytes)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MacAddress(bytes));
    }


    [Theory]
    [InlineData("01:02:03:04:05:06")]
    [InlineData("AA:03:FF:01:55:CC")]
    [InlineData("FF:02:03:04:05:CA")]
    [InlineData("10:02:03:04:BB:06")]
    [InlineData("20:02:04:04:05:AB")]
    public void MacAddress_Constructor_CreatesValidMac_FromString(string value)
    {
        // Arrange
        var macAddress = new MacAddress(value);

        // Act

        // Assert
        macAddress.ToString().Should().Be(value);
    }


    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void MacAddress_Constructor__WithString_ThrowNullException_ForNullInput(string? data)
    {
        Assert.Throws<ArgumentNullException>(() => new MacAddress(macAddress: data!));
    }
    
    [Theory]
    [InlineData("00:00:00:00:00")] 
    [InlineData("00:00:00:00:00:00:00")] 
    public void MacAddress_Constructor_ThrowExceptionForInvalidStringInput(string macAddress)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MacAddress(macAddress));
    }

    [Theory]
    [InlineData("01:02:03:04:05:06")]
    [InlineData("AA:03:FF:01:55:CC")]
    [InlineData("FF:02:03:04:05:CA")]
    [InlineData("10:02:03:04:BB:06")]
    [InlineData("20:02:04:04:05:AB")]
    public void MacAddress_ToString_EqualsValue(string value)
    {
        // Arrange
        var macAddress = new MacAddress(value);

        // Act

        // Assert
        macAddress.ToString().Should().Be(value);
    }

    [Fact]
    public void MacAddress_ToString_ReturnsCorrectFormat()
    {
        var mac = new MacAddress(new byte[] { 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56 });
        Assert.Equal("AB:CD:EF:12:34:56", mac.ToString());
    }

    [Fact]
    public void MacAddress_Empty_CreatesEmptyAddress()
    {
        // Arrange
        var macAddress = MacAddress.Empty;

        // Act

        // Assert
        macAddress.ToString().Should().Be("00:00:00:00:00:00");
    }

    [Fact]
    public void MacAddress_Equals_ReturnsTrueForEqualAddresses()
    {
        var mac1 = new MacAddress(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 });
        var mac2 = new MacAddress(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 });

        Assert.Equal(mac1, mac2);
        Assert.True(mac1 == mac2);
        Assert.Equal(mac1.GetHashCode(), mac2.GetHashCode());
    }

    [Fact]
    public void MacAddress_Equals_ReturnsFalseForDifferentAddresses()
    {
        var mac1 = new MacAddress(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 });
        var mac2 = new MacAddress(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x07 });

        Assert.NotEqual(mac1, mac2);
        Assert.True(mac1 != mac2);
        Assert.NotEqual(mac1.GetHashCode(), mac2.GetHashCode());
    }


    [Fact]
    public void MacAddress_ImplicitOperators_WorkCorrectly()
    {
        var mac = new MacAddress(new byte[] { 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56 });
        string macString = mac;
        byte[] macBytes = mac;

        Assert.Equal("AB:CD:EF:12:34:56", macString);
        Assert.Equal(new byte[] { 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56 }, macBytes);
    }


    [Theory]
    [InlineData("00:00:00:00:00:00")]
    public void New_MacAddress_WithValidString_CreatesValid_MacAddress(string value)
    {
        // Arrange
        var macAddress = new MacAddress(value);

        // Act

        // Assert
        macAddress.ToString().Should().BeEquivalentTo(value);
    }
    
    [Theory]
    [InlineData("00:00:00:00:00:00", "00:00:00:00:00:00")]
    public void MacAddresses_WithSameValue_ShouldBeEqual(string mac1, string mac2)
    {
        // Arrange
        var macAddress1 = new MacAddress(mac1);
        var macAddress2 = new MacAddress(mac2);

        // Act
        var equal = macAddress1 == macAddress2;

        // Assert
        equal.Should().BeTrue();
    }
}