using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PauperVault.Web.Infrastructure.Http.PauperVault;
using System.ComponentModel.DataAnnotations;

namespace PauperVault.Web.Pages.Account;

public class RegisterModel : PageModel
{
	private readonly IPauperVaultApiClient _api;

	public RegisterModel(IPauperVaultApiClient api)
	{
		_api = api;
	}

	[BindProperty]
	public RegisterInput Input { get; set; } = new();

	public string? ErrorMessage { get; set; }

	public class RegisterInput
	{
		[Required(ErrorMessage = "L’adresse e-mail est obligatoire.")]
		[EmailAddress(ErrorMessage = "Veuillez saisir une adresse e-mail valide.")]
		public string Email { get; set; } = "";

		[Required(ErrorMessage = "Le mot de passe est obligatoire.")]
		[MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
		public string Password { get; set; } = "";

		[Required(ErrorMessage = "La confirmation du mot de passe est obligatoire.")]
		[Compare(nameof(Password), ErrorMessage = "Les mots de passe ne correspondent pas.")]
		public string ConfirmPassword { get; set; } = "";
	}

	public async Task<IActionResult> OnPostAsync(CancellationToken ct)
	{
		if (!ModelState.IsValid)
			return Page();

		try
		{
			await _api.RegisterAsync(Input.Email, Input.Password, ct);
			return RedirectToPage("/Account/Login");
		}
		catch (Exception ex)
		{
			// Pour l’instant, brut. Après on peut parser proprement le JSON { errors: [...] }
			ErrorMessage = "Inscription impossible. " + ex.Message;
			return Page();
		}
	}
}
