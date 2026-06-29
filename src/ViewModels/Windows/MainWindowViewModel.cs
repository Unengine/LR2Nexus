using CommunityToolkit.Mvvm.ComponentModel;
using LR2Nexus.Services;
using LR2Nexus.View;
using LR2Nexus.I18n;

namespace LR2Nexus.ViewModel
{
	public partial class MainWindowViewModel : ObservableObject
	{
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(IsHomeMenu))]
		[NotifyPropertyChangedFor(nameof(IsJukeboxMenu))]
		[NotifyPropertyChangedFor(nameof(IsPlayOptionMenu))]
		[NotifyPropertyChangedFor(nameof(IsSystemOptionMenu))]
		[NotifyPropertyChangedFor(nameof(CurrentMenuName))]
		[NotifyPropertyChangedFor(nameof(CurrentMenu))]
		private MainWindow.MenuState _currentMenuState;

		public string CurrentMenu => CurrentMenuState.ToString();
		public string CurrentMenuName => GetMenuName(CurrentMenuState);
		public bool IsHomeMenu => CurrentMenuState == MainWindow.MenuState.Home;
		public bool IsJukeboxMenu => CurrentMenuState == MainWindow.MenuState.Jukebox;
		public bool IsPlayOptionMenu => CurrentMenuState == MainWindow.MenuState.PlayOption;
		public bool IsSystemOptionMenu => CurrentMenuState == MainWindow.MenuState.SystemOption;

		public MainWindowViewModel()
		{
			I18nManager.Instance.PropertyChanged += (s, e) =>
			{
				OnPropertyChanged(string.Empty);
			};

			CurrentMenuState = MainWindow.MenuState.Home;
			CurrentLanguage = I18nManager.Instance.GetLanguageOption(LauncherSettingManager.Current.Language);
		}

		private string GetMenuName(MainWindow.MenuState state) => state switch
		{
			MainWindow.MenuState.Home => I18nManager.Instance["Home"],
			MainWindow.MenuState.Jukebox => I18nManager.Instance["Jukebox"],
			MainWindow.MenuState.PlayOption => I18nManager.Instance["PlayOption"],
			MainWindow.MenuState.SystemOption => I18nManager.Instance["SystemOption"],
			_ => "MENU_NAME_NOT_FOUND"
		};

		public List<I18nManager.LanguageOption> Languages { get; } =
		[
			I18nManager.Instance.GetLanguageOption(I18nManager.Language.English),
			I18nManager.Instance.GetLanguageOption(I18nManager.Language.Japanese),
			I18nManager.Instance.GetLanguageOption(I18nManager.Language.Korean),
		];

		private I18nManager.LanguageOption? _currentLanguage;
		public I18nManager.LanguageOption CurrentLanguage
		{
			get => _currentLanguage ?? I18nManager.Instance.GetLanguageOption(I18nManager.Language.English);
			set
			{
				if (_currentLanguage != value &&
					SetProperty(ref _currentLanguage, value))
				{
					I18nManager.Instance.ChangeLanguage(value.Language);
				}
			}
		}
	}
}
