using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PauperVault.Web.Contracts.Decks;
using PauperVault.Web.Infrastructure.Http.PauperVault;

namespace PauperVault.Web.Pages.Decks;

public class ViewModel(IPauperVaultApiClient api) : PageModel
{
	[FromRoute]
	public Guid Id { get; set; }

	public PublicDeckDetailsDto Deck { get; private set; } = default!;

	public string? ErrorMessage { get; private set; }

	public async Task<IActionResult> OnGetAsync(CancellationToken ct)
	{
		try
		{
			Deck = await api.GetPublicDeckAsync(Id, ct);
			return Page();
		}
		catch (HttpRequestException)
		{
			ErrorMessage = "Impossible de charger ce deck pour le moment.";
			return Page();
		}
	}
}