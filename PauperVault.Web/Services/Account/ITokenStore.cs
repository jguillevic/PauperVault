using Microsoft.AspNetCore.Http;

namespace PauperVault.Web.Services.Account;

public interface ITokenStore
{
	string? GetToken(HttpContext httpContext);
	void SetToken(HttpContext httpContext, string token, TimeSpan? lifetime = null);
	void ClearToken(HttpContext httpContext);
	bool HasToken(HttpContext httpContext);
}
