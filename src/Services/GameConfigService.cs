using Avalonia.Controls;
using LR2Nexus.Models;
using LR2Nexus.Utils;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LR2Nexus.Services
{	
	public static class GameConfigService
	{
		public static GameConfig Current { get; private set; } = new();

		public static bool AddJukeboxPath(string path)
		{
			if (!Current.Jukebox.Paths.Contains(path))
			{
				Current.Jukebox.Paths.Add(path);
				SaveGameConfig();
				return true;
			}
			return false;
		}

		public static bool RemoveJukeboxPath(string path)
		{
			bool result = Current.Jukebox.Paths.Remove(path);
			if (result) SaveGameConfig();
			return result;
		}

		public static void SaveGameConfig()
		{
			var path = LauncherSettingManager.GetLR2ConfigPath();
			XmlReflectionEngine.Serialize(Current, path);
		}

		public static void LoadGameConfig()
		{
			var path = LauncherSettingManager.GetLR2ConfigPath();
			if (!File.Exists(path))
			{
				string resourceName = "LR2Nexus.src.Assets.configTemplate.xml";
				var assembly = System.Reflection.Assembly.GetExecutingAssembly();

				using Stream? stream = assembly.GetManifestResourceStream(resourceName)
					?? throw new FileNotFoundException($"Cannot found embedded build resource : {resourceName}");
				string? directory = Path.GetDirectoryName(path);
				if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				using FileStream fileStream = File.Create(path);
				stream.CopyTo(fileStream);
			}

			try
			{
				var doc = XDocument.Load(path);
				var newConfig = new GameConfig();
				XmlReflectionEngine.Deserialize(doc.Root!, newConfig);

				Current = newConfig;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed loading config.xml, using default config. : {ex.Message}");
				Current = new GameConfig();
			}
		}
	}
}
