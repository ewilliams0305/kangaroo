using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Kangaroo.UI.ViewModels;

public partial class ConfigurationViewModel : ViewModelBase
{

    [ObservableProperty] private WithOptions _options;

    [ObservableProperty] private string _dbLocation;

    [ObservableProperty] private ThemeVariant _themeVariant;
}
