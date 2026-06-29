using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LR2Nexus.Utils;
using LR2Nexus.ViewModel;

namespace LR2Nexus.View;

public partial class AlertWindow : Window
{
	private readonly AlertWindowViewModel _viewModel = new();

	public AlertWindow()
	{
		InitializeComponent();
		DataContext = _viewModel;
	}

	private void OnConfirmClick(object sender, RoutedEventArgs e)
	{
		Close(true);
	}

	public static async Task<bool?> PromptWithTaskAsync(Window owner,
		Task task, CancellationTokenSource cts,
		string title, params string[] messages)
	{
		var window = SetupWindow(owner, title, messages);	
		window.Closed += (_, _) => cts.Cancel();

		_ = task.ContinueWith(async t =>
		{
			await Dispatcher.UIThread.InvokeAsync(() =>
			{
				if (window.IsVisible) window.Close();
			});
		}, TaskScheduler.Default);

		return await window.ShowDialog<bool>(owner);
	}

	public static async Task<bool?> PromptAsync(Window owner, string title, params string[] messages)
	{
		var window = SetupWindow(owner, title, messages);
		return await window.ShowDialog<bool>(owner);
	}

	private static AlertWindow SetupWindow(Window owner, string title, params string[] messages)
	{
		var width = owner.Width * 0.3 / owner.RenderScaling;
		var height = owner.Height * 0.3 / owner.RenderScaling;
		var win = new AlertWindow()
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
		return win;
	}
}