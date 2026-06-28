using LR2Nexus.Utils;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Linq;

namespace LR2Nexus.Services
{
	public static class GameProcessService
	{
		public static string? LR2BodyPath { get; set; }

		public static bool IsLR2Running
		{
			get
			{
				if (string.IsNullOrEmpty(LR2BodyPath) || !File.Exists(LR2BodyPath))
					return false;

				string fileName = Path.GetFileNameWithoutExtension(LR2BodyPath);
				var processes = Process.GetProcessesByName(fileName);

				return processes.Any(p =>
				{
					try
					{
						return p.MainModule?.FileName != null &&
							   Path.GetFullPath(p.MainModule.FileName).Equals(
								   Path.GetFullPath(LR2BodyPath), StringComparison.OrdinalIgnoreCase);
					}
					catch (Win32Exception)
					{
						return false;
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Unexpected error checking process: {ex.Message}");
						return false;
					}
				});
			}
		}

		public static event EventHandler? LR2Exited;

		public static void LaunchLR2Body(string id)
		{
			try
			{
				if (!LauncherSettingManager.Current.Players.TryGetValue(id, out var password) ||
					password == null)
				{
					throw new Exception("Password not set.");
				}

				UpdateLR2Config(id, password);

				var startInfo = new ProcessStartInfo
				{
					FileName = LR2BodyPath,
					UseShellExecute = true,
					WorkingDirectory = Path.GetDirectoryName(LR2BodyPath)
				};

				var process = new Process
				{
					StartInfo = startInfo,
					EnableRaisingEvents = true
				};

				process.Exited += (s, e) =>
				{
					var exitCode = process.ExitCode;
					Console.WriteLine($"Process exited with code: {exitCode}");
					LR2Exited?.Invoke(null, EventArgs.Empty);
					process.Dispose();
				};

				process.Start();

				Console.WriteLine($"Successfully launched process: {LR2BodyPath}");
			}
			catch
			{
				throw;
			}
		}

		public static void UpdateLR2Config(string id, string password)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new Exception("Player ID/password not assigned.");
			}

			try
			{
				var configPath = LauncherSettingManager.GetLR2ConfigPath();
				XDocument doc = XDocument.Load(configPath);
				var playerElement = doc.Root?.Element("player");

				var isDirty = false;
				var idElement = playerElement?.Element("id");
				if (idElement != null)
				{
					idElement.Value = id;
					isDirty = true;
				}

				var passElement = playerElement?.Element("pass");
				if (passElement != null)
				{
					passElement.Value = password;
					isDirty = true;
				}

				if (isDirty)
				{
					doc.Save(configPath);
				}

				GameConfigService.SaveGameConfig();
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
