using LR2Nexus.Services;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace LR2Nexus.I18n;

public class I18nManager : INotifyPropertyChanged
{
	public static I18nManager Instance { get; } = new();
	private readonly Dictionary<string, string> _translations = [];

	private readonly Dictionary<Language, LanguageOption> _languageOptionDict = new()
	{
		{ Language.English, new("English", Language.English) },
		{ Language.Japanese, new("日本語", Language.Japanese) },
		{ Language.Korean, new("한국어", Language.Korean) },
	};

	private readonly Dictionary<string, Language> _localeMap = new()
	{
		{ "en-US", Language.English },
		{ "ja-JP", Language.Japanese },
		{ "ko-KR", Language.Korean },
	};

	public event PropertyChangedEventHandler? PropertyChanged;

	public record LanguageOption(string DisplayName, Language Language);

	public string this[string key] =>
		_translations.TryGetValue(key, out var val) &&
		!string.IsNullOrWhiteSpace(val) ? val : $"!!{key}!!";

	public LanguageOption GetLanguageOption(Language language) =>
		_languageOptionDict.TryGetValue(language, out var opt) ? opt : _languageOptionDict[Language.English];

	public void ChangeLanguage(Language language)
	{
		LoadCsv(language);
		LauncherSettingManager.Current.Language = language;
		LauncherSettingManager.SaveAppSettings();

		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
	}

	public Language GetCurrentLocaleLanguage()
	{
		string cultureName = CultureInfo.CurrentCulture.Name;
		return _localeMap.GetValueOrDefault(cultureName, Language.English);
	}

	private void LoadCsv(Language language)
	{
		string baseDir = AppDomain.CurrentDomain.BaseDirectory;
		string filePath = Path.Combine(baseDir, "Assets", "localization.csv");

		if (!File.Exists(filePath))
		{
			Debug.WriteLine($"[Error] 파일을 찾을 수 없습니다: {filePath}");
			return;
		}

		try
		{
			using var parser = new TextFieldParser(filePath);
			parser.TextFieldType = FieldType.Delimited;
			parser.SetDelimiters(",");
			parser.HasFieldsEnclosedInQuotes = true;

			string[]? headers = parser.ReadFields();
			if (headers == null || headers.Length == 0) return;

			int langIndex = Array.IndexOf(headers, language.ToString());
			if (langIndex == -1)
			{
				Console.WriteLine($"Lauguage '{language}' not found in localization.csv");
				return;
			}

			_translations.Clear();
			while (!parser.EndOfData)
			{
				string[]? fields = parser.ReadFields();
				if (fields == null || fields.Length == 0) continue;

				string key = fields[0];
				if (string.IsNullOrWhiteSpace(key)) continue;

				string value = (langIndex < fields.Length) ? fields[langIndex] : $"!!{key}!!";
				_translations[key] = value;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error parsing localization data : {ex.Message}");
		}
	}

	public enum Language
	{
		Default,
		English,
		Korean,
		Japanese
	}
}