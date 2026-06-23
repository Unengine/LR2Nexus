using CommunityToolkit.Mvvm.ComponentModel;

namespace LR2Nexus.src.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? _lr2Path;

        [ObservableProperty]
        private string? _currentProfileName;
    }
}
