using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PauperVault.Web.Pages.Auth;

public sealed class LoginModel : PageModel
{
	public IActionResult OnGet()
		=> User?.Identity?.IsAuthenticated == true ? RedirectToPage("/Me") : Page();
}
