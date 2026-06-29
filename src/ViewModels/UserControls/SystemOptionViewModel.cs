using Avalonia.Controls;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using LR2Nexus.I18n;
using LR2Nexus.Model;
using LR2Nexus.Services;
using LR2Nexus.View;
using System.Collections.ObjectModel;

namespace LR2Nexus.ViewModel
{
	public partial class SystemOptionViewModel : ObservableObject
	{
		[ObservableProperty]
		private string? _misslayerDuration;

		private int _enableVSync;
		public bool EnableVSync
		{
			get => (_enableVSync == 1);
			set
			{
				if ((_enableVSync == 1) == value) return;
				var enabled = value ? 1 : 0;
				_enableVSync = enabled;
				GameConfigService.Current.System.VSync = enabled;
				OnPropertyChanged(nameof(EnableVSync));
			}
		}

		public List<ISoundDriver> SoundDrivers { get; set; } = [];

		[ObservableProperty]
		public ISoundDriver? _selectedSoundDriver;

		public ObservableCollection<string> PlaybackDrivers { get; set; } = [];

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(PlaybackDrivers))]
		public string? _selectedPlaybackDriver;

		[ObservableProperty]
		private string? _audioBufferSize;

		private int _disableFmodEx;
		public bool DisableFmodEx
		{
			get => (_disableFmodEx == 1);
			set
			{
				if ((_disableFmodEx == 1) == value) return;
				var enabled = value ? 1 : 0;
				_disableFmodEx = enabled;
				GameConfigService.Current.Sound.DisableFmodEx = enabled;
				OnPropertyChanged(nameof(DisableFmodEx));
			}
		}

		public SystemOptionViewModel()
		{
			MisslayerDuration = GameConfigService.Current.Play.MisslayerDuration.ToString();
			EnableVSync = GameConfigService.Current.System.VSync == 1;

			AudioBufferSize = GameConfigService.Current.Sound.AudioBufferSize.ToString();
			DisableFmodEx = GameConfigService.Current.Sound.DisableFmodEx == 1;
			LoadSoundDriverSettings();
		}

		private void LoadSoundDriverSettings()
		{
			if (App.Current is not App app || app.SoundDrivers == null) return;

			var supportedDrivers = app.SoundDrivers.Where(x => x.IsSupported);
			foreach (var driver in supportedDrivers)
			{
				SoundDrivers.Add(driver);
			}

			var soundDriverCount = supportedDrivers.Count();
			if (soundDriverCount == 0) return;

			var soundDriverIndex = GameConfigService.Current.Sound.SoundDriver;
			SelectedSoundDriver = soundDriverIndex < soundDriverCount ?
				SoundDrivers[soundDriverIndex] : SoundDrivers[0];

			if (SelectedSoundDriver.Drivers == null) return;

			var playbackDriverCount = SelectedSoundDriver.Drivers.Count;
			if (playbackDriverCount == 0) return;

			var playbackDriverIndex = GameConfigService.Current.Sound.PlaybackDriver;
			SelectedPlaybackDriver = playbackDriverIndex < playbackDriverCount ?
				SelectedSoundDriver.Drivers[playbackDriverIndex] : null;
		}

		#region Unused
		public ObservableCollection<MonitorInfo> AvailableMonitors { get; set; } = [];

		[ObservableProperty]
		public MonitorInfo? _selectedMonitor;
		public void UpdateMonitorDevices(TopLevel topLevel)
		{
			IReadOnlyList<Screen>? unsortedMonitors = topLevel?.Screens?.All;

			if (unsortedMonitors != null && unsortedMonitors.Count > 0)
			{
				var monitors = unsortedMonitors.OrderByDescending(m => m.IsPrimary).ToList();

				for (int i = 0; i < monitors.Count; i++)
				{
					var monitor = monitors[i];
					var width = monitor.Bounds.Width;
					var height = monitor.Bounds.Height;
					var primaryTag = monitor.IsPrimary ? $" ({I18nManager.Instance["PrimaryDevice"]})" : string.Empty;
					var name = $"{i + 1} : {width}×{height} {primaryTag}";

					AvailableMonitors.Add(new MonitorInfo
					{
						DisplayName = name,
						Index = i,
						IsPrimary = monitor.IsPrimary,
						ResolutionX = width,
						ResolutionY = height
					});
				}

				var expectedMonitor = AvailableMonitors?.FirstOrDefault(
					m => m.Index == GameConfigService.Current.System.MainDisplay);
				SelectedMonitor = expectedMonitor ?? AvailableMonitors!.First();
			}
		}

		#endregion
	}
}
