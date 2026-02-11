using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace PauperVault.Api.Endpoints;

public static class UserEndpoints
{
	public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapGet("/auth/me", GetMe).RequireAuthorization();

		return app;
	}
	private static IResult GetMe(ClaimsPrincipal user)
	{
		var (userId, email) = ExtractUserInfo(user);
		return Results.Ok(new { userId, email });
	}

	private static (string? UserId, string? Email) ExtractUserInfo(ClaimsPrincipal user)
	{
		var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

		var email = user.FindFirstValue(ClaimTypes.Email)
				 ?? user.FindFirstValue(JwtRegisteredClaimNames.Email)
				 ?? user.FindFirstValue("email");

		return (userId, email);
	}
}
