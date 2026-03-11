using Microsoft.AspNetCore.Mvc.RazorPages;
using PauperVault.Web.Contracts.Decks;
using PauperVault.Web.Infrastructure.Http.PauperVault;

namespace PauperVault.Web.Pages.Decks;

public class ExploreModel(
	IPauperVaultApiClient api
) : PageModel
{
	public IReadOnlyList<PublicDeckListItemDto> Decks { get; private set; } = Array.Empty<PublicDeckListItemDto>();

	public string? ErrorMessage { get; private set; }

	public async Task OnGetAsync(CancellationToken ct)
	{
		try
		{
			Decks = await api.GetPublicDecksAsync(skip: 0, take: 60, ct);
		}
		catch
		{
			ErrorMessage = "Impossible de charger les decks pour le moment.";
			Decks = Array.Empty<PublicDeckListItemDto>();
		}
	}
}