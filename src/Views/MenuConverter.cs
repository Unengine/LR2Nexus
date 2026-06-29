using Avalonia.Controls;
using Avalonia.Data.Converters;
using LR2Nexus.I18n;
using System.Globalization;

namespace LR2Nexus.View;

public class MenuConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) 
	{
		return value switch
		{
			"Home" => new Home(),
			"Jukebox" => new Jukebox(),
			"PlayOption" => new PlayOption(),
			"SystemOption" => new SystemOption(),
			_ => new TextBlock {
				Text = I18nManager.Instance["WIP"],
				FontSize = 50,
				Margin = new(50),
				HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
				VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch }
		};
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}