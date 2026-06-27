using Avalonia.Controls;
using LR2Nexus.Services;
using LR2Nexus.src.Utils;
using LR2Nexus.ViewModels;

namespace LR2Nexus.Views;

public partial class PlayOption : UserControl
{
	public PlayOptionViewModel _viewModel = new();

	public PlayOption()
	{
		InitializeComponent();

		DataContext = _viewModel;
	}

	private void OnHSMaxChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.Play.HsMax = TextBoxExtension.FilterIntegerText(textBox, default, 10000);
	}

	private void OnHSMinChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.Play.HsMin = TextBoxExtension.FilterIntegerText(textBox, default, 10000);
	}

	private void OnBaseSpeedChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.Play.BaseSpeed = TextBoxExtension.FilterIntegerText(textBox, default, 10000);
	}

	private void OnHSStepChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.Play.HsStep = TextBoxExtension.FilterIntegerText(textBox, default, 10000);
	}

	private void OnSudHidStepChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.Play.SudHidStep = TextBoxExtension.FilterIntegerText(textBox, default, 100);
	}

	private void OnMaxDisplayedItemsChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.Select.SearchMax = TextBoxExtension.FilterIntegerText(textBox, default, 10000);
	}

	private void OnSongReloadTypeChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		var reloadType = GameConfigService.Current.System.AutoReload;
		_viewModel.ReloadType = reloadType != 2 ? reloadType + 1 : 0;
		GameConfigService.Current.System.AutoReload = _viewModel.ReloadType;
	}
}
