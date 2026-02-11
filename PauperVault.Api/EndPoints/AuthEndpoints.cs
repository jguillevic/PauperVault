using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PauperVault.Api.Infrastructure.Auth;

namespace PauperVault.Api.Endpoints;

public static class AuthEndpoints
{
	public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapPost("/auth/register", Register);
		app.MapPost("/auth/login", Login);

		return app;
	}

	private static async Task<IResult> Register(
		[FromBody] RegisterRequest request,
		UserManager<ApplicationUser> userManager)
	{
		var user = new ApplicationUser
		{
			UserName = request.Email,
			Email = request.Email
		};

		var result = await userManager.CreateAsync(user, request.Password);

		if (!result.Succeeded)
		{
			return Results.BadRequest(new
			{
				errors = result.Errors.Select(e => new { e.Code, e.Description })
			});
		}

		return Results.Ok(new { message = "User created" });
	}

	private static async Task<IResult> Login(
		[FromBody] LoginRequest request,
		SignInManager<ApplicationUser> signInManager,
		UserManager<ApplicationUser> userManager,
		IConfiguration config)
	{
		var result = await signInManager.PasswordSignInAsync(
			request.Email,
			request.Password,
			isPersistent: false,
			lockoutOnFailure: false);

		if (!result.Succeeded)
			return Results.Unauthorized();

		var user = await userManager.FindByEmailAsync(request.Email);
		if (user is null)
			return Results.Unauthorized();

		var token = JwtTokenGenerator.GenerateToken(user, config);
		return Results.Ok(new { token });
	}
}

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
