using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PauperVault.Web.Services.Account;

namespace PauperVault.Web.Pages.Account;

public class LogoutModel(IAccountService accountService) : PageModel
{
	public IActionResult OnPost()
	{
		accountService.SignOut(HttpContext);
		return RedirectToPage("/Account/Login");
	}
}
