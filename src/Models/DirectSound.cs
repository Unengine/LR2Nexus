using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace LR2Nexus.Model
{
	public class DirectSound : ISoundDriver
	{
		public GameConfig.SoundDriver DriverType => GameConfig.SoundDriver.DirectSound;
		public bool IsSupported { get; } = false;
		public ReadOnlyCollection<string>? Drivers { get; }

		public DirectSound()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				IsSupported = true;
			}
		}

		public override string ToString() => DriverType.ToString();
	}
}
