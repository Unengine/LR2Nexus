using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using LR2Nexus.Services;
using LR2Nexus.src.ViewModels;
using System.Diagnostics;

namespace LR2Nexus.Views;

internal partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel = new();

    public MainWindow()
    {
        InitializeComponent();


        DataContext = _viewModel;
        _viewModel.Lr2Path = ProcessManager.LR2BodyPath;

        ProcessManager.LR2Exited += OnLR2Exited;

        // Dispatcher.UIThread.Post ensures this runs after the UI is fully rendered
        Dispatcher.UIThread.Post(InitializeState);
    }

    private void InitializeState()
    {
        if (ProcessManager.IsLR2Running)
        {
            LaunchGameButton.IsEnabled = false;
        }
    }

    private void UpdateLR2Path(string path)
    {
        ProcessManager.LR2BodyPath = path;
        _viewModel.Lr2Path = ProcessManager.LR2BodyPath;
    }

    private void OnManageProfileClick(object sender, RoutedEventArgs e)
    {

    }

    private async void OnSelectPathClick(object sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);

        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select LR2 Client",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Executable") { Patterns = ["*.exe"] }]
        });

        if (files.Count >= 1)
        {
            UpdateLR2Path(files[0].Path.LocalPath);
            System.Console.WriteLine($"LR2Body path set : {ProcessManager.LR2BodyPath}");
        }
    }

    private void OnLaunchButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            LaunchGameButton.IsEnabled = false;
            ProcessManager.LaunchLR2Body();
        }
        catch (Exception ex)
        {
            LaunchGameButton.IsEnabled = true;
            Console.WriteLine($"LR2 launch error: {ex.Message}");
        }
    }

    private void OnLR2Exited(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            LaunchGameButton.IsEnabled = true;
        });
    }
}
