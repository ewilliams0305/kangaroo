using System;
using Avalonia.Controls;
using FluentAvalonia.UI.Windowing;

namespace Kangaroo.UI.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        InitializeComponent();

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
    }
}