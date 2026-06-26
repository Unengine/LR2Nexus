using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LR2Nexus.I18n;
using LR2Nexus.Services;
using LR2Nexus.ViewModels;

namespace LR2Nexus.Views;

public partial class MainWindow : Window
{
	private readonly MainWindowViewModel _viewModel = new();

	public MainWindow()
	{
		InitializeComponent();

		Opened += (sender, e) =>
		{
			var screen = Screens.ScreenFromWindow(this);
			if (screen != null)
			{
				double targetWidth = screen.WorkingArea.Width * 0.667;
				double targetHeight = screen.WorkingArea.Height * 0.667;

				var bestFit = GetClosestResolution(targetWidth, targetHeight);
				Width = bestFit.Width;
				Height = bestFit.Height;
			}
		};

		DataContext = _viewModel;

		SizeChanged += OnWindowSizeChanged;
		ProcessService.LR2Exited += OnLR2Exited;

		// Dispatcher.UIThread.Post ensures this runs after the UI is fully rendered
		Dispatcher.UIThread.Post(InitializeState);
	}

	private void InitializeState()
	{
		if (ProcessService.IsLR2Running)
		{
			LaunchGameButton.IsEnabled = false;
		}
	}

	private void OnMenuClick(object sender, PointerPressedEventArgs e)
	{
		if (sender is Border clickedBorder &&
			clickedBorder.Tag is string tag)
		{
			var newState = tag switch
			{
				"HomeMenu" => MenuState.Home,
				"JukeboxMenu" => MenuState.Jukebox,
				"PlayOptionMenu" => MenuState.PlayOption,
				"SystemOptionMenu" => MenuState.SystemOption,
				_ => MenuState.Home
			};

			_viewModel.CurrentMenuState = newState;
		}
	}

	private async void OnLaunchButtonClick(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrWhiteSpace(LauncherSettingManager.Current.Lr2Path) ||
			!File.Exists(LauncherSettingManager.Current.Lr2Path))
		{
			await AlertWindow.PromptAsync(
				this,
				I18nManager.Instance["Error"],
				I18nManager.Instance["SetLR2PathFirst"]);
			return;
		}

		try
		{
			var currentId = LauncherSettingManager.Current.RecentPlayerName;

			if (!LauncherSettingManager.HasPlayerDB(currentId))
			{
				var id = await InputWindow.PromptWithRegexAsync(
					this,
					I18nManager.Instance["CreateNewProfileTitle"],
					LauncherSettingManager.ProfileNamePattern,
					I18nManager.Instance["CreateNewProfileContent"]);
				if (string.IsNullOrEmpty(id)) return;

				if (LauncherSettingManager.HasPlayerDB(id))
				{
					var message = string.Format(I18nManager.Instance["PlayerAlreadyExistsFormat"], id);

					await AlertWindow.PromptAsync(
						this,
						I18nManager.Instance["Error"],
						message);
					return;
				}

				var password = await InputWindow.PromptWithRegexAsync(
					this,
					I18nManager.Instance["CreateNewProfileTitle"],
					LauncherSettingManager.PasswordPattern,
					I18nManager.Instance["CreateNewProfilePwdContent1"],
					I18nManager.Instance["PasswordConditionContent1"],
					$"<b>{I18nManager.Instance["PasswordConditionContent2"]}</b>");
				if (string.IsNullOrEmpty(password)) return;

				if (!LauncherSettingManager.TryCreateNewPlayer(id!, password!))
				{
					var message = string.Format(I18nManager.Instance["CreateNewProfileErrorFormat"], id);

					await AlertWindow.PromptAsync(
						this,
						I18nManager.Instance["Error"],
						message);
					return;
				}

				currentId = id;
			}

			if (GameConfigService.Current.Jukebox.Paths.Count == 0)
			{
				await AlertWindow.PromptAsync(
					this,
					I18nManager.Instance["Error"],
					I18nManager.Instance["ErrorEmptyJukeboxContent"]);
				return;
			}

			LaunchGameButton.IsEnabled = false;
			var success = await GameLauncherService.PrepareAndLaunchGameAsync(this, currentId!, InputWindow.PromptWithRegexAsync);
			if (!success) LaunchGameButton.IsEnabled = true;
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

	private void OnWindowSizeChanged(object? sender, SizeChangedEventArgs e)
	{
		const double targetRatio = 16.0 / 9.0;

		double currentWidth = Width;
		double currentHeight = Height;

		if (e.WidthChanged)
		{
			Height = currentWidth / targetRatio;
		}
		else if (e.HeightChanged)
		{
			Width = currentHeight * targetRatio;
		}
	}

	private (double Width, double Height) GetClosestResolution(double width, double height)
	{
		var resolutions = new[] {
			(640.0, 360.0),
			(1280.0, 720.0),
			(1920.0, 1080.0),
			(2560.0, 1440.0)
		};

		return resolutions.OrderBy(r => Math.Pow(r.Item1 - width, 2) + Math.Pow(r.Item2 - height, 2)).First();
	}

	public enum MenuState
	{
		Home,
		Jukebox,
		PlayOption,
		SystemOption
	}
}
