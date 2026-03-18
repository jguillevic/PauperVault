using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PauperVault.Api.Infrastructure.Auth;
using PauperVault.Contracts.Auth.Dtos;
using PauperVault.Contracts.Auth.Requests;
using PauperVault.Core.Domain.Auth;
using System.Security.Claims;

namespace PauperVault.Api.Features.Auth;

public static class AuthHandlers
{
	public static async Task<IResult> RegisterAsync(
		[FromBody] RegisterRequest request,
		UserManager<ApplicationUser> userManager)
	{
		var email = AuthRules.NormalizeEmail(request.Email);

		var result = await AuthCommands.RegisterAsync(
			email,
			request.Password,
			userManager);

		return AuthMappings.ToHttpResult(result);
	}

	public static async Task<IResult> LoginAsync(
		[FromBody] LoginRequest request,
		SignInManager<ApplicationUser> signInManager,
		UserManager<ApplicationUser> userManager,
		IConfiguration config)
	{
		var email = AuthRules.NormalizeEmail(request.Email);

		var result = await AuthCommands.LoginAsync(
			email,
			request.Password,
			signInManager,
			userManager,
			config);

		return AuthMappings.ToHttpResult(result);
	}

	public static async Task<IResult> GoogleLoginAsync(
		[FromBody] GoogleLoginRequest request,
		UserManager<ApplicationUser> userManager,
		IConfiguration config)
	{
		if (!AuthRules.HasGoogleIdToken(request.IdToken))
		{
			var error = new AuthCommandResult.ValidationError(new[]
			{
				new AuthErrorItem("google_id_token_missing", AuthErrors.MissingGoogleIdToken)
			});

			return AuthMappings.ToHttpResult(error);
		}

		var (identity, validationError) = await AuthQueries.ValidateGoogleTokenAsync(
			request.IdToken!,
			config);

		if (validationError is not null)
			return AuthMappings.ToHttpResult(validationError);

		var result = await AuthCommands.GoogleLoginAsync(
			identity!,
			userManager,
			config);

		return AuthMappings.ToHttpResult(result);
	}

	public static IResult GetMe(ClaimsPrincipal user)
	{
		var userId = user.GetUserIdOrNull();
		var email = user.GetEmailOrNull();

		return Results.Ok(new MeDto(userId, email));
	}
}