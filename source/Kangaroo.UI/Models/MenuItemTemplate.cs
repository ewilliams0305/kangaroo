using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Kangaroo.UI.Models;

public sealed class MenuItemTemplate
{
    public string Label { get; set; }

    public StreamGeometry? Icon { get; set; }

    public Type ModelType { get; set; }

    public MenuItemTemplate(Type type, string label, string icon)
    {
        ModelType = type;
        Label = label;

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