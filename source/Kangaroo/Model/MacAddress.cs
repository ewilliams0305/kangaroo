using System.Text;

namespace Kangaroo;

/// <summary>
/// A validated mac address
/// </summary>
public readonly struct MacAddress
{
    private const char ColonChar = ':';
    private const string ColonString = ":";

    private readonly byte[] _bytes = new byte[6];

    /// <summary>
    /// The first byte of the mac address
    /// </summary>
    public byte FirstByte => _bytes[0];
    /// <summary>
    /// The second byte of the mac address
    /// </summary>
    public byte SecondByte => _bytes[1];
    /// <summary>
    /// The third byte of the mac address
    /// </summary>
    public byte ThirdByte => _bytes[2];
    /// <summary>
    /// The forth byte of the mac address
    /// </summary>
    public byte ForthByte => _bytes[3];
    /// <summary>
    /// The fifth byte of the mac address
    /// </summary>
    public byte FifthByte => _bytes[4];
    /// <summary>
    /// The sixth byte of the mac address
    /// </summary>
    public byte SixthByte => _bytes[5];

    /// <summary>
    /// Creates a new mac address from an array of bytes.
    /// </summary>
    /// <remarks>must be 6 bytes long</remarks>
    /// <param name="bytes">byte values</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public MacAddress(byte[] bytes)
    {
        if (bytes == null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        if (bytes.Length != 6)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes));
        }

        _bytes = bytes;
    }

    /// <summary>
    /// Creates a new mac address from a string.
    /// <remarks>The string must be formatted with : separated hex values</remarks>
    /// </summary>
    /// <param name="macAddress">string value</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public MacAddress(string macAddress)
    {

        if (string.IsNullOrEmpty(macAddress))
        {
            throw new ArgumentNullException(nameof(macAddress));
        }

        if (macAddress.Length != 17)
        {
            throw new ArgumentOutOfRangeException(nameof(macAddress));
        }

        var bytes = Convert.FromHexString(macAddress.Replace(ColonString, ""));

        if (bytes.Length != 6)
        {
            throw new ArgumentOutOfRangeException(nameof(macAddress));
        }
        _bytes = bytes;
    }

    /// <inheritdoc />
    public override string ToString() => 
        new StringBuilder()
            .Append(FirstByte.ToString("X2")).Append(ColonChar)
            .Append(SecondByte.ToString("X2")).Append(ColonChar)
            .Append(ThirdByte.ToString("X2")).Append(ColonChar)
            .Append(ForthByte.ToString("X2")).Append(ColonChar)
            .Append(FifthByte.ToString("X2")).Append(ColonChar)
            .Append(SixthByte.ToString("X2")).ToString();

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is MacAddress other && Equals(other);
    }

    /// <summary>
    /// returns true when equal
    /// </summary>
    /// <param name="other">compare to</param>
    /// <returns>true or false</returns>
    public bool Equals(MacAddress other)
    {
        return _bytes.SequenceEqual(other._bytes);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = 0;
            foreach (byte b in _bytes)
            {
                hashCode = (hashCode * 397) ^ b.GetHashCode();
            }
            return hashCode;
        }
    }

    /// <summary>
    /// True when the values used to create the mac address are equal
    /// </summary>
    /// <param name="left">mac address</param>
    /// <param name="right">mac address</param>
    /// <returns>boolean</returns>
    public static bool operator ==(MacAddress left, MacAddress right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// True when the values used to create the mac address are not equal
    /// </summary>
    /// <param name="left">mac address</param>
    /// <param name="right">mac address</param>
    /// <returns>boolean</returns>
    public static bool operator !=(MacAddress left, MacAddress right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Creates a default invalid mac address
    /// </summary>
    public static MacAddress Empty => new(new[] { byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue });

    /// <summary>
    /// Returns the string formatted mac address.
    /// </summary>
    /// <param name="macAddress">the instance of a mac address</param>
    public static implicit operator string(MacAddress macAddress) => macAddress.ToString();

    /// <summary>
    /// Returns the mac address as a byte array
    /// </summary>
    /// <param name="macAddress"></param>
    /// <returns></returns>
    public static implicit operator byte[](MacAddress macAddress) => macAddress._bytes;

}