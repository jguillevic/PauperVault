using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PauperVault.Web.Infrastructure.Http.PauperVault;
using PauperVault.Web.Services.Account;

namespace PauperVault.Web.Pages.Account;

public class MeModel(IPauperVaultApiClient api, ITokenStore tokenStore) : PageModel
{
	public MeResponse? Me { get; private set; }
	public string? ErrorMessage { get; private set; }

	public async Task<IActionResult> OnGetAsync(CancellationToken ct)
	{
		if (!tokenStore.HasToken(HttpContext))
			return RedirectToPage("/Account/Login");

		try
		{
			Me = await api.MeAsync(ct);
			return Page();
		}
		catch (UnauthorizedAccessException)
		{
			tokenStore.ClearToken(HttpContext);
			return RedirectToPage("/Account/Login");
		}
		catch (Exception ex)
		{
			ErrorMessage = ex.Message;
			return Page();
		}
	}
}
