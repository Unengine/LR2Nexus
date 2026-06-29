using LR2Nexus.I18n;

namespace LR2Nexus.Model
{
	public class AppSettings
	{
		public string? Lr2Path { get; set; } = null;

		private string? _recentPlayerName;

		public string? RecentPlayerName
		{
			get => _recentPlayerName;
			set
			{
				if (_recentPlayerName != value)
				{
					_recentPlayerName = value;
					ProfileChanged?.Invoke(value ?? string.Empty);
				}
			}
		}

		public Dictionary<string, string?> Players { get; set; } = [];
		public I18nManager.Language Language { get; set; } = I18nManager.Language.Default;

		public event Action<string> ProfileChanged = delegate { };
	}
}
