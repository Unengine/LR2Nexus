using LR2Nexus.Models;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LR2Nexus.Services
{
	internal static class LauncherSettingManager
	{
		private static readonly string SettingsFilePath =
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LR2NexusSettings.json");

		private static readonly string DefaultLR2Path =
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LR2Body.exe");

		public static readonly string ProfileNamePattern = @"^[a-zA-Z0-9_\-\.]{1,255}$";
		private static readonly Regex ProfileNameRegex = new(ProfileNamePattern, RegexOptions.Compiled);

		// numbers, alphabets, special characters. (3~20 letters)
		public static readonly string PasswordPattern = @"^[a-zA-Z0-9!@#$%^&*()_+=[\]{}|\\:;""'<>,.?/`~-]{3,20}$";
		private static readonly Regex PasswordRegex = new(PasswordPattern, RegexOptions.Compiled);

		public static AppSettings Current { get; private set; } = new();

		private static readonly object _locker = new();

		public static void Initialize()
		{
			if (File.Exists(SettingsFilePath))
			{
				try
				{
					string json = File.ReadAllText(SettingsFilePath);
					Current = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
					Console.WriteLine($"Successfully read app settings.");
				}
				catch (Exception ex)
				{
					Current = new AppSettings();
					SaveAppSettings();
					Console.WriteLine($"Failed to read app settings : {ex.Message}\nLaunching with default setting values.");
				}
			}
			else
			{
				Console.WriteLine($"App settings not found. Launching with default setting values.");
				Current = new AppSettings();
				SaveAppSettings();
			}

			if (Current.Lr2Path == null)
			{
				if (File.Exists(DefaultLR2Path))
				{
					SetLR2BodyPath(DefaultLR2Path);
				}
			}

			ScanPlayersInDB();

			ProcessService.LR2BodyPath = Current.Lr2Path;
		}

		public static void SetLR2BodyPath(string path)
		{
			ProcessService.LR2BodyPath = path;
			Console.WriteLine($"Updated LR2Body path : {path}");

			Current.Lr2Path = path;
			GameConfigService.LoadGameConfig();
			ScanPlayersInDB();
		}

		public static void ScanPlayersInDB()
		{
			if (string.IsNullOrEmpty(Current.Lr2Path) ||
				!File.Exists(Current.Lr2Path)) return;

			try
			{
				var parent = Directory.GetParent(Current.Lr2Path)?.FullName;
				var playerDBDir = Path.Combine(parent!, "LR2Files", "Database", "Score");
				if (Directory.Exists(playerDBDir))
				{
					var playerIds = Directory.GetFiles(playerDBDir, "*.db")
						.Select(path => Path.GetFileNameWithoutExtension(path));

					if (playerIds.Any())
					{
						var playerName = playerIds.First();
						if (Current.RecentPlayerName != playerName)
						{
							Current.RecentPlayerName = playerName;
						}

						foreach (var id in playerIds)
						{
							if (!Current.Players.ContainsKey(id))
							{
								Current.Players[id] = null;
							}
						}
					}
					else
					{
						Current.RecentPlayerName = null;
					}
				}
				else
				{
					Current.RecentPlayerName = null;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error scanning player DB: {ex.Message}");
			}

			var orphanPlayers =
				Current.Players.Keys.Where(key => !HasPlayerDB(key)).ToList();
			foreach (var id in orphanPlayers)
			{
				Current.Players.Remove(id);
			}

			SaveAppSettings();
		}

		public static string GetPlayerDBPath(string? playerName)
		{
			if (string.IsNullOrEmpty(Current.Lr2Path) ||
				string.IsNullOrEmpty(playerName))
			{
				return string.Empty;
			}

			var parent = Directory.GetParent(Current.Lr2Path)?.FullName;
			return Path.Combine(parent!, "LR2Files", "Database", "Score", $"{playerName}.db");
		}

		public static string GetLR2ConfigPath()
		{
			if (string.IsNullOrEmpty(Current.Lr2Path))
			{
				return string.Empty;
			}

			var parent = Directory.GetParent(Current.Lr2Path)?.FullName;
			return Path.Combine(parent!, "LR2Files", "Config", "config.xml");
		}

		public static bool TryCreateNewPlayer(string id, string password)
		{
			lock (_locker)
			{
				var dbPath = GetPlayerDBPath(id);

				try
				{
					Console.WriteLine($"Creating new player : {id}");
					if (HasPlayerDB(id) ||
						!IsNameLegit(id) ||
						!IsPasswordLegit(password)) return false;

					var parentDir = Path.GetDirectoryName(dbPath);
					if (!string.IsNullOrEmpty(parentDir))
					{
						Directory.CreateDirectory(parentDir);
					}

					var assembly = System.Reflection.Assembly.GetExecutingAssembly();
					string resourceName = "LR2Nexus.src.Assets.playerdbTemplate.db";

					using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
					using (FileStream fileStream = File.Create(dbPath))
					{
						stream?.CopyTo(fileStream);
					}

					if (!HasPlayerDB(id)) return false;

					using (var connection = new SqliteConnection($"Data Source={dbPath};Mode=ReadWrite;Pooling=False;"))
					{
						connection.Open();
						using var selectCmd = new SqliteCommand("SELECT id, name FROM player LIMIT 1", connection);
						var currentHash = selectCmd.ExecuteScalar()?.ToString();
						using var updateCmd = new SqliteCommand("UPDATE player SET id = @id, name = @name", connection);
						updateCmd.Parameters.AddWithValue("@id", id);
						updateCmd.Parameters.AddWithValue("@name", id);
						updateCmd.ExecuteNonQuery();
					}

					Current.Players[id] = password;
					Current.RecentPlayerName = id;
					SaveAppSettings();
					return true;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Critical Error: Failed to create {id}.db file: {ex.Message}");
					if (File.Exists(dbPath)) File.Delete(dbPath);
					return false;
				}
			}
		}

		public static bool TryDeletePlayer(string id)
		{
			try
			{
				Console.WriteLine($"Delete player : {id}");
				var dbPath = GetPlayerDBPath(id);

				if (File.Exists(dbPath))
				{
					File.Delete(dbPath);
				}

				if (File.Exists(dbPath)) return false;

				Current.Players.Remove(id);

				var nextPlayer = Current.Players.Keys.FirstOrDefault(HasPlayerDB);
				Current.RecentPlayerName = nextPlayer;

				SaveAppSettings();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error deleting player {id}: {ex.Message}");
				return false;
			}
		}

		public static void SaveAppSettings()
		{
			try
			{
				string json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
				File.WriteAllText(SettingsFilePath, json);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to save app settings : {ex.Message}");
			}

			Console.WriteLine($"Successfully saved app settings.");
		}

		public static bool HasPlayerDB(string? id)
		{
			if (string.IsNullOrEmpty(Current.Lr2Path) ||
				string.IsNullOrEmpty(id))
			{
				return false;
			}

			var parent = Directory.GetParent(Current.Lr2Path)?.FullName;
			var playerDBDir = Path.Combine(parent ?? string.Empty,
				"LR2Files", "Database", "Score", $"{id}.db");
			return File.Exists(playerDBDir);
		}

		public static bool TryChangeCurrentProfile(string id)
		{
			if (!Current.Players.ContainsKey(id))
			{
				return false;
			}

			Current.RecentPlayerName = id;
			SaveAppSettings();
			return true;
		}


		public static bool IsNameLegit(string? name) => name != null && ProfileNameRegex.IsMatch(name);
		public static bool IsPasswordLegit(string? password) => password != null && PasswordRegex.IsMatch(password);
	}
}
