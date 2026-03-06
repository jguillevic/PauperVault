using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PauperVault.Web.Contracts.Decks;
using PauperVault.Web.Infrastructure.Http.PauperVault;

namespace PauperVault.Web.Pages.Decks;

public class EditModel(
	IPauperVaultApiClient api
) : PageModel
{
	[FromRoute]
	public Guid Id { get; set; }

	public DeckDetailsDto Deck { get; private set; } = default!;

	public async Task OnGetAsync(CancellationToken ct)
	{
		Deck = await api.GetDeckAsync(Id, ct);
	}
}