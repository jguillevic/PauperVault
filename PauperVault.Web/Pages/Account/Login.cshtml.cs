using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using PauperVault.Web.Services.Account;

namespace PauperVault.Web.Pages.Account;

public class LoginModel(
	IAccountService accountService,
	IConfiguration config,
	ILogger<LoginModel> logger) : PageModel
{
	[BindProperty]
	public LoginInput Input { get; set; } = new();

	public string? ErrorMessage { get; private set; }

	public string GoogleClientId { get; } = config["Google:ClientId"] ?? "";

	public class LoginInput
	{
		[Required(ErrorMessage = "L’adresse e-mail est obligatoire.")]
		[EmailAddress(ErrorMessage = "Veuillez saisir une adresse e-mail valide.")]
		public string Email { get; set; } = "";

		[Required(ErrorMessage = "Le mot de passe est obligatoire.")]
		public string Password { get; set; } = "";
	}

	public IActionResult OnGet()
	{
		if (User?.Identity?.IsAuthenticated == true)
			return RedirectToPage("/Index");

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (User?.Identity?.IsAuthenticated == true)
			return RedirectToPage("/Index");

		if (!ModelState.IsValid)
		{
			ErrorMessage = "Merci de corriger les champs du formulaire.";
			return Page();
		}

		try
		{
			var (success, error) = await accountService.SignInAsync(HttpContext, Input.Email, Input.Password);

			if (!success)
			{
				ErrorMessage = error;
				return Page();
			}

			return RedirectToPage("/Index");
		}
		catch (HttpRequestException ex)
		{
			logger.LogError(ex, "Erreur réseau lors de la connexion par mot de passe pour {Email}.", Input.Email);
			ErrorMessage = "Le service de connexion est momentanément indisponible. Veuillez réessayer dans quelques instants.";
			return Page();
		}
		catch (TaskCanceledException ex)
		{
			logger.LogError(ex, "Timeout lors de la connexion par mot de passe pour {Email}.", Input.Email);
			ErrorMessage = "La connexion a pris trop de temps. Veuillez réessayer.";
			return Page();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Erreur inattendue lors de la connexion par mot de passe pour {Email}.", Input.Email);
			ErrorMessage = "Une erreur inattendue est survenue lors de la connexion.";
			return Page();
		}
	}

	public async Task<IActionResult> OnPostGoogleAsync([FromForm] string? idToken)
	{
		if (User?.Identity?.IsAuthenticated == true)
			return RedirectToPage("/Index");

		try
		{
			if (string.IsNullOrWhiteSpace(idToken))
			{
				ErrorMessage = "Connexion Google impossible : jeton manquant.";
				return Page();
			}

			var (success, error) = await accountService.SignInWithGoogleAsync(HttpContext, idToken);

			if (!success)
			{
				ErrorMessage = string.IsNullOrWhiteSpace(error)
					? "La connexion avec Google a échoué."
					: error;

				return Page();
			}

			return RedirectToPage("/Index");
		}
		catch (HttpRequestException ex)
		{
			logger.LogError(ex, "Erreur réseau lors de la connexion Google.");
			ErrorMessage = "Le service de connexion Google est momentanément indisponible. Veuillez réessayer dans quelques instants.";
			return Page();
		}
		catch (TaskCanceledException ex)
		{
			logger.LogError(ex, "Timeout lors de la connexion Google.");
			ErrorMessage = "La connexion Google a pris trop de temps. Veuillez réessayer.";
			return Page();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Erreur inattendue lors de la connexion Google.");
			ErrorMessage = "Une erreur inattendue est survenue lors de la connexion avec Google.";
			return Page();
		}
	}
}