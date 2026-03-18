using Google.Apis.Auth;
using PauperVault.Core.Domain.Auth;

namespace PauperVault.Api.Features.Auth;

public static class AuthQueries
{
	public static async Task<(GoogleIdentity? Identity, AuthCommandResult? Error)> ValidateGoogleTokenAsync(
		string idToken,
		IConfiguration config)
	{
		var googleClientId = config["Google:ClientId"];
		if (string.IsNullOrWhiteSpace(googleClientId))
		{
			return (null, new AuthCommandResult.ConfigurationError(
				AuthErrors.GoogleClientIdNotConfigured));
		}

		GoogleJsonWebSignature.Payload payload;
		try
		{
			payload = await GoogleJsonWebSignature.ValidateAsync(
				idToken,
				new GoogleJsonWebSignature.ValidationSettings
				{
					Audience = new[] { googleClientId }
				});
		}
		catch
		{
			return (null, new AuthCommandResult.Unauthorized());
		}

		var email = AuthRules.NormalizeEmail(payload.Email);

		if (!AuthRules.HasEmail(email))
		{
			return (null, new AuthCommandResult.ValidationError(new[]
			{
				new AuthErrorItem("google_email_missing", AuthErrors.GoogleTokenHasNoEmail)
			}));
		}

		return (new GoogleIdentity(email), null);
	}
}