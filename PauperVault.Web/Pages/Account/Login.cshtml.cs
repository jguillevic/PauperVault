using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using PauperVault.Web.Services.Account;

namespace PauperVault.Web.Pages.Account;

public class LoginModel(IAccountService accountService) : PageModel
{
	[BindProperty]
	public LoginInput Input { get; set; } = new();

	public string? ErrorMessage { get; set; }

	public class LoginInput
	{
		[Required(ErrorMessage = "L’adresse e-mail est obligatoire.")]
		[EmailAddress(ErrorMessage = "Veuillez saisir une adresse e-mail valide.")]
		public string Email { get; set; } = "";

		[Required(ErrorMessage = "Le mot de passe est obligatoire.")]
		public string Password { get; set; } = "";
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
			return Page();

		var (success, error) = await accountService.SignInAsync(HttpContext, Input.Email, Input.Password);
		if (!success)
		{
			ErrorMessage = error;
			return Page();
		}

		return RedirectToPage("/Account/Me");
	}
}
