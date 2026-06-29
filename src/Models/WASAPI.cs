using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace LR2Nexus.Model
{
	public class WASAPI : ISoundDriver
	{
		public GameConfig.SoundDriver DriverType => GameConfig.SoundDriver.WASAPI;
		public bool IsSupported { get; } = false;
		public ReadOnlyCollection<string>? Drivers { get; }

		public WASAPI()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				IsSupported = true;
			}
		}

		public override string ToString() => DriverType.ToString();
	}
}
