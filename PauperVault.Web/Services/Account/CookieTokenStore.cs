using Microsoft.AspNetCore.Http;

namespace PauperVault.Web.Services.Account;

public sealed class CookieTokenStore : ITokenStore
{
	public const string CookieName = "pv_token";

	public bool HasToken(HttpContext httpContext)
		=> GetToken(httpContext) is not null;

	public string? GetToken(HttpContext httpContext)
	{
		httpContext.Request.Cookies.TryGetValue(CookieName, out var token);
		return string.IsNullOrWhiteSpace(token) ? null : token;
	}

	public void SetToken(HttpContext httpContext, string token, TimeSpan? lifetime = null)
	{
		var options = BuildCookieOptions(httpContext, lifetime ?? TimeSpan.FromMinutes(60));
		httpContext.Response.Cookies.Append(CookieName, token, options);
	}

	public void ClearToken(HttpContext httpContext)
	{
		// Pour supprimer de façon fiable, on passe les mêmes options (Path/Secure/SameSite).
		var options = BuildCookieOptions(httpContext, TimeSpan.FromMinutes(60));
		httpContext.Response.Cookies.Delete(CookieName, options);
	}

	private static CookieOptions BuildCookieOptions(HttpContext httpContext, TimeSpan lifetime)
		=> new()
		{
			HttpOnly = true,
			Secure = httpContext.Request.IsHttps, // dev-friendly
			SameSite = SameSiteMode.Lax,
			Expires = DateTimeOffset.UtcNow.Add(lifetime),
			Path = "/"
		};
}
