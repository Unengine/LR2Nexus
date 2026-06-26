using Avalonia.Data;
using Avalonia.Markup.Xaml;
using LR2Nexus.Services;

namespace LR2Nexus.I18n;

public class TranslateExtension : MarkupExtension
{
	public string Key { get; }
	public TranslateExtension(string key) => Key = key;

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		var binding = new Binding
		{
			Path = $"[{Key}]",
			Source = I18nManager.Instance,
			Mode = BindingMode.OneWay
		};

		return I18nManager.Instance[Key];
	}
}