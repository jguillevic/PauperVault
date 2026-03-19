using Microsoft.EntityFrameworkCore;
using PauperVault.Api.Infrastructure.Data;
using PauperVault.Api.Infrastructure.Scryfall;
using PauperVault.Contracts.Cards.Dto;
using PauperVault.Core.Domain.Cards;

namespace PauperVault.Api.Features.Cards;

public static class CardQueries
{
	private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

	public static CardAutocompleteDto EmptyAutocomplete()
		=> new(Array.Empty<string>());

	public static async Task<CardAutocompleteDto> AutocompleteAsync(
		ScryfallClient scryfall,
		string query,
		CancellationToken ct)
	{
		var suggestions = await scryfall.AutocompleteAsync(query, ct);
		return new CardAutocompleteDto(suggestions);
	}

	public static async Task<ResolvedCard?> ResolveFromScryfallAsync(
		ScryfallClient scryfall,
		string name,
		CancellationToken ct)
	{
		var card = await scryfall.ResolveByFuzzyNameAsync(name, ct);
		if (card is null)
			return null;

		var imageUris = card.ImageUris
			?? card.CardFaces?.FirstOrDefault(f => f.ImageUris is not null)?.ImageUris;

		return new ResolvedCard(
			card.Id,
			card.Name,
			card.ManaCost,
			card.TypeLine,
			card.OracleText,
			card.SetCode,
			card.SetName,
			card.CollectorNumber,
			CardRules.GetImageUrl(imageUris, "small"),
			CardRules.GetImageUrl(imageUris, "normal"),
			CardRules.GetImageUrl(imageUris, "large"),
			card.Power,
			card.Toughness,
			card.Rarity,
			CardRules.GetPauperLegality(card.Legalities));
	}

	public static async Task<CardCache?> GetCachedByIdAsync(
		DataDbContext db,
		Guid scryfallId,
		CancellationToken ct)
	{
		return await db.Cards
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.ScryfallId == scryfallId, ct);
	}

	public static async Task<CardCache?> GetValidCachedByNameAsync(
		DataDbContext db,
		string normalizedName,
		CancellationToken ct)
	{
		var cutoff = DateTimeOffset.UtcNow.Subtract(CacheTtl);

		return await db.Cards
			.FirstOrDefaultAsync(c =>
				c.Name == normalizedName &&
				c.InvalidatedAt == null &&
				c.LastFetchedAt >= cutoff,
				ct);
	}

	public static CardDto ToDto(CardCache card)
		=> new(
			card.ScryfallId,
			card.Name,
			card.ManaCost,
			card.TypeLine,
			card.OracleText,
			card.SetCode,
			card.CollectorNumber,
			card.Rarity,
			card.PauperLegality,
			card.ImageSmallUrl,
			card.ImageNormalUrl,
			card.ImageLargeUrl,
			card.InvalidatedAt);
}