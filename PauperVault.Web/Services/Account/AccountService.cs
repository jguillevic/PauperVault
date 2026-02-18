using Microsoft.AspNetCore.Http;
using PauperVault.Web.Infrastructure.Http.PauperVault;

namespace PauperVault.Web.Services.Account;

public class AccountService(IPauperVaultApiClient api, ITokenStore tokenStore) : IAccountService
{
	public bool IsSignedIn(HttpContext httpContext)
		=> tokenStore.HasToken(httpContext);

	public string? GetToken(HttpContext httpContext)
		=> tokenStore.GetToken(httpContext);

	public async Task<(bool Success, string? Error)> SignInAsync(HttpContext httpContext, string email, string password)
	{
		string token;
		try
		{
			token = await api.LoginAsync(email, password);
		}
		catch (Exception ex)
		{
			return (false, $"Erreur appel API : {ex.Message}");
		}

		tokenStore.SetToken(httpContext, token);
		return (true, null);
	}

	public void SignOut(HttpContext httpContext)
		=> tokenStore.ClearToken(httpContext);

	public async Task<(bool Success, string? Error)> SignInWithGoogleAsync(HttpContext httpContext, string idToken)
	{
		try
		{
			var token = await api.GoogleLoginAsync(idToken);
			tokenStore.SetToken(httpContext, token);
			return (true, null);
		}
		catch (HttpRequestException ex)
		{
			return (false, $"Erreur réseau/appel API : {ex.Message}");
		}
		catch (Exception ex)
		{
			return (false, $"Erreur inattendue : {ex.Message}");
		}
	}
}
