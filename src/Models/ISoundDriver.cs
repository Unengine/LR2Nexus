using System.Collections.ObjectModel;

namespace LR2Nexus.Model
{
	public interface ISoundDriver
	{
		public GameConfig.SoundDriver DriverType { get; }
		public bool IsSupported { get; }
		public ReadOnlyCollection<string>? Drivers { get; }
		public string ToString();
	}
}
