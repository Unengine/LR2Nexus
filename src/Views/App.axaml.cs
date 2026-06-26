using Avalonia;
using Avalonia.Markup.Xaml;
using LR2Nexus.I18n;
using LR2Nexus.Services;
using System.Text;
namespace LR2Nexus.Views;

internal partial class App : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		InitializeServices();

		if (ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.MainWindow = new MainWindow();
		}
		base.OnFrameworkInitializationCompleted();
	}

	private void InitializeServices()
	{
		LauncherSettingManager.Initialize();
		if (LauncherSettingManager.Current.Language == I18nManager.Language.Default)
		{
			LauncherSettingManager.Current.Language = I18nManager.Instance.GetCurrentLocaleLanguage();
		}
		I18nManager.Instance.ChangeLanguage(LauncherSettingManager.Current.Language);
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		GameConfigService.LoadGameConfig();
	}
}