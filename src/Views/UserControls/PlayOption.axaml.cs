using Avalonia.Controls;
using Avalonia.Input;
using LR2Nexus.Services;
using LR2Nexus.src.Utils;
using LR2Nexus.ViewModel;

namespace LR2Nexus.View;

public partial class PlayOption : UserControl
{
	public PlayOptionViewModel _viewModel = new();

	public PlayOption()
	{
		InitializeComponent();

		DataContext = _viewModel;
		InitializeIntTextBoxEvents();
	}

	private void OnHSMaxChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.Play.HsMax = TextBoxExtension.FilterIntegerText(textBox, 10000);
	}

	private void OnHSMinChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.Play.HsMin = TextBoxExtension.FilterIntegerText(textBox, 10000);
	}

	private void OnBaseSpeedChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.Play.BaseSpeed = TextBoxExtension.FilterIntegerText(textBox, 10000);
	}

	private void OnHSStepChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.Play.HsStep = TextBoxExtension.FilterIntegerText(textBox, 10000);
	}

	private void OnSudHidStepChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.Play.SudHidStep = TextBoxExtension.FilterIntegerText(textBox, 100);
	}

	private void OnMaxDisplayedItemsChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.Select.SearchMax = TextBoxExtension.FilterIntegerText(textBox, 10000);
	}

	private void OnMinimumInputDelayChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.System.InputInterval = TextBoxExtension.FilterIntegerText(textBox, 16);
	}

	private void OnSongReloadTypeChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		var reloadType = GameConfigService.Current.System.AutoReload;
		_viewModel.ReloadType = reloadType != 2 ? reloadType + 1 : 0;
		GameConfigService.Current.System.AutoReload = _viewModel.ReloadType;
	}

	private void InitializeIntTextBoxEvents()
	{
		void updateHsMaxXEvent(object? s, FocusChangedEventArgs e) =>
			TextBoxExtension.UpdateIntegerTextOnEvent(GameConfigService.Current.Play.HsMax, s, e);
		void updateHsMinEvent(object? s, FocusChangedEventArgs e) =>
			TextBoxExtension.UpdateIntegerTextOnEvent(GameConfigService.Current.Play.HsMin, s, e);
		void updateBaseSpeedEvent(object? s, FocusChangedEventArgs e) =>
			TextBoxExtension.UpdateIntegerTextOnEvent(GameConfigService.Current.Play.BaseSpeed, s, e);
		void updateHsStepEvent(object? s, FocusChangedEventArgs e) =>
			TextBoxExtension.UpdateIntegerTextOnEvent(GameConfigService.Current.Play.HsStep, s, e);
		void updateSudHidStepEvent(object? s, FocusChangedEventArgs e) =>
			TextBoxExtension.UpdateIntegerTextOnEvent(GameConfigService.Current.Play.SudHidStep, s, e);
		void updateMaxDisplayedItemEvent(object? s, FocusChangedEventArgs e) =>
			TextBoxExtension.UpdateIntegerTextOnEvent(GameConfigService.Current.Select.SearchMax, s, e);
		void updateMinimumInputIntervalEvent(object? s, FocusChangedEventArgs e) =>
			TextBoxExtension.UpdateIntegerTextOnEvent(GameConfigService.Current.System.InputInterval, s, e);

		TextBoxHsMax.LostFocus += updateHsMaxXEvent;
		TextBoxHsMin.LostFocus += updateHsMinEvent;
		TextBoxBaseSpeed.LostFocus += updateBaseSpeedEvent;
		TextBoxHsStep.LostFocus += updateHsStepEvent;
		TextBoxSudHidStep.LostFocus += updateSudHidStepEvent;
		TextBoxMaxDisplayedItemCount.LostFocus += updateMaxDisplayedItemEvent;
		TextBoxMinimumInputInterval.LostFocus += updateMinimumInputIntervalEvent;

		Unloaded += (_, _) =>
		{
			TextBoxHsMax.LostFocus -= updateHsMaxXEvent;
			TextBoxHsMin.LostFocus -= updateHsMinEvent;
			TextBoxBaseSpeed.LostFocus -= updateBaseSpeedEvent;
			TextBoxHsStep.LostFocus -= updateHsStepEvent;
			TextBoxSudHidStep.LostFocus -= updateSudHidStepEvent;
			TextBoxMaxDisplayedItemCount.LostFocus -= updateMaxDisplayedItemEvent;
			TextBoxMinimumInputInterval.LostFocus -= updateMinimumInputIntervalEvent;
		};
	}
}
