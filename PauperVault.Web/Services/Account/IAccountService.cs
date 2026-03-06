using Microsoft.AspNetCore.Http;

namespace PauperVault.Web.Services.Account;

public interface IAccountService
{
	Task<(bool Success, string? Error)> SignInAsync(HttpContext httpContext, string email, string password);
	void SignOut(HttpContext httpContext);
	bool IsSignedIn(HttpContext httpContext);
	Task<(bool Success, string? Error)> SignInWithGoogleAsync(HttpContext httpContext, string idToken);
	Task<(bool Success, string? Error)> RegisterAsync(string email, string password, CancellationToken ct = default);
}
