using CommunityToolkit.Mvvm.ComponentModel;
using LR2Nexus.I18n;
using LR2Nexus.Model;
using LR2Nexus.Services;

namespace LR2Nexus.ViewModel
{
	public partial class PlayOptionViewModel : ObservableObject
	{
		#region Scroll Settings
		[ObservableProperty]
		private string? _hsMax;

		[ObservableProperty]
		private string? _hsMin;

		[ObservableProperty]
		private string? _baseSpeed;

		[ObservableProperty]
		private string? _hsStep;

		[ObservableProperty]
		private string? _sudHidStep;
		#endregion

		#region Custom Folder Settings

		private int _useFolderLamps;
		public bool UseFolderLamps
		{
			get => (_useFolderLamps == 1);
			set
			{
				if ((_useFolderLamps == 1) == value) return;
				var enabled = value ? 1 : 0;
				_useFolderLamps = enabled;
				GameConfigService.Current.Select.FolderLamp = enabled;
				OnPropertyChanged(nameof(UseFolderLamps));
			}
		}

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(RandomSelectEnabled))]
		[NotifyPropertyChangedFor(nameof(FavoriteFolderEnabled))]
		[NotifyPropertyChangedFor(nameof(Top10PlaycountFolderEnabled))]
		[NotifyPropertyChangedFor(nameof(LevelFolderEnabled))]
		[NotifyPropertyChangedFor(nameof(ClearTypeFolderEnabled))]
		[NotifyPropertyChangedFor(nameof(PlayrankFolderEnabled))]
		[NotifyPropertyChangedFor(nameof(IgnoredFolderEnabled))]
		[NotifyPropertyChangedFor(nameof(InsaneBMSFolderEnabled))]
		private int _customFolderValue;

		[ObservableProperty]
		private string? _maxDisplayedItemCount;

		public bool RandomSelectEnabled
		{
			get => (CustomFolderValue & GameConfig.BitRandomSelect) != 0;
			set => UpdateFolderBit(GameConfig.BitRandomSelect, value);
		}

		public bool FavoriteFolderEnabled
		{
			get => (CustomFolderValue & GameConfig.BitFavoriteFolder) != 0;
			set => UpdateFolderBit(GameConfig.BitFavoriteFolder, value);
		}

		public bool Top10PlaycountFolderEnabled
		{
			get => (CustomFolderValue & GameConfig.BitTop10Playcount) != 0;
			set => UpdateFolderBit(GameConfig.BitTop10Playcount, value);
		}

		public bool LevelFolderEnabled
		{
			get => (CustomFolderValue & GameConfig.BitLevelFolder) != 0;
			set => UpdateFolderBit(GameConfig.BitLevelFolder, value);
		}

		public bool ClearTypeFolderEnabled
		{
			get => (CustomFolderValue & GameConfig.BitClearTypeFolder) != 0;
			set => UpdateFolderBit(GameConfig.BitClearTypeFolder, value);
		}

		public bool PlayrankFolderEnabled
		{
			get => (CustomFolderValue & GameConfig.BitPlayrank) != 0;
			set => UpdateFolderBit(GameConfig.BitPlayrank, value);
		}

		public bool IgnoredFolderEnabled
		{
			get => (CustomFolderValue & GameConfig.BitIgnoredFolder) != 0;
			set => UpdateFolderBit(GameConfig.BitIgnoredFolder, value);
		}

		public bool InsaneBMSFolderEnabled
		{
			get => (CustomFolderValue & GameConfig.BitInsaneBMSFolder) != 0;
			set => UpdateFolderBit(GameConfig.BitInsaneBMSFolder, value);
		}

		#endregion

		#region Miscellaneous Settings

		private int _songPreviewEnabled;
		public bool SongPreviewEnabled
		{
			get => (_songPreviewEnabled == 1);
			set
			{
				if ((_songPreviewEnabled == 1) == value) return;
				var enabled = value ? 1 : 0;
				_songPreviewEnabled = enabled;
				GameConfigService.Current.Select.Preview = enabled;
				OnPropertyChanged(nameof(SongPreviewEnabled));
			}
		}

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(ReloadTypeName))]
		private int _reloadType;

		[ObservableProperty]
		private string? _minimumInputInterval;

		public string ReloadTypeName => ReloadType switch
		{
			0 => I18nManager.Instance["Manual"],
			1 => I18nManager.Instance["AutoType1"],
			2 => I18nManager.Instance["AutoType2"],
			_ => I18nManager.Instance["Error"]
		};

		#endregion

		public PlayOptionViewModel()
		{
			HsMax = GameConfigService.Current.Play.HsMax.ToString();
			HsMin = GameConfigService.Current.Play.HsMin.ToString();
			BaseSpeed = GameConfigService.Current.Play.BaseSpeed.ToString();
			HsStep = GameConfigService.Current.Play.HsStep.ToString();
			SudHidStep = GameConfigService.Current.Play.SudHidStep.ToString();

			UseFolderLamps = GameConfigService.Current.Select.FolderLamp == 1;
			CustomFolderValue = GameConfigService.Current.System.CustomFolder;
			MaxDisplayedItemCount = GameConfigService.Current.Select.SearchMax.ToString();

			SongPreviewEnabled = GameConfigService.Current.Select.Preview == 1;
			ReloadType = GameConfigService.Current.System.AutoReload;
			MinimumInputInterval = GameConfigService.Current.System.InputInterval.ToString();
		}

		private void UpdateFolderBit(int bit, bool isEnabled)
		{
			if (isEnabled)
				CustomFolderValue |= bit;
			else
				CustomFolderValue &= ~bit;

			GameConfigService.Current.System.CustomFolder = CustomFolderValue;
			OnPropertyChanged(nameof(CustomFolderValue));
		}
	}
}
