using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;

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

				using var enumerator = new MMDeviceEnumerator();
				var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
				Drivers = devices.Select(x => x.FriendlyName).ToList().AsReadOnly();
			}
		}

		public override string ToString() => DriverType.ToString();
	}
}
