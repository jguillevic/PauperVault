using Microsoft.EntityFrameworkCore;
using PauperVault.Api.Contracts.Cards;
using PauperVault.Api.Infrastructure.Data;
using PauperVault.Api.Infrastructure.Scryfall;
using PauperVault.Core.Domain.Cards;

namespace PauperVault.Api.Endpoints;

public static class CardEndpoints
{
	public static void MapCardEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/cards")
			.RequireAuthorization(); // tu peux enlever si tu veux permettre l'usage sans auth

		// 1) Autocomplete (noms)
		group.MapGet("/autocomplete", async (
			string q,
			ScryfallClient scryfall,
			CancellationToken ct) =>
		{
			q = (q ?? "").Trim();
			if (q.Length < 2)
				return Results.Ok(new CardAutocompleteDto(Array.Empty<string>()));

			var suggestions = await scryfall.AutocompleteAsync(q, ct);
			return Results.Ok(new CardAutocompleteDto(suggestions));
		});

		// 2) Resolve (fuzzy) + upsert dans CardCache
		group.MapGet("/resolve", async (
			string name,
			ScryfallClient scryfall,
			DataDbContext db,
			CancellationToken ct) =>
		{
			name = (name ?? "").Trim();
			if (name.Length < 2)
				return Results.BadRequest("Card name is required.");

			// 2.1: Résoudre via Scryfall
			var card = await scryfall.ResolveByFuzzyNameAsync(name, ct);
			if (card is null)
				return Results.NotFound("Card not found.");

			// 2.2: Mapper champs utiles
			card.Legalities ??= new Dictionary<string, string>();
			card.ImageUris ??= new Dictionary<string, string>();

			card.Legalities.TryGetValue("pauper", out var pauperLegality);

			// Scryfall images: small/normal/large/png/art_crop/border_crop
			card.ImageUris.TryGetValue("small", out var imageSmall);

			// 2.3: Upsert dans CardCache
			var existing = await db.Cards.FirstOrDefaultAsync(c => c.ScryfallId == card.Id, ct);
			if (existing is null)
			{
				existing = new CardCache { ScryfallId = card.Id };
				db.Cards.Add(existing);
			}

			existing.Name = card.Name;
			existing.ManaCost = card.ManaCost;
			existing.TypeLine = card.TypeLine;
			existing.OracleText = card.OracleText;
			existing.SetCode = card.SetCode;
			existing.CollectorNumber = card.CollectorNumber;
			existing.ImageSmallUrl = imageSmall;
			existing.Rarity = card.Rarity;
			existing.PauperLegality = pauperLegality;
			existing.LastFetchedAt = DateTimeOffset.UtcNow;

			await db.SaveChangesAsync(ct);

			// 2.4: Réponse API stable (depuis cache)
			var dto = new CardDto(
				existing.ScryfallId,
				existing.Name,
				existing.ManaCost,
				existing.TypeLine,
				existing.OracleText,
				existing.SetCode,
				existing.CollectorNumber,
				existing.Rarity,
				existing.PauperLegality,
				existing.ImageSmallUrl);

			return Results.Ok(dto);
		});

		// 3) (Optionnel mais pratique) Get par id depuis cache
		group.MapGet("/{scryfallId:guid}", async (
			Guid scryfallId,
			DataDbContext db,
			CancellationToken ct) =>
		{
			var c = await db.Cards.AsNoTracking().FirstOrDefaultAsync(x => x.ScryfallId == scryfallId, ct);
			if (c is null) return Results.NotFound();

			return Results.Ok(new CardDto(
				c.ScryfallId, c.Name, c.ManaCost, c.TypeLine, c.OracleText,
				c.SetCode, c.CollectorNumber, c.Rarity, c.PauperLegality, c.ImageSmallUrl));
		});
	}
}