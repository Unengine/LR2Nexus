using CommunityToolkit.Mvvm.ComponentModel;
using LR2Nexus.Services;
using LR2Nexus.Views;
using System.Collections.ObjectModel;
using LR2Nexus.I18n;

namespace LR2Nexus.ViewModels
{
	public partial class HomeViewModel : ObservableObject
	{
		[ObservableProperty]

		private string? _lr2Path;

		private string? _selectedPlayer;
		public string? SelectedPlayer
		{
			get => _selectedPlayer;
			set
			{
				if (_selectedPlayer == value) return;

				var isPlayerDifferent =
					LauncherSettingManager.Current.RecentPlayerName != null ||
					LauncherSettingManager.Current.RecentPlayerName != value;
				if (isPlayerDifferent && LauncherSettingManager.TryChangeCurrentProfile(value ?? string.Empty))
				{
					_selectedPlayer = value;
				}

				OnPropertyChanged(nameof(SelectedPlayer));
			}
		}

		[ObservableProperty]
		private string? _windowSizeX;

		[ObservableProperty]
		private string? _windowSizeY;

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(ScreenModeText))]
		private int? _screenMode;

		public string ScreenModeText => ScreenMode switch
		{
			0 => I18nManager.Instance["BorderlessFullscreen"],
			1 => I18nManager.Instance["Windowed"],
			2 => I18nManager.Instance["Fullscreen"],
			_ => I18nManager.Instance["Windowed"],
		};

		public ObservableCollection<string> AvailableProfiles { get; set; } = [];

		public HomeViewModel()
		{
			Update();

			I18nManager.Instance.PropertyChanged += (s, e) =>
			{
				OnPropertyChanged(string.Empty);
			};

			var playerName = LauncherSettingManager.Current.RecentPlayerName;
			var targetPlayer = LauncherSettingManager.HasPlayerDB(playerName) ? playerName : null;
			SelectedPlayer = LauncherSettingManager.Current.RecentPlayerName;
		}

		public void Update()
		{
			WindowSizeX = GameConfigService.Current.System.WindowSizeX.ToString();
			WindowSizeY = GameConfigService.Current.System.WindowSizeY.ToString();
			ScreenMode = GameConfigService.Current.System.Screenmode;
			Lr2Path = LauncherSettingManager.Current.Lr2Path;

			var newProfiles = LauncherSettingManager.Current.Players
					.Select(kvp => kvp.Key)
					.ToHashSet();
			var toRemove = AvailableProfiles.Where(
				id => !newProfiles.Contains(id))
				.ToList();
			foreach (var id in toRemove)
			{
				AvailableProfiles.Remove(id);
			}

			foreach (var id in newProfiles)
			{
				if (!AvailableProfiles.Contains(id))
				{
					AvailableProfiles.Add(id);
				}
			}

			SelectedPlayer = LauncherSettingManager.Current.RecentPlayerName;
		}
	}
}
