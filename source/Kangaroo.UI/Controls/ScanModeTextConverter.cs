using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Kangaroo.UI.Models;

namespace Kangaroo.UI.Controls;

public class ScanModeTextConverter : IValueConverter
{
    public static readonly ScanModeTextConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ScanMode mode && targetType.IsAssignableTo(typeof(string)))
        {
            return mode switch
            {
                ScanMode.NetworkSubnet => "Network Subnet",
                ScanMode.AddressRange => "IP Address Range",
                ScanMode.NetworkAdapter => "Network Adapter",
                ScanMode.SingleAddress => "IP Address",
                ScanMode.SpecifiedAddresses => "IP Addresses",
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