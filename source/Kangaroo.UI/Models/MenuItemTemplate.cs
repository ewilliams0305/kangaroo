using System;

namespace Kangaroo.UI.Models;

public sealed class MenuItemTemplate
{

    public string Label { get; set; }

    public Type ModelType { get; set; }

    public MenuItemTemplate(Type type, string label)
    {
        ModelType = type;
        Label = label;
    }
}