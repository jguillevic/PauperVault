namespace PauperVault.Core.Domain.Cards;

public static class CardRules
{
	public const int MinSearchLength = 2;

	public static string NormalizeSearch(string? input)
		=> (input ?? string.Empty).Trim();

	public static bool IsValidSearch(string? input)
		=> NormalizeSearch(input).Length >= MinSearchLength;

	public static string? GetPauperLegality(IDictionary<string, string>? legalities)
	{
		if (legalities is null)
			return null;

		return legalities.TryGetValue("pauper", out var value)
			? value
			: null;
	}

	public static string? GetSmallImageUrl(IDictionary<string, string>? imageUris)
	{
		if (imageUris is null)
			return null;

		return imageUris.TryGetValue("small", out var value)
			? value
			: null;
	}
}