using Avalonia.Controls;
using LR2Nexus.I18n;

namespace LR2Nexus.Services
{
	public class GameLauncherService
	{
		public delegate Task<string?> PasswordPromptDelegate(
			Window owner, string title, string? regex, params string[] messages);

		public static async Task<bool> PrepareAndLaunchGameAsync(
			Window owner,
			string playerId,
			PasswordPromptDelegate passwordPromptFunc)
		{
			if (!LauncherSettingManager.Current.Players.TryGetValue(playerId, out var password))
			{
				return true;
			}

			if (string.IsNullOrEmpty(password))
			{
				password = await passwordPromptFunc(
					owner,
					I18nManager.Instance["PasswordReset"],
					LauncherSettingManager.PasswordPattern,
					I18nManager.Instance["ResetPlayerPasswordContent"],
					I18nManager.Instance["PasswordConditionContent1"],
					$"<b>{I18nManager.Instance["PasswordConditionContent2"]}</b>");

				if (password == null) return false;

				LauncherSettingManager.Current.Players[playerId] = password;
				LauncherSettingManager.SaveAppSettings();
			}

			ProcessService.LaunchLR2Body(playerId);
			return true;
		}
	}
}
