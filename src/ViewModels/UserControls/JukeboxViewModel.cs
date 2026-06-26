using CommunityToolkit.Mvvm.ComponentModel;
using LR2Nexus.Services;
using System.Collections.ObjectModel;

namespace LR2Nexus.ViewModels
{
	public partial class JukeboxViewModel : ObservableObject
	{
		public ObservableCollection<string> AvailablePathes { get; set; } = [];

		public void Update()
		{
			var paths = GameConfigService.Current.Jukebox.Paths;
			foreach (var path in paths)
			{
				AvailablePathes.Add(path);
			}
		}
	}
}
