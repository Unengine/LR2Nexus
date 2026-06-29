using Avalonia.Controls;
using Avalonia.Interactivity;
using LR2Nexus.Utils;
using LR2Nexus.ViewModel;

namespace LR2Nexus.View;

public partial class ConfirmWindow : Window
{
	private readonly ConfirmWindowViewModel _viewModel = new();

	public ConfirmWindow()
	{
		InitializeComponent();
		DataContext = _viewModel;
	}

	private void OnConfirmClick(object sender, RoutedEventArgs e)
	{
		Close(true);
	}

	private void OnCancelClick(object sender, RoutedEventArgs e)
	{
		Close(false);
	}

	public static async Task<bool?> PromptAsync(Window owner, string title, params string[] messages)
	{
		var width = owner.Width * 0.3;
		var height = owner.Height * 0.3;
		var win = new ConfirmWindow()
		{
			Title = title,
			Width = width,
			MinHeight = height
		};

		var textBlock = new TextBlock()
		{
			TextWrapping = Avalonia.Media.TextWrapping.Wrap,
			TextAlignment = Avalonia.Media.TextAlignment.Center,
			VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
			HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
			LineSpacing = 2,
		};
		InlineTextParser.AppendFormattedText(textBlock.Inlines!, messages);

		win.FindControl<ContentControl>("MessageContainer")!.Content = textBlock;
		return await win.ShowDialog<bool>(owner);
	}
}