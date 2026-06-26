using Avalonia;
using LR2Nexus.I18n;
using LR2Nexus.Services;
using LR2Nexus.Views;
using System.Text;

namespace LR2Nexus;

class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
	}

	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace();
}
