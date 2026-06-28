using Avalonia.Controls;
using LR2Nexus.I18n;
using LR2Nexus.Models;
using LR2Nexus.Utils;
using LR2Nexus.Views;
using Microsoft.Data.Sqlite;

namespace LR2Nexus.Services
{
	public class GameLauncherService
	{
		private const string SelectPlayerQuery = "SELECT * FROM player LIMIT 1";
		private const string UpdatePlayerHashQuery = "UPDATE player SET hash = @hash, scorehash = @scorehash";
		private const string SelectScoresQuery = "SELECT * from score";
		private const string UpdateScoreQuery = "Update score SET scorehash = @scorehash WHERE hash = @hash";

		public delegate Task<string?> PasswordPromptDelegate(
			Window owner, string title, string? regex, params string[] messages);

		public static async Task<bool> PrepareAndLaunchGameAsync(
			Window owner,
			string id,
			PasswordPromptDelegate passwordPromptFunc)
		{
			if (!LauncherSettingManager.Current.Players.TryGetValue(id, out var password))
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

				LauncherSettingManager.Current.Players[id] = password;
				LauncherSettingManager.SaveAppSettings();
			}

			bool isHashLegit;
			var dbPath = LauncherSettingManager.GetPlayerDBPath(id);
			using (var connection = new SqliteConnection($"Data Source={dbPath};"))
			{
				connection.Open();
				using var selectPlayerCmd = new SqliteCommand("SELECT hash FROM player LIMIT 1", connection);
				string? hash = selectPlayerCmd.ExecuteScalar()?.ToString();
				isHashLegit = hash != null && hash == MD5Util.CalculateMD5(password).Body;
			}

			if (!isHashLegit)
			{
				var cts = new CancellationTokenSource();
				var task = Task.Run(() => EnsurePlayerAuth(id, password, cts.Token), cts.Token);
				await LoadingWindow.PromptWithTaskAsync(owner, task, cts, false,
					I18nManager.Instance["SettingPassword"],
					I18nManager.Instance["SettingPasswordContent1"]);
			}
			
			GameProcessService.LaunchLR2Body(id);
			return true;
		}

		public static void EnsurePlayerAuth(string id, string targetPassword, CancellationToken token)
		{
			var dbPath = LauncherSettingManager.GetPlayerDBPath(id);

			using var connection = new SqliteConnection($"Data Source={dbPath};Mode=ReadWrite;");
			connection.Open();

			DBPlayerRow player;
			string? passwordHash, playerScorehash;
			using var selectPlayerCmd = new SqliteCommand(SelectPlayerQuery, connection);
			using (var reader = selectPlayerCmd.ExecuteReader())
			{
				if (!reader.Read()) throw new Exception("Player data not found.");

				player = new DBPlayerRow(reader);
				passwordHash = player.Hash;
				playerScorehash = player.Scorehash;
			}

			MD5Hash targetPlayerPasswordHashMD5 = MD5Util.CalculateMD5(targetPassword);
			MD5Hash targetPlayerScorehashMD5 = player.CalculateScorehash();

			if (passwordHash == targetPlayerPasswordHashMD5.Body && playerScorehash == targetPlayerScorehashMD5.Body) return;
			using var transaction = connection.BeginTransaction();
			try
			{
				using (var updatePlayerCmd = new SqliteCommand(UpdatePlayerHashQuery, connection, transaction))
				{
					updatePlayerCmd.Parameters.AddWithValue("@hash", targetPlayerPasswordHashMD5.Body);
					updatePlayerCmd.Parameters.AddWithValue("@scorehash", targetPlayerScorehashMD5.Body);
					updatePlayerCmd.ExecuteNonQuery();
				}

				using var updateScoreCmd = new SqliteCommand(UpdateScoreQuery, connection, transaction);
				updateScoreCmd.Parameters.Add("@scorehash", SqliteType.Text);
				updateScoreCmd.Parameters.Add("@hash", SqliteType.Text);
				updateScoreCmd.Prepare();

				using (var selectScoresCmd = new SqliteCommand(SelectScoresQuery, connection, transaction))
				using (var scoreReader = selectScoresCmd.ExecuteReader())
				{
					while (scoreReader.Read())
					{
						token.ThrowIfCancellationRequested();
						var score = new DBScoreRow(scoreReader);
						var targetScorehashMD5 = score.CalculateScorehash(targetPlayerPasswordHashMD5);
						if (!DBScoreRow.IsScorehashValid(score, targetPlayerPasswordHashMD5, targetScorehashMD5))
						{
							updateScoreCmd.Parameters["@scorehash"].Value = targetScorehashMD5.Body;
							updateScoreCmd.Parameters["@hash"].Value = score.Hash;
							updateScoreCmd.ExecuteNonQuery();
							Console.WriteLine($"Updating scorehash of a score.\n[Update] Hash : {score.Hash} / Scorehash : {targetScorehashMD5}");
						}
					}
				}

				transaction.Commit();
				Console.WriteLine($"Password updated successfully. Hash : {targetPlayerPasswordHashMD5} / Scorehash : {targetPlayerScorehashMD5}");
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine("Password update cancelled.");
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				Console.WriteLine($"Failed to update password for {id}: {ex.Message}");
				throw;
			}
		}
	}
}
