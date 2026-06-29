using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using LR2Nexus.Utils;
using LR2Nexus.ViewModel;
using System.Text.RegularExpressions;

namespace LR2Nexus.View;

public partial class InputWindow : Window
{
	private readonly InputWindowViewModel _viewModel = new();

	public string? FilterRegex
	{
		get => _viewModel.FilterRegex;
		set => _viewModel.FilterRegex = value;
	}

	public string? InputResult { get; private set; }

	public InputWindow()
	{
		InitializeComponent();
		DataContext = _viewModel;
	}

	private void OnConfirmClick(object sender, RoutedEventArgs e)
	{
		if (_viewModel.IsValid)
		{
			InputResult = _viewModel.InputValue;
			Close(true);
		}
		else
		{
			// TODO : Error feedback
		}
	}

	public static async Task<string?> PromptWithTargetAsync(Window owner, string title, string target, params string[] messages)
	{
		var regex = $"^{Regex.Escape(target)}$";
		return await PromptWithRegexAsync(owner, title, regex, messages);
	}

	public static async Task<string?> PromptWithRegexAsync(Window owner, string title, string? regex, params string[] messages)
	{
		var renderScaling = owner.RenderScaling;
		var width = owner.Width * 0.3 / owner.RenderScaling;
		var height = owner.Height * 0.3 / owner.RenderScaling;
		var win = new InputWindow()
		{
			Title = title,
			Width = width,
			MinHeight = height,
			FilterRegex = regex,
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
		var confirmed = await win.ShowDialog<bool>(owner);
		return confirmed ? win.InputResult : null;
	}
}