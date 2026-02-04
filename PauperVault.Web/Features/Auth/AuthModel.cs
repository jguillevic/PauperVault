namespace PauperVault.Web.Features.Auth;

public sealed record SessionLoginRequest(string IdToken, string CsrfToken);

public sealed record AuthResult(bool Ok, string? Error = null);
