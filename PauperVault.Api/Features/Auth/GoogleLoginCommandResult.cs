namespace PauperVault.Api.Features.Auth;

public abstract record GoogleLoginCommandResult
{
	public sealed record Success(string Token) : GoogleLoginCommandResult;
	public sealed record ValidationError(object Payload) : GoogleLoginCommandResult;
}