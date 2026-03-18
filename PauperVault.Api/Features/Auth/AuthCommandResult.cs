namespace PauperVault.Api.Features.Auth;

public abstract record AuthCommandResult
{
	public sealed record Success(string? Token = null, string? Message = null) : AuthCommandResult;

	public sealed record ValidationError(
		IReadOnlyCollection<AuthErrorItem> Errors) : AuthCommandResult;

	public sealed record Unauthorized(
		string? Message = null) : AuthCommandResult;

	public sealed record ConfigurationError(
		string Message) : AuthCommandResult;

	public sealed record Conflict(
		string Message) : AuthCommandResult;
}

public sealed record AuthErrorItem(string Code, string Description);