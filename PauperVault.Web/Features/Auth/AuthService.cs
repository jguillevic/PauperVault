using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace PauperVault.Web.Features.Auth;

public interface IAuthService
{
	string CreateCsrfToken(HttpContext ctx);
	Task<AuthResult> SessionLoginAsync(HttpContext ctx, SessionLoginRequest req);
	Task SessionLogoutAsync(HttpContext ctx);
}

public sealed class AuthService : IAuthService
{
	public string CreateCsrfToken(HttpContext ctx)
	{
		var token = Guid.NewGuid().ToString("N");

		ctx.Response.Cookies.Append(AuthConstants.CsrfCookieName, token, new CookieOptions
		{
			HttpOnly = false,                // lisible côté JS
			Secure = ctx.Request.IsHttps,    // en prod toujours https
			SameSite = SameSiteMode.Lax,
			Path = "/"
		});

		return token;
	}

	public async Task<AuthResult> SessionLoginAsync(HttpContext ctx, SessionLoginRequest req)
	{
		// CSRF check (double submit cookie)
		if (!ctx.Request.Cookies.TryGetValue(AuthConstants.CsrfCookieName, out var csrfCookie) ||
			string.IsNullOrWhiteSpace(req.CsrfToken) ||
			!string.Equals(csrfCookie, req.CsrfToken, StringComparison.Ordinal))
		{
			return new AuthResult(false, "CSRF token invalid");
		}

		// Verify Firebase ID token
		FirebaseToken decoded;
		try
		{
			decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(req.IdToken);
		}
		catch
		{
			return new AuthResult(false, "Invalid Firebase token");
		}

		// Build claims
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, decoded.Uid)
		};

		if (decoded.Claims.TryGetValue("email", out var emailObj) && emailObj is string email)
			claims.Add(new Claim(ClaimTypes.Email, email));

		var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthConstants.Scheme));

		// ASP.NET SignIn (cookie)
		await ctx.SignInAsync(AuthConstants.Scheme, principal);

		return new AuthResult(true);
	}

	public async Task SessionLogoutAsync(HttpContext ctx)
	{
		ctx.Response.Cookies.Delete(AuthConstants.CsrfCookieName, new CookieOptions { Path = "/" });
		ctx.Response.Cookies.Delete(AuthConstants.AuthCookieName, new CookieOptions { Path = "/" });
		await ctx.SignOutAsync(AuthConstants.Scheme);
	}
}
