using Avalonia.Controls;
using Avalonia.Input;
using LR2Nexus.Model;
using LR2Nexus.Services;
using LR2Nexus.src.Utils;
using LR2Nexus.ViewModel;

namespace LR2Nexus.View;

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

			// seems like <maindisplay> is not working for now.
			// if (topLevel != null) _viewModel.UpdateMonitorDevices(topLevel);
			_isInitialized = true;
		};

		DataContext = _viewModel;
		InitializeIntTextBoxEvents();
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
		void updateAudioBufferSizeEvent(object? s, FocusChangedEventArgs e) =>
			TextBoxExtension.UpdateIntegerTextOnEvent(GameConfigService.Current.Sound.AudioBufferSize, s, e);

		TextBoxMisslayerDuration.LostFocus += updateMisslayerDurationEvent;
		TextBoxAudioBufferSize.LostFocus += updateAudioBufferSizeEvent;

		Unloaded += (_, _) =>
		{
			TextBoxMisslayerDuration.LostFocus -= updateMisslayerDurationEvent;
			TextBoxAudioBufferSize.LostFocus -= updateAudioBufferSizeEvent;
		};
	}

	#region Unused

	private void OnDisplayMonitorChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (sender is not ComboBox comboBox || comboBox.SelectedItem is not MonitorInfo monitorInfo) return;
		var index = monitorInfo.Index;
		var width = monitorInfo.ResolutionX;
		var height = monitorInfo.ResolutionY;
		GameConfigService.Current.System.MainDisplay = index;
		Console.WriteLine($"Main Display Changed : {monitorInfo.Index} : {width}x{height}");
	}

	#endregion

	private void OnSoundDriverChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (sender is not ComboBox comboBox ||
			comboBox.SelectedItem is not ISoundDriver driver) return;

		_viewModel.SelectedSoundDriver = driver;
		if (driver.Drivers != null && driver.Drivers.Count > 0)
		{
			foreach (var playbackDriver in driver.Drivers)
			{
				_viewModel.PlaybackDrivers.Add(playbackDriver);
			}
			_viewModel.SelectedPlaybackDriver = driver.Drivers.First();
		}
		else
		{
			_viewModel.PlaybackDrivers.Clear();
		}

		GameConfigService.Current.Sound.SoundDriver = (int)driver.DriverType;
	}

	private void OnPlaybackDriverChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (sender is not ComboBox comboBox ||
			comboBox.SelectedItem is not string driverName ||
			_viewModel.SelectedSoundDriver?.Drivers == null) return;

		var index = _viewModel.SelectedSoundDriver.Drivers.IndexOf(driverName);
		var isFound = index > -1;
		_viewModel.SelectedPlaybackDriver = isFound ? driverName : null;
		GameConfigService.Current.Sound.PlaybackDriver = isFound ? index : 0;
	}

	private void OnAudioBufferSizeChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;
		GameConfigService.Current.Sound.AudioBufferSize =
			TextBoxExtension.FilterIntegerText(textBox, GameConfig.AudioBufferSizeMax, CorrectAudioBufferSize);
	}

	private int CorrectAudioBufferSize(int value)
	{
		if (value < GameConfig.AudioBufferSizeMin) return value;

		value = int.Clamp(value, GameConfig.AudioBufferSizeMin, GameConfig.AudioBufferSizeMax);
		var nearest = (int)Math.Pow(2, Math.Round(Math.Log(value, 2)));
		return nearest;
	}

	private void OnClickDisableFmodExButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		if (sender is not CheckBox checkBox) return;
		PlaybackDriverComboBox.IsEnabled = !checkBox.IsChecked ?? true;
		SoundDriverComboBox.IsEnabled = !checkBox.IsChecked ?? true;
	}
}
