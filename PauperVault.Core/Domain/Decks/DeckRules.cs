namespace PauperVault.Core.Domain.Decks;

public static class DeckRules
{
	public static bool IsValidName(string? name)
		=> !string.IsNullOrWhiteSpace(name) && name.Trim().Length >= 2;

	public static bool IsValidQuantity(int quantity)
		=> quantity >= 1;
}