using Avalonia.Controls;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using LR2Nexus.I18n;
using LR2Nexus.Models;
using LR2Nexus.Services;
using System.Collections.ObjectModel;

namespace LR2Nexus.ViewModels
{
	public partial class SystemOptionViewModel : ObservableObject
	{
		[ObservableProperty]
		private string? _misslayerDuration;

		public ObservableCollection<MonitorInfo> AvailableMonitors { get; set; } = [];

		[ObservableProperty]
		public MonitorInfo? _selectedMonitor;

		public SystemOptionViewModel()
		{
			MisslayerDuration = GameConfigService.Current.Play.MisslayerDuration.ToString();
		}

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
	}
}
