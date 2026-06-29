using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace LR2Nexus.Model
{
	public class DirectSound : ISoundDriver
	{
		public GameConfig.SoundDriver DriverType => GameConfig.SoundDriver.DirectSound;
		public bool IsSupported { get; } = false;
		public ReadOnlyCollection<string>? Drivers { get; }

		private delegate int DSEnumCallback(IntPtr guid, string description, string module, IntPtr context);
		[DllImport("dsound.dll", CharSet = CharSet.Ansi)]
		private static extern int DirectSoundEnumerate(DSEnumCallback lpDSEnumCallback, IntPtr lpContext);
		private const int DS_OK = 0;

		public DirectSound()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				IsSupported = true;

				var devices = new List<string>();
				var result = DirectSoundEnumerate((
					IntPtr guid,
					[MarshalAs(UnmanagedType.LPStr)] string desc,
					[MarshalAs(UnmanagedType.LPStr)] string mod,
					IntPtr ctx) => {
					devices.Add(desc);
					return 1;
				}, IntPtr.Zero);

				if (result == DS_OK)
				{
					Drivers = devices.AsReadOnly();
				}
			}
		}

		public override string ToString() => DriverType.ToString();
	}
}
