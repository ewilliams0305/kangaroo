using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Kangaroo.UI.Database;
using Kangaroo.UI.Models;

namespace Kangaroo.UI.Converters;

public class RecentScanTextConverter : IValueConverter
{
    public static readonly RecentScanTextConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is RecentScan scan && targetType.IsAssignableTo(typeof(string)))
        {
            return scan.ScanMode switch
            {
                ScanMode.NetworkSubnet => $"{scan.CreatedDateTime} {scan.StartAddress}\\{scan.SubnetMask}",
                ScanMode.AddressRange => $"{scan.CreatedDateTime} {scan.StartAddress} - {scan.EndAddress}",
                ScanMode.NetworkAdapter => $"{scan.CreatedDateTime} {scan.StartAddress} - {scan.EndAddress}",
                ScanMode.SingleAddress => $"{scan.CreatedDateTime} {scan.StartAddress}",
                ScanMode.SpecifiedAddresses => $"{scan.CreatedDateTime} {scan.SpecifiedAddresses}",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}