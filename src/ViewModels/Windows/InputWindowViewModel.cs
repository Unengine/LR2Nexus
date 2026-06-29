using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.RegularExpressions;


namespace LR2Nexus.ViewModel;

public partial class InputWindowViewModel() : ObservableObject
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsValid))]
	private string? _inputValue;

	private Regex? _regex;
	public string? FilterRegex
	{
		get => _regex?.ToString();
		set => _regex = value != null ? new Regex(value, RegexOptions.Compiled) : null;
	}

	public bool IsValid => _regex == null || _regex.IsMatch(InputValue ?? string.Empty);
}
