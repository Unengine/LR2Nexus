using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using LR2Nexus.I18n;
using LR2Nexus.Services;
using LR2Nexus.src.Utils;
using LR2Nexus.ViewModels;

namespace LR2Nexus.Views;

public partial class Home : UserControl
{
	private readonly HomeViewModel _viewModel = new();

	public Home()
	{
		InitializeComponent();

		DataContext = _viewModel;

		// Dispatcher.UIThread.Post ensures this runs after the UI is fully rendered
		Dispatcher.UIThread.Post(InitializeState);
	}

	private void InitializeState()
	{

	}

	private async void OnSelectPathClick(object sender, RoutedEventArgs e)
	{
		var topLevel = TopLevel.GetTopLevel(this);

		var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
		{
			Title = I18nManager.Instance["SetLR2Path"],
			AllowMultiple = false,
			FileTypeFilter = [new FilePickerFileType("Executable") { Patterns = ["*.exe"] }]
		});

		if (files.Count >= 1)
		{
			LauncherSettingManager.SetLR2BodyPath(files[0].Path.LocalPath);
		}

		UpdateViewModel();
	}

	private async void OnAddProfileClick(object sender, RoutedEventArgs e)
	{
		var topLevel = TopLevel.GetTopLevel(this);
		if (topLevel is not Window window) return;

		var id = await InputWindow.PromptWithRegexAsync(
			window,
			I18nManager.Instance["CreateNewProfileTitle"],
			LauncherSettingManager.ProfileNamePattern,
			I18nManager.Instance["CreateNewProfileContent"]);
		if (string.IsNullOrEmpty(id)) return;
		if (LauncherSettingManager.HasPlayerDB(id))
		{
			var message = string.Format(I18nManager.Instance["PlayerAlreadyExistsFormat"], id);

			await AlertWindow.PromptAsync(
				window,
				I18nManager.Instance["Error"],
				message);
			return;
		}

		var password = await InputWindow.PromptWithRegexAsync(
			window,
			I18nManager.Instance["CreateNewProfileTitle"],
			LauncherSettingManager.PasswordPattern,
			I18nManager.Instance["CreateNewProfilePwdContent1"],
			I18nManager.Instance["PasswordConditionContent1"],
			$"<b>{I18nManager.Instance["PasswordConditionContent2"]}</b>");
		if (string.IsNullOrEmpty(password)) return;

		if (LauncherSettingManager.TryCreateNewPlayer(id!, password!))
		{
			UpdateViewModel();
		}
	}

	private async void OnDeleteProfileClick(object sender, RoutedEventArgs e)
	{
		var topLevel = TopLevel.GetTopLevel(this);
		if (topLevel is not Window window) return;

		var currentId = LauncherSettingManager.Current.RecentPlayerName;

		if (string.IsNullOrWhiteSpace(currentId)) return;

		var id = await InputWindow.PromptWithTargetAsync(window,
			I18nManager.Instance["DeleteProfileTitle"],
			currentId,
			I18nManager.Instance["DeleteProfileContent1"],
			$"<warn>{currentId}</warn>",
			I18nManager.Instance["DeleteProfileContent2"],
			I18nManager.Instance["DeleteProfileContent3"]) ?? string.Empty;

		if (currentId == id &&
			LauncherSettingManager.TryDeletePlayer(currentId))
		{
			UpdateViewModel();
		}
	}

	public void UpdateViewModel()
	{
		_viewModel.Update();
	}

	private void OnResolutionXTextChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.System.WindowSizeX = TextBoxExtension.FilterIntegerText(textBox, default, 1920);
	}

	private void OnResolutionYTextChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.System.WindowSizeY = TextBoxExtension.FilterIntegerText(textBox, default, 1080);
	}

	private void OnScreenModeClick(object? sender, RoutedEventArgs e)
	{
		// TODO : Add ScreenMode == 2 case when it is available.
		int targetValue;
		if (_viewModel.ScreenMode == 0)
		{
			targetValue = 1;
		}
		else
		{
			targetValue = 0;
		}

		_viewModel.ScreenMode = targetValue;
		GameConfigService.Current.System.Screenmode = targetValue;
	}
}
