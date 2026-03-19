using PauperVault.Api.Infrastructure.Data;
using PauperVault.Api.Infrastructure.Scryfall;
using PauperVault.Core.Domain.Cards;

namespace PauperVault.Api.Features.Cards;

public static class CardHandlers
{
	public static async Task<IResult> AutocompleteAsync(
		string q,
		ScryfallClient scryfall,
		CancellationToken ct)
	{
		var normalized = CardRules.NormalizeSearch(q);

		if (!CardRules.IsValidSearch(normalized))
			return Results.Ok(CardQueries.EmptyAutocomplete());

		var suggestions = await CardQueries.AutocompleteAsync(scryfall, normalized, ct);
		return Results.Ok(suggestions);
	}

	public static async Task<IResult> ResolveAsync(
		string name,
		ScryfallClient scryfall,
		DataDbContext db,
		CancellationToken ct)
	{
		var normalized = CardRules.NormalizeSearch(name);

		if (!CardRules.IsValidSearch(normalized))
			return Results.BadRequest(CardErrors.NameRequired);

		var cached = await CardQueries.GetValidCachedByNameAsync(db, normalized, ct);
		if (cached is not null)
			return Results.Ok(CardQueries.ToDto(cached));

		var resolved = await CardQueries.ResolveFromScryfallAsync(scryfall, normalized, ct);
		if (resolved is null)
			return Results.NotFound(CardErrors.NotFound);

		var refreshed = await CardCommands.UpsertCacheAsync(db, resolved, ct);

		return Results.Ok(CardQueries.ToDto(refreshed));
	}

	public static async Task<IResult> GetByIdAsync(
		Guid scryfallId,
		DataDbContext db,
		CancellationToken ct)
	{
		var card = await CardQueries.GetCachedByIdAsync(db, scryfallId, ct);
		return card is null
			? Results.NotFound()
			: Results.Ok(CardQueries.ToDto(card));
	}

	public static async Task<IResult> InvalidateAsync(
		Guid scryfallId,
		DataDbContext db,
		CancellationToken ct)
	{
		var ok = await CardCommands.InvalidateAsync(db, scryfallId, ct);
		return ok ? Results.NoContent() : Results.NotFound();
	}

	public static async Task<IResult> InvalidateAllAsync(
		DataDbContext db,
		CancellationToken ct)
	{
		var count = await CardCommands.InvalidateAllAsync(db, ct);
		return Results.Ok(new { invalidatedCount = count });
	}
}