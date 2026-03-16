using System.Text.RegularExpressions;

namespace PauperVault.Web.Infrastructure.Mana;

public static class ManaCostRenderer
{
	private static readonly Regex ManaRegex = new(@"\{([^}]+)\}");

	public static string ToHtml(string? manaCost)
	{
		if (string.IsNullOrWhiteSpace(manaCost))
			return string.Empty;

		return ManaRegex.Replace(manaCost, m =>
		{
			var symbol = m.Groups[1].Value.ToLower();
			return $"<i class=\"ms ms-{symbol}\"></i>";
		});
	}
}