using Avalonia;
using Avalonia.Markup.Xaml;
using LR2Nexus.I18n;
using LR2Nexus.Model;
using LR2Nexus.Services;
using System.Collections.Immutable;
using System.Text;

namespace LR2Nexus.View;

internal partial class App : Application
{
	public ImmutableList<ISoundDriver>? SoundDrivers { get; private set; }

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

		SoundDrivers = [new DirectSound(), new WASAPI(), new ASIO()];
	}
}