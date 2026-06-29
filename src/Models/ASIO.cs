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

				List<string> drivers = [];
				using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\ASIO"))
				{
					if (key != null)
					{
						foreach (string subKeyName in key.GetSubKeyNames())
						{
							drivers.Add(subKeyName);
						}
					}
				}

				Drivers = drivers.AsReadOnly();
			}
		}

		public override string ToString() => DriverType.ToString();
	}
}
