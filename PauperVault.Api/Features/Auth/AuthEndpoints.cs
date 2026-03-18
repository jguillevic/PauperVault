namespace PauperVault.Api.Features.Auth;

public static class AuthEndpoints
{
	public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapPost("/auth/register", AuthHandlers.RegisterAsync);
		app.MapPost("/auth/login", AuthHandlers.LoginAsync);
		app.MapPost("/auth/google", AuthHandlers.GoogleLoginAsync);
		app.MapGet("/auth/me", AuthHandlers.GetMe).RequireAuthorization();

		return app;
	}
}