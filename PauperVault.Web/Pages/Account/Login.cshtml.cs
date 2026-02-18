using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using PauperVault.Web.Services.Account;

namespace PauperVault.Web.Pages.Account;

public class LoginModel(
	IAccountService accountService, 
	IConfiguration config) : PageModel
{
	[BindProperty]
	public LoginInput Input { get; set; } = new();

	public string? ErrorMessage { get; set; }

	public string GoogleClientId { get; } = config["Google:ClientId"] ?? "";

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

	public async Task<IActionResult> OnPostGoogleAsync([FromForm] string idToken)
	{
		if (User?.Identity?.IsAuthenticated == true)
			return RedirectToPage("/Account/Me");

		try
		{
			if (string.IsNullOrWhiteSpace(idToken))
			{
				ErrorMessage = "Connexion Google impossible : token manquant.";
				return Page();
			}

			var (success, error) = await accountService.SignInWithGoogleAsync(HttpContext, idToken);
			if (!success)
			{
				ErrorMessage = error;
				return Page();
			}

			return RedirectToPage("/Account/Me");
		}
		catch (Exception ex)
		{
			// Important : log + message affichée
			Console.Error.WriteLine(ex);
			ErrorMessage = ex.ToString();
			return Page();
		}
	}

}
