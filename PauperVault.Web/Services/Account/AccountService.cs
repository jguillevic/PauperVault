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
		catch (UnauthorizedAccessException)
		{
			return (false, "Adresse e-mail ou mot de passe invalide.");
		}
		catch (InvalidOperationException)
		{
			return (false, "Les informations de connexion sont invalides.");
		}
		catch (HttpRequestException)
		{
			return (false, "Le service de connexion est momentanément indisponible. Veuillez réessayer dans quelques instants.");
		}
		catch (TaskCanceledException)
		{
			return (false, "La connexion a pris trop de temps. Veuillez réessayer.");
		}
		catch (Exception)
		{
			return (false, "Une erreur inattendue est survenue lors de la connexion.");
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

	public async Task<(bool Success, string? Error)> RegisterAsync(string email, string password, CancellationToken ct = default)
	{
		try
		{
			await api.RegisterAsync(email, password, ct);
			return (true, null);
		}
		catch (InvalidOperationException ex)
		{
			return (false, string.IsNullOrWhiteSpace(ex.Message)
				? "Les informations d’inscription sont invalides."
				: ex.Message);
		}
		catch (HttpRequestException)
		{
			return (false, "Le service d’inscription est momentanément indisponible. Veuillez réessayer dans quelques instants.");
		}
		catch (TaskCanceledException)
		{
			return (false, "L’inscription a pris trop de temps. Veuillez réessayer.");
		}
		catch (Exception)
		{
			return (false, "Une erreur inattendue est survenue lors de l’inscription.");
		}
	}
}
