
namespace LR2Nexus.Models
{
	public class MonitorInfo
	{
		public required string DisplayName { get; set; }
		public required int Index { get; set; }
		public bool IsPrimary { get; set; }
		public int ResolutionX { get; set; }
		public int ResolutionY { get; set; }

		public override string ToString() => DisplayName;
	}
}
