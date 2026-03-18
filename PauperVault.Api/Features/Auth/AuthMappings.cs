using Microsoft.AspNetCore.Identity;

namespace PauperVault.Api.Features.Auth;

public static class AuthMappings
{
	public static AuthCommandResult.ValidationError ToValidationError(IdentityResult result)
		=> new(result.Errors
			.Select(e => new AuthErrorItem(e.Code, e.Description))
			.ToArray());

	public static IResult ToHttpResult(AuthCommandResult result)
	{
		return result switch
		{
			AuthCommandResult.Success success when !string.IsNullOrWhiteSpace(success.Token)
				=> Results.Ok(new { token = success.Token }),

			AuthCommandResult.Success success when !string.IsNullOrWhiteSpace(success.Message)
				=> Results.Ok(new { message = success.Message }),

			AuthCommandResult.Success
				=> Results.Ok(),

			AuthCommandResult.ValidationError validation
				=> Results.BadRequest(new
				{
					errors = validation.Errors.Select(e => new { e.Code, e.Description })
				}),

			AuthCommandResult.Unauthorized
				=> Results.Unauthorized(),

			AuthCommandResult.ConfigurationError config
				=> Results.Problem(config.Message),

			AuthCommandResult.Conflict conflict
				=> Results.Conflict(new { error = conflict.Message }),

			_ => Results.Problem("Unexpected auth result.")
		};
	}
}