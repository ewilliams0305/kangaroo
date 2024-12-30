using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Kangaroo.UI.Models;

namespace Kangaroo.UI.Converters;

public class NetworkAdapterTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is NetworkAdapter apt && targetType.IsAssignableTo(typeof(string)))
        {
            return $"{apt.Name} | {apt.IpAddress} | {apt.MacAddress}";
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}