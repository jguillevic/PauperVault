using Google.Apis.Auth;
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
		app.MapPost("/auth/google", GoogleLogin);

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

	private static async Task<IResult> GoogleLogin(
		[FromBody] GoogleLoginRequest request,
		UserManager<ApplicationUser> userManager,
		IConfiguration config)
	{
		if (string.IsNullOrWhiteSpace(request.IdToken))
			return Results.BadRequest(new { error = "Missing idToken" });

		var googleClientId = config["Google:ClientId"];
		if (string.IsNullOrWhiteSpace(googleClientId))
			return Results.Problem("Google:ClientId not configured.");

		GoogleJsonWebSignature.Payload payload;
		try
		{
			payload = await GoogleJsonWebSignature.ValidateAsync(
				request.IdToken,
				new GoogleJsonWebSignature.ValidationSettings
				{
					Audience = new[] { googleClientId }
				});
		}
		catch
		{
			return Results.Unauthorized();
		}

		var email = payload.Email;
		if (string.IsNullOrWhiteSpace(email))
			return Results.BadRequest(new { error = "Google token has no email." });

		// Trouver ou créer l'utilisateur
		var user = await userManager.FindByEmailAsync(email);
		if (user is null)
		{
			user = new ApplicationUser
			{
				UserName = email,
				Email = email,
				EmailConfirmed = true
			};

			var createRes = await userManager.CreateAsync(user);
			if (!createRes.Succeeded)
			{
				return Results.BadRequest(new
				{
					errors = createRes.Errors.Select(e => new { e.Code, e.Description })
				});
			}
		}

		var token = JwtTokenGenerator.GenerateToken(user, config);
		return Results.Ok(new { token });
	}
}

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
public record GoogleLoginRequest(string IdToken);
