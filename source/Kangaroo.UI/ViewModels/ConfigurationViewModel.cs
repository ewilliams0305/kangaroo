using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Kangaroo.UI.ViewModels;

public partial class ConfigurationViewModel : ViewModelBase
{

    [ObservableProperty] private WithOptions _options = new();

    [ObservableProperty] private string _dbLocation = string.Empty;

    [ObservableProperty] private ThemeVariant _themeVariant = ThemeVariant.Dark;
}
