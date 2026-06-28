using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LR2Nexus.I18n;
using LR2Nexus.Utils;
using LR2Nexus.ViewModels;

namespace LR2Nexus.Views;

public partial class LoadingWindow : Window
{
	private readonly LoadingWindowViewModel _viewModel = new();

	public LoadingWindow()
	{
		InitializeComponent();
		DataContext = _viewModel;
	}

	private void OnConfirmClick(object sender, RoutedEventArgs e)
	{
		Close(true);
	}

	public static async Task<bool?> PromptWithTaskAsync(Window owner,
		Task task, CancellationTokenSource cts, bool cancellable,
		string title, params string[] messages)
	{
		var window = SetupWindow(owner, title, messages);
		window.Closed += (_, _) => cts.Cancel();
		if (!cancellable)
		{
			window.Closing += (_, e) =>
			{
				if (!task.IsCompleted)
				{
					e.Cancel = true;
				}
			};

			window.WindowDecorations = WindowDecorations.None;
			window.ShowInTaskbar = false;
		}

		_ = task.ContinueWith(async t =>
		{
			await Dispatcher.UIThread.InvokeAsync(async () =>
			{
				await AlertWindow.PromptAsync(owner,
					I18nManager.Instance["Error"],
					I18nManager.Instance["PasswordResetFailedContent"]);
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

	private static LoadingWindow SetupWindow(Window owner, string title, params string[] messages)
	{
		var width = owner.Width;
		var height = owner.Height;
		var win = new LoadingWindow
		{
			Title = title,
			Width = width,
			MinHeight = height,
			Position = owner.Position
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