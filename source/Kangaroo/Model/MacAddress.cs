using System.Text;

namespace Kangaroo;

public readonly record struct MacAddress
{
    private const char ColonChar = ':';
    private const string ColonString = ":";

    private readonly byte[] _bytes = new byte[6];

    public byte FirstByte => _bytes[0];
    public byte SecondByte => _bytes[1];
    public byte ThirdByte => _bytes[2];
    public byte ForthByte => _bytes[3];
    public byte FifthByte => _bytes[4];
    public byte SixthByte => _bytes[5];

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

    public static MacAddress Empty => new (new[] { byte.MinValue, byte.MinValue , byte.MinValue , byte.MinValue , byte.MinValue , byte.MinValue });

    #region Overrides of ValueType

    /// <inheritdoc />
    public override string ToString() => 
        new StringBuilder()
            .Append(FirstByte.ToString("X2")).Append(ColonChar)
            .Append(SecondByte.ToString("X2")).Append(ColonChar)
            .Append(ThirdByte.ToString("X2")).Append(ColonChar)
            .Append(ForthByte.ToString("X2")).Append(ColonChar)
            .Append(FifthByte.ToString("X2")).Append(ColonChar)
            .Append(SixthByte.ToString("X2")).ToString();

    #endregion
}