using Microsoft.AspNetCore.Identity;
using PauperVault.Api.Infrastructure.Auth;
using PauperVault.Core.Domain.Auth;

namespace PauperVault.Api.Features.Auth;

public static class AuthCommands
{
	public static async Task<AuthCommandResult> RegisterAsync(
		string email,
		string password,
		UserManager<ApplicationUser> userManager)
	{
		if (!AuthRules.HasEmail(email))
		{
			return new AuthCommandResult.ValidationError(new[]
			{
				new AuthErrorItem(AuthErrorCodes.EmailRequired, AuthErrors.MissingEmail)
			});
		}

		if (!AuthRules.HasPassword(password))
		{
			return new AuthCommandResult.ValidationError(new[]
			{
				new AuthErrorItem(AuthErrorCodes.PasswordRequired, AuthErrors.MissingPassword)
			});
		}

		var user = new ApplicationUser
		{
			UserName = email,
			Email = email
		};

		var result = await userManager.CreateAsync(user, password);

		if (!result.Succeeded)
			return AuthMappings.ToValidationError(result);

		return new AuthCommandResult.Success(Message: AuthMessages.UserCreated);
	}

	public static async Task<AuthCommandResult> LoginAsync(
		string email,
		string password,
		SignInManager<ApplicationUser> signInManager,
		UserManager<ApplicationUser> userManager,
		IConfiguration config)
	{
		if (!AuthRules.HasEmail(email))
		{
			return new AuthCommandResult.ValidationError(new[]
			{
				new AuthErrorItem(AuthErrorCodes.EmailRequired, AuthErrors.MissingEmail)
			});
		}

		if (!AuthRules.HasPassword(password))
		{
			return new AuthCommandResult.ValidationError(new[]
			{
				new AuthErrorItem(AuthErrorCodes.PasswordRequired, AuthErrors.MissingPassword)
			});
		}

		var result = await signInManager.PasswordSignInAsync(
			email,
			password,
			isPersistent: false,
			lockoutOnFailure: false);

		if (!result.Succeeded)
			return new AuthCommandResult.Unauthorized(AuthErrors.InvalidCredentials);

		var user = await userManager.FindByEmailAsync(email);
		if (user is null)
			return new AuthCommandResult.Unauthorized(AuthErrors.InvalidCredentials);

		var token = JwtTokenGenerator.GenerateToken(user, config);

		return new AuthCommandResult.Success(Token: token);
	}

	public static async Task<AuthCommandResult> GoogleLoginAsync(
		GoogleIdentity googleIdentity,
		UserManager<ApplicationUser> userManager,
		IConfiguration config)
	{
		if (!AuthRules.HasEmail(googleIdentity.Email))
		{
			return new AuthCommandResult.ValidationError(new[]
			{
				new AuthErrorItem(AuthErrorCodes.EmailRequired, AuthErrors.MissingEmail)
			});
		}

		var user = await userManager.FindByEmailAsync(googleIdentity.Email);

		if (user is null)
		{
			user = new ApplicationUser
			{
				UserName = googleIdentity.Email,
				Email = googleIdentity.Email,
				EmailConfirmed = true
			};

			var createResult = await userManager.CreateAsync(user);
			if (!createResult.Succeeded)
				return AuthMappings.ToValidationError(createResult);
		}

		var token = JwtTokenGenerator.GenerateToken(user, config);

		return new AuthCommandResult.Success(Token: token);
	}
}