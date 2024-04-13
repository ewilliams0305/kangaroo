using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Kangaroo.UI.Controls;

public partial class ScanConfiguratorView : UserControl
{
    public ScanConfiguratorView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ScanConfiguratorViewModel>();
    }
}
