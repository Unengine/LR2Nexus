using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace LR2Nexus.Utils
{
	public static class InlineTextParser
	{
		private static readonly Dictionary<string, Action<InlineCollection, string>> TagHandlers = new()
		{
			{
				"<b>", (inlines, text) => inlines.Add(new Run(text) { FontWeight = FontWeight.Bold })
			},
			{
				"<warn>", (inlines, text) =>
				{
					var run = new Run(text) { FontWeight = FontWeight.Bold, Foreground = Brushes.OrangeRed };
					run.FontSize += 4;
					inlines.Add(run);
				}
			},
		};

		public static void AppendFormattedText(InlineCollection inlines, params string[] messages)
		{
			foreach (var message in messages)
			{
				bool handled = false;

				foreach (var handler in TagHandlers)
				{
					string openTag = handler.Key;
					string closeTag = openTag.Replace("<", "</");

					if (message.StartsWith(openTag) && message.EndsWith(closeTag))
					{
						string content = message.Substring(openTag.Length, message.Length - openTag.Length - closeTag.Length);
						handler.Value(inlines, content);
						handled = true;
						break;
					}
				}

				if (!handled)
				{
					inlines.Add(new Run(message));
				}

				inlines.Add(new LineBreak());
			}
		}
	}
}