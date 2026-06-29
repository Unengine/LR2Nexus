using Avalonia.Controls;
using Avalonia.Input;
using LR2Nexus.Model;
using LR2Nexus.Services;
using LR2Nexus.src.Utils;
using LR2Nexus.ViewModel;
using System.Collections.ObjectModel;

namespace LR2Nexus.View;

public partial class SystemOption : UserControl
{
	private SystemOptionViewModel _viewModel = new();
	private GameConfig.SoundDriver? _previousSoundDriverType = null;

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
		AudioBufferSizeTextBox.LostFocus += updateAudioBufferSizeEvent;

		Unloaded += (_, _) =>
		{
			TextBoxMisslayerDuration.LostFocus -= updateMisslayerDurationEvent;
			AudioBufferSizeTextBox.LostFocus -= updateAudioBufferSizeEvent;
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
		var playbackDriverCount = driver.Drivers?.Count;

		_viewModel.PlaybackDrivers.Clear();
		if (driver.Drivers != null && playbackDriverCount > 0)
		{
			foreach (var playbackDriver in driver.Drivers)
			{
				_viewModel.PlaybackDrivers.Add(playbackDriver);
			}
			var playbackDriverIndex = GameConfigService.Current.Sound.PlaybackDriver;

			if (_previousSoundDriverType == null)
			{
				_viewModel.SelectedPlaybackDriver = playbackDriverIndex < playbackDriverCount ?
					driver.Drivers[playbackDriverIndex] : driver.Drivers.First();
				GameConfigService.Current.Sound.PlaybackDriver = playbackDriverIndex < playbackDriverCount ?
					playbackDriverIndex : 0;
				_previousSoundDriverType = driver.DriverType;
			}
			else
			{
				_viewModel.SelectedPlaybackDriver = driver.Drivers.First();
				GameConfigService.Current.Sound.PlaybackDriver = 0;
			}
		}
		else
		{
			_viewModel.SelectedPlaybackDriver = null;
		}

		GameConfigService.Current.Sound.SoundDriver = (int)driver.DriverType;
	}

	private void OnPlaybackDriverChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (sender is not ComboBox comboBox ||
			comboBox.SelectedItem is not string driverName ||
			_viewModel.SelectedSoundDriver?.Drivers == null) return;

		var drivers = _viewModel.SelectedSoundDriver.Drivers;
		var playbackDriverCount = drivers.Count;
		var targetIndex = drivers.IndexOf(driverName);
		if (targetIndex >= 0)
		{
			GameConfigService.Current.Sound.PlaybackDriver =
				targetIndex < playbackDriverCount ? targetIndex : 0;
		}
		else
		{
			_viewModel.SelectedPlaybackDriver = null;
		}
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
		AudioBufferSizeTextBox.IsEnabled = !checkBox.IsChecked ?? true;
	}
}
