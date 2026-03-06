using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PauperVault.Web.Contracts.Decks;
using PauperVault.Web.Infrastructure.Http.PauperVault;

namespace PauperVault.Web.Pages.Decks;

public class IndexModel(
	IPauperVaultApiClient api
) : PageModel
{
	public IReadOnlyList<DeckListItemDto> Decks { get; private set; } = Array.Empty<DeckListItemDto>();

	public string? ErrorMessage { get; private set; }

	public async Task<IActionResult> OnGetAsync(CancellationToken ct)
	{
		try
		{
			Decks = await api.GetDecksAsync(ct);
			return Page();
		}
		catch (UnauthorizedAccessException)
		{
			return RedirectToPage("/Account/Login");
		}
		catch (HttpRequestException)
		{
			ErrorMessage = "Impossible de charger les decks pour le moment.";
			return Page();
		}
	}

	public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken ct)
	{
		try
		{
			await api.DeleteDeckAsync(id, ct);
			return RedirectToPage();
		}
		catch (UnauthorizedAccessException)
		{
			return RedirectToPage("/Account/Login");
		}
		catch (HttpRequestException)
		{
			ErrorMessage = "Impossible de supprimer ce deck.";
			Decks = await api.GetDecksAsync(ct);
			return Page();
		}
	}
}