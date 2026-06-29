
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LR2Nexus.src.Utils
{
	public static class TextBoxExtension
	{
		public static int FilterIntegerText(TextBox textBox, int maxValue, Func<int, int>? valueCorrectionFunc = null)
		{
			var (value, text) = FilterIntegerStringFromTextBox(textBox);
			value = value > maxValue ? maxValue : value;
			if (valueCorrectionFunc != null) value = valueCorrectionFunc(value);

			if (textBox.Text != text)
			{
				int selectionStart = textBox.SelectionStart;
				textBox.Text = text;
				textBox.SelectionStart = Math.Min(selectionStart, text.Length);
			}

			return value;
		}

		public static void UpdateIntegerTextOnEvent(int value, object? sender, RoutedEventArgs e)
		{
			if (sender is not TextBox textBox) return;

			var text = value.ToString();
			if (textBox.Text != text)
			{
				int selectionStart = textBox.SelectionStart;
				textBox.Text = text;
				textBox.SelectionStart = Math.Min(selectionStart, text.Length);
			}
		}

		private static (int value, string result) FilterIntegerStringFromTextBox(TextBox textBox)
		{
			string filtered = textBox.Text == null ?
				string.Empty :
				new string([.. textBox.Text.Where(char.IsDigit)]);

			if (filtered != string.Empty &&
				int.TryParse(filtered, out var parsed))
			{
				return (parsed, parsed.ToString());
			}

			return (default, string.Empty);
		}
	}
}
