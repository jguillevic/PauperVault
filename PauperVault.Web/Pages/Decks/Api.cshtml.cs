using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PauperVault.Web.Contracts.Decks;
using PauperVault.Web.Infrastructure.Http.PauperVault;

namespace PauperVault.Web.Pages.Decks;

public class ApiModel : PageModel
{
	private readonly IPauperVaultApiClient _api;
	public ApiModel(IPauperVaultApiClient api) => _api = api;

	public async Task<IActionResult> OnGetCardAutocompleteAsync(string q, CancellationToken ct)
	{
		var dto = await _api.AutocompleteCardsAsync(q, ct);
		return new JsonResult(dto);
	}

	public async Task<IActionResult> OnGetCardResolveAsync(string name, CancellationToken ct)
	{
		var dto = await _api.ResolveCardAsync(name, ct);
		return new JsonResult(dto);
	}

	public record AddCardWebRequest(Guid DeckId, Guid ScryfallId, string Zone, int Quantity);

	public async Task<IActionResult> OnPostAddCardAsync([FromBody] AddCardWebRequest req, CancellationToken ct)
	{
		// Zone vient en string "Main"/"Sideboard" -> parse vers enum côté Web
		var zone = Enum.Parse<PauperVault.Core.Domain.Decks.DeckZone>(req.Zone, ignoreCase: true);

		await _api.AddOrUpdateDeckCardAsync(req.DeckId,
			new AddOrUpdateDeckCardRequest(req.ScryfallId, zone, req.Quantity),
			ct);

		return new OkResult();
	}
}