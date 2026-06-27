
using Avalonia.Controls;

namespace LR2Nexus.src.Utils
{
	public static class TextBoxExtension
	{
		public static int FilterIntegerText(TextBox textBox, int minValue, int maxValue)
		{
			string filtered = textBox.Text == null ?
				string.Empty :
				new string([.. textBox.Text.Where(char.IsDigit)]);

			if (filtered != string.Empty &&
				int.TryParse(filtered, out var parsed))
			{
				parsed = int.Clamp(parsed, minValue, maxValue);
				filtered = parsed.ToString();
			}

			if (textBox.Text != filtered)
			{
				int selectionStart = textBox.SelectionStart;
				textBox.Text = filtered;
				textBox.SelectionStart = Math.Min(selectionStart, filtered.Length);
			}

			return int.TryParse(textBox.Text, out var value) ? value : default;
		}
	}
}
