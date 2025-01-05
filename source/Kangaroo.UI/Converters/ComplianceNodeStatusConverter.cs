using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Kangaroo.UI.Models;

namespace Kangaroo.UI.Converters;

public class ComplianceNodeStatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is CompliantViewNodeModel apt && targetType.IsAssignableTo(typeof(Geometry)))
        {
            return apt switch
            {
                { IsRunning: false, IsCompleted: false, IsCompliant: false, IsError: false } => GetGeometry("IconWaiting"),
                { IsRunning: true, IsCompleted: false, IsCompliant: false, IsError: false } => GetGeometry("IconRunning"),
                { IsRunning: false, IsCompleted: true, IsCompliant: true, IsError: false }  => GetGeometry("IconCheck"),
                { IsRunning: false, IsCompleted: true, IsCompliant: false, IsError: true }  => GetGeometry("IconFailure"),
                _ => GetGeometry("IconWaiting")
            };
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private Geometry? GetGeometry(string icon)
    {
        if (!Application.Current!.TryFindResource(icon, out var obj))
        {
            return new StreamGeometry();
        }

        if (obj is not StreamGeometry streamGeo)
        {
            return new StreamGeometry();
        }
        
        return streamGeo;
    }
}