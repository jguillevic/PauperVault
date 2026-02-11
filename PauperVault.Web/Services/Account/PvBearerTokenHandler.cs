using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace PauperVault.Web.Services.Account;

public class PvBearerTokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		// Ne pas toucher si déjà un Authorization explicitement posé
		if (request.Headers.Authorization is not null)
			return base.SendAsync(request, cancellationToken);

		// Skip sur /auth/login
		var path = request.RequestUri?.AbsolutePath?.ToLowerInvariant() ?? "";
		if (path.EndsWith("/auth/login"))
			return base.SendAsync(request, cancellationToken);

		var ctx = httpContextAccessor.HttpContext;
		if (ctx is null)
			return base.SendAsync(request, cancellationToken);

		if (ctx.Request.Cookies.TryGetValue(CookieTokenStore.CookieName, out var token)
			&& !string.IsNullOrWhiteSpace(token))
		{
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}

		return base.SendAsync(request, cancellationToken);
	}
}
