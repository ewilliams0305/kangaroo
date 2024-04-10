using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Kangaroo.UI.Models;

public sealed class MenuItemTemplate
{
    /// <summary>
    /// Temp property to grey out menu items that are not yet configured with views
    /// </summary>
    public bool IsEnabled { get; }

    public string Label { get; set; }

    public StreamGeometry? Icon { get; set; }

    public Type ModelType { get; set; }

    public MenuItemTemplate(Type type, string label, string icon, bool isEnabled)
    {
        ModelType = type;
        Label = label;
        IsEnabled = isEnabled;

        if (!Application.Current!.TryFindResource(icon, out var obj))
        {
            Icon = new StreamGeometry();
        }

        if (obj is StreamGeometry streamGeo)
        {
            Icon = streamGeo;
        }
    }
}