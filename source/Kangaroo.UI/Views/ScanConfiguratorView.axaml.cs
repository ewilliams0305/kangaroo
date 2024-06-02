using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Kangaroo.UI.Views;

public partial class ScanConfiguratorView : UserControl
{
    public ScanConfiguratorView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ViewModels.ScanConfiguratorViewModel>();
    }
}
