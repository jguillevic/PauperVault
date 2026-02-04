using PauperVault.Web.Features.Auth;

namespace PauperVault.Web.Endpoints;

public static class AuthEndpoints
{
	public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/api/auth").WithTags("Auth");

		group.MapGet("/csrf", (HttpContext ctx, IAuthService auth) =>
		{
			var token = auth.CreateCsrfToken(ctx);
			return Results.Ok(new { csrfToken = token });
		});

		group.MapPost("/sessionLogin", async (HttpContext ctx, IAuthService auth, SessionLoginRequest req) =>
		{
			var result = await auth.SessionLoginAsync(ctx, req);
			return result.Ok ? Results.Ok(new { ok = true }) : Results.BadRequest(new { error = result.Error });
		});

		group.MapPost("/sessionLogout", async (HttpContext ctx, IAuthService auth) =>
		{
			await auth.SessionLogoutAsync(ctx);
			return Results.Ok(new { ok = true });
		});

		return app;
	}
}
