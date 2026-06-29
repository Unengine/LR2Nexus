using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace LR2Nexus.Model
{
	public class ASIO : ISoundDriver
	{
		public GameConfig.SoundDriver DriverType => GameConfig.SoundDriver.ASIO;
		public bool IsSupported { get; } = false;
		public ReadOnlyCollection<string>? Drivers { get; }

		public ASIO()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				IsSupported = true;

				HashSet<string> drivers = [];
				SearchRegistry(RegistryView.Registry64, drivers);
				SearchRegistry(RegistryView.Registry32, drivers);
				Drivers = drivers.ToList().AsReadOnly();
			}
		}

		private void SearchRegistry(RegistryView view, HashSet<string> driverSet)
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

			using RegistryKey? baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
			using RegistryKey? asioKey = baseKey.OpenSubKey(@"SOFTWARE\ASIO");
			if (asioKey != null)
			{
				foreach (string subKeyName in asioKey.GetSubKeyNames())
				{
					driverSet.Add(subKeyName);
				}
			}
		}

		public override string ToString() => DriverType.ToString();
	}
}
