using System.Text;

namespace Kangaroo;

/// <summary>
/// A validated mac address
/// </summary>
public readonly record struct MacAddress
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

        switch (bytes.Length)
        {
            case 0:
                throw new ArgumentOutOfRangeException(nameof(bytes));
            case > 6:
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
        var b = new byte();

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

    /// <summary>
    /// Creates a default invalid mac address
    /// </summary>
    public static MacAddress Empty => new(new[] { byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue });

    /// <summary>
    /// Returns the string formatted mac address.
    /// </summary>
    /// <param name="macAddress">the instance of a mac address</param>
    public static implicit operator string(MacAddress macAddress) => macAddress.ToString();

}