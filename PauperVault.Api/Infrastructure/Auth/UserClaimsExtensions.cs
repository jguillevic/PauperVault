using System.Security.Claims;

namespace PauperVault.Api.Infrastructure.Auth;

public static class UserClaimsExtensions
{
	public static string GetUserIdOrThrow(this ClaimsPrincipal user)
	{
		var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrWhiteSpace(userId))
			throw new InvalidOperationException("Missing NameIdentifier claim.");
		return userId;
	}

	public static string? GetUserIdOrNull(this ClaimsPrincipal user)
	{
		var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
		return string.IsNullOrWhiteSpace(userId) ? null : userId;
	}
}