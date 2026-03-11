using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PauperVault.Web.Contracts.Decks;
using PauperVault.Web.Infrastructure.Http.PauperVault;

namespace PauperVault.Web.Pages
{
	public class IndexModel(
		IPauperVaultApiClient api
	) : PageModel
	{
		public IReadOnlyList<PublicDeckListItemDto> LatestDecks { get; private set; } = Array.Empty<PublicDeckListItemDto>();

		public string? ErrorMessage { get; private set; }

		public async Task OnGetAsync(CancellationToken ct)
		{
			try
			{
				LatestDecks = await api.GetPublicDecksAsync(skip: 0, take: 10, ct);
			}
			catch
			{
				ErrorMessage = "Impossible de charger les derniers decks pour le moment.";
				LatestDecks = Array.Empty<PublicDeckListItemDto>();
			}
		}
	}
}
