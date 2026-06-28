using Avalonia.Controls;
using LR2Nexus.I18n;
using LR2Nexus.Models;
using LR2Nexus.Utils;
using LR2Nexus.Views;
using Microsoft.Data.Sqlite;
using System.Numerics;
using Tmds.DBus.Protocol;

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
				isHashLegit = hash != null && hash == MD5Util.CalculateMD5(password);
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
			string? idHash, playerScorehash;
			using var selectPlayerCmd = new SqliteCommand(SelectPlayerQuery, connection);
			using (var reader = selectPlayerCmd.ExecuteReader())
			{
				if (!reader.Read()) throw new Exception("Player data not found.");

				player = new DBPlayerRow(reader);
				idHash = player.Hash;
				playerScorehash = player.Scorehash;
			}

			var targetPlayerIdHash = MD5Util.CalculateMD5(targetPassword);
			var targetPlayerScorehash = player.CalculateScorehash();

			if (idHash == targetPlayerIdHash && playerScorehash == targetPlayerScorehash) return;
			using var transaction = connection.BeginTransaction();
			try
			{
				using (var updatePlayerCmd = new SqliteCommand(UpdatePlayerHashQuery, connection, transaction))
				{
					updatePlayerCmd.Parameters.AddWithValue("@hash", targetPlayerIdHash);
					updatePlayerCmd.Parameters.AddWithValue("@scorehash", targetPlayerScorehash);
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
						var scorehash = score.CalculateScorehash(targetPassword);

						updateScoreCmd.Parameters["@scorehash"].Value = scorehash;
						updateScoreCmd.Parameters["@hash"].Value = score.Hash;
						updateScoreCmd.ExecuteNonQuery();
						Console.WriteLine($"Updating scorehash of a score. Hash : {score.Hash} / Scorehash : {scorehash}");
					}
				}

				transaction.Commit();
				Console.WriteLine($"Password updated successfully. Hash : {targetPlayerIdHash} / Scorehash : {targetPlayerScorehash}");
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
