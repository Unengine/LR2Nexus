using Avalonia.Controls;
using Avalonia.Input;
using LR2Nexus.Models;
using LR2Nexus.Services;
using LR2Nexus.src.Utils;
using LR2Nexus.ViewModels;
using System.Diagnostics;

namespace LR2Nexus.Views;

public partial class SystemOption : UserControl
{
	private SystemOptionViewModel _viewModel = new();

	private bool _isInitialized = false;

	public SystemOption()
	{
		InitializeComponent();

		AttachedToVisualTree += (s, e) =>
		{
			if (_isInitialized) return;

			var topLevel = TopLevel.GetTopLevel(this);
			// seems like <maindisplay> is not working in config.xml for now.
			// if (topLevel != null) _viewModel.UpdateMonitorDevices(topLevel);
			_isInitialized = true;
		};

		DataContext = _viewModel;
		InitializeIntTextBoxEvents();
	}

	private void OnDisplayMonitorChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (sender is not ComboBox comboBox || comboBox.SelectedItem is not MonitorInfo monitorInfo) return;
		var index = monitorInfo.Index;
		var width = monitorInfo.ResolutionX;
		var height = monitorInfo.ResolutionY;
		GameConfigService.Current.System.MainDisplay = index;
		Console.WriteLine($"Main Display Changed : {monitorInfo.Index} : {width}x{height}");
	}

	private void OnMisslayerDurationChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.Play.MisslayerDuration = TextBoxExtension.FilterIntegerText(textBox, 5000);
	}

	private void InitializeIntTextBoxEvents()
	{
		void updateMisslayerDurationEvent(object? s, FocusChangedEventArgs e) =>
			TextBoxExtension.UpdateIntegerTextOnEvent(GameConfigService.Current.Play.MisslayerDuration, s, e);

		TextBoxMisslayerDuration.LostFocus += updateMisslayerDurationEvent;

		Unloaded += (_, _) =>
		{
			TextBoxMisslayerDuration.LostFocus -= updateMisslayerDurationEvent;
		};
	}
}
