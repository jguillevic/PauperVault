namespace PauperVault.Core.Domain.Auth;

public static class AuthErrors
{
	public const string MissingGoogleIdToken = "Missing idToken";
	public const string MissingEmail = "Email is required.";
	public const string MissingPassword = "Password is required.";
	public const string InvalidCredentials = "Invalid credentials.";
	public const string GoogleTokenHasNoEmail = "Google token has no email.";
	public const string GoogleClientIdNotConfigured = "Google:ClientId not configured.";
}