using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using LR2Nexus.I18n;
using LR2Nexus.Services;
using LR2Nexus.ViewModels;

namespace LR2Nexus.Views;

public partial class Jukebox : UserControl
{
	private JukeboxViewModel _viewModel = new();

	public Jukebox()
	{
		InitializeComponent();

		_viewModel.Update();
		DataContext = _viewModel;
	}

	private async void OnAddPathClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		var topLevel = TopLevel.GetTopLevel(this);

		var folders = await topLevel!.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
		{
			Title = I18nManager.Instance["AddBMSPathTitle"],
			AllowMultiple = true,
		});

		if (topLevel is Window window)
		{
			if (folders.Count > 0)
			{
				var duplicatedCount = 0;
				foreach (var folder in folders)
				{
					var path = Path.GetFullPath(folder.Path.LocalPath);

					if (!_viewModel.AvailablePathes.Contains(path))
					{
						if (GameConfigService.AddJukeboxPath(path))
						{
							_viewModel.AvailablePathes.Add(path);
						}
						else
						{
							duplicatedCount++;
						}
					}
					else
					{
						duplicatedCount++;
					}
				}

				var message = duplicatedCount == 0 ?
					string.Format(I18nManager.Instance["AddBMSPathContentFormat"], folders.Count) :
					string.Format(I18nManager.Instance["AddBMSPathContentWithExcludesFormat"],
					folders.Count - duplicatedCount, duplicatedCount);

				await AlertWindow.PromptAsync(window, I18nManager.Instance["AddBMSPathTitle"], message);
			}
		}
	}

	private async void OnDeletePathClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		if (PathListBox.SelectedItem is not string path) return;

		var topLevel = TopLevel.GetTopLevel(this);
		if (topLevel is not Window window) return;

		var result = await ConfirmWindow.PromptAsync(
			window,
			I18nManager.Instance["DeleteBMSPathTitle"],
			I18nManager.Instance["DeleteBMSPathContent"],
			$"<b>{path}</b>") ?? false;

		if (result)
		{
			GameConfigService.RemoveJukeboxPath(path);
			_viewModel.AvailablePathes.Remove(path);
		}
	}


	private void OnItemDropped(object? sender, DragEventArgs e)
	{
		if (!e.DataTransfer.Contains(DataFormat.File)) return;

		var items = e.DataTransfer.TryGetFiles();

		if (items == null) return;

		foreach (var item in items)
		{
			if (item is IStorageFolder folder)
			{
				string path = folder.Path.LocalPath;
				if (GameConfigService.AddJukeboxPath(path))
				{
					_viewModel.AvailablePathes.Add(path);
				}
			}
		}

		AddFolderIcon.Opacity = 0;
		PathListBox.Background = Brush.Parse("#10000000");
	}

	private void OnDragOver(object? sender, Avalonia.Input.DragEventArgs e)
	{
		AddFolderIcon.Opacity = 0.7;
		PathListBox.Background = Brush.Parse("#25000000");
	}

	private void OnDragLeave(object? sender, DragEventArgs e)
	{
		AddFolderIcon.Opacity = 0;
		PathListBox.Background = Brush.Parse("#10000000");
	}
}