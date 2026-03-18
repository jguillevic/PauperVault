namespace PauperVault.Core.Domain.Auth;

public static class AuthRules
{
	public static string NormalizeEmail(string? email)
		=> (email ?? string.Empty).Trim().ToLowerInvariant();

	public static bool HasEmail(string? email)
		=> !string.IsNullOrWhiteSpace(NormalizeEmail(email));

	public static bool HasPassword(string? password)
		=> !string.IsNullOrWhiteSpace(password);

	public static bool HasGoogleIdToken(string? idToken)
		=> !string.IsNullOrWhiteSpace(idToken);
}