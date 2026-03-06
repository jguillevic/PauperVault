using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PauperVault.Web.Services.Account;
using System.ComponentModel.DataAnnotations;

namespace PauperVault.Web.Pages.Account;

public class RegisterModel : PageModel
{
	private readonly IAccountService _accountService;

	public RegisterModel(IAccountService accountService)
	{
		_accountService = accountService;
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

		var (success, error) = await _accountService.RegisterAsync(Input.Email, Input.Password, ct);

		if (!success)
		{
			ErrorMessage = error;
			return Page();
		}

		return RedirectToPage("/Account/Login");
	}
}