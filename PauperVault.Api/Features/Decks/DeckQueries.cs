using Microsoft.EntityFrameworkCore;
using PauperVault.Api.Infrastructure.Data;
using PauperVault.Contracts.Decks.Dtos;

namespace PauperVault.Api.Features.Decks;

public static class DeckQueries
{
	public static async Task<IReadOnlyList<PublicDeckListItemDto>> GetPublicDecksAsync(
		DataDbContext db,
		int? skip,
		int? take,
		CancellationToken ct)
	{
		var safeSkip = Math.Max(0, skip ?? 0);
		var safeTake = Math.Clamp(take ?? 10, 1, 100);

		var decks = await db.Decks
			.AsNoTracking()
			.OrderByDescending(d => d.UpdatedAt)
			.Skip(safeSkip)
			.Take(safeTake)
			.Select(d => new PublicDeckProjection(
				d.Id,
				d.Name,
				d.UpdatedAt,
				d.Cards
					.OrderBy(c => c.Zone)
					.ThenBy(c => c.ScryfallId)
					.Select(c => (Guid?)c.ScryfallId)
					.FirstOrDefault()))
			.ToListAsync(ct);

		var firstCardIds = decks
			.Where(x => x.FirstCardId.HasValue)
			.Select(x => x.FirstCardId!.Value)
			.Distinct()
			.ToList();

		var imageByCardId = await db.Cards
			.AsNoTracking()
			.Where(c => firstCardIds.Contains(c.ScryfallId))
			.ToDictionaryAsync(c => c.ScryfallId, c => c.ImageSmallUrl, ct);

		return decks
			.Select(d => new PublicDeckListItemDto(
				d.Id,
				d.Name,
				d.UpdatedAt,
				ResolveCoverImageUrl(d.FirstCardId, imageByCardId)))
			.ToList();
	}

	public static async Task<IReadOnlyList<DeckListItemDto>> GetDeckListForUserAsync(
		DataDbContext db,
		string userId,
		CancellationToken ct)
	{
		return await db.Decks
			.AsNoTracking()
			.Where(d => d.OwnerUserId == userId)
			.OrderByDescending(d => d.UpdatedAt)
			.Select(d => new DeckListItemDto(d.Id, d.Name, d.UpdatedAt))
			.ToListAsync(ct);
	}

	public static async Task<DeckDetailsDto?> GetDeckDetailsForUserAsync(
		DataDbContext db,
		Guid deckId,
		string userId,
		CancellationToken ct)
	{
		var deck = await db.Decks
			.AsNoTracking()
			.Include(d => d.Cards)
			.FirstOrDefaultAsync(d => d.Id == deckId && d.OwnerUserId == userId, ct);

		if (deck is null)
			return null;

		var scryfallIds = deck.Cards
			.Select(c => c.ScryfallId)
			.Distinct()
			.ToList();

		var cardNamesById = await db.Cards
			.AsNoTracking()
			.Where(c => scryfallIds.Contains(c.ScryfallId))
			.ToDictionaryAsync(c => c.ScryfallId, c => c.Name, ct);

		var cards = deck.Cards
			.OrderBy(c => c.Zone)
			.ThenBy(c => ResolveCardName(c.ScryfallId, cardNamesById))
			.Select(c => new DeckCardDto(
				c.ScryfallId,
				ResolveCardName(c.ScryfallId, cardNamesById),
				c.Zone,
				c.Quantity))
			.ToList();

		return new DeckDetailsDto(
			deck.Id,
			deck.Name,
			deck.Description,
			deck.CreatedAt,
			deck.UpdatedAt,
			cards);
	}

	private static string? ResolveCoverImageUrl(
		Guid? firstCardId,
		IReadOnlyDictionary<Guid, string?> imageByCardId)
	{
		if (!firstCardId.HasValue)
			return null;

		if (!imageByCardId.TryGetValue(firstCardId.Value, out var imageUrl))
			return null;

		return string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl;
	}

	private static string ResolveCardName(
		Guid scryfallId,
		IReadOnlyDictionary<Guid, string> cardNamesById)
	{
		return cardNamesById.TryGetValue(scryfallId, out var name)
			? name
			: scryfallId.ToString();
	}

	private sealed record PublicDeckProjection(
		Guid Id,
		string Name,
		DateTimeOffset UpdatedAt,
		Guid? FirstCardId);

	public static async Task<PublicDeckDetailsDto?> GetPublicDeckDetailsAsync(
		DataDbContext db,
		Guid deckId,
		string? currentUserId,
		CancellationToken ct)
	{
		var deck = await db.Decks
			.AsNoTracking()
			.Include(d => d.Cards)
			.FirstOrDefaultAsync(d => d.Id == deckId, ct);

		if (deck is null)
			return null;

		var scryfallIds = deck.Cards
			.Select(c => c.ScryfallId)
			.Distinct()
			.ToList();

		var cardCacheById = await db.Cards
			.AsNoTracking()
			.Where(c => scryfallIds.Contains(c.ScryfallId))
			.ToDictionaryAsync(c => c.ScryfallId, ct);

		var cards = deck.Cards
			.OrderBy(c => c.Zone)
			.ThenBy(c => c.ScryfallId)
			.Select(c =>
			{
				cardCacheById.TryGetValue(c.ScryfallId, out var cached);

				return new PublicDeckCardDto(
					c.ScryfallId,
					cached?.Name ?? c.ScryfallId.ToString(),
					c.Zone,
					c.Quantity,
					cached?.ImageSmallUrl,
					cached?.ManaCost,
					cached?.TypeLine,
					cached?.OracleText,
					cached?.Power,
					cached?.Toughness,
					cached?.SetCode,
					cached?.SetName,
					cached?.CollectorNumber);
			})
			.ToList();

		var canEdit = !string.IsNullOrWhiteSpace(currentUserId)
			&& deck.OwnerUserId == currentUserId;

		return new PublicDeckDetailsDto(
			deck.Id,
			deck.Name,
			deck.Description,
			deck.OwnerUserId,
			deck.UpdatedAt,
			cards,
			canEdit);
	}
}