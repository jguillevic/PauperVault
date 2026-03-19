using Microsoft.EntityFrameworkCore;
using PauperVault.Api.Infrastructure.Data;
using PauperVault.Core.Domain.Cards;

namespace PauperVault.Api.Features.Cards;

public static class CardCommands
{
	public static async Task<CardCache> UpsertCacheAsync(
		DataDbContext db,
		ResolvedCard card,
		CancellationToken ct)
	{
		var existing = await db.Cards
			.FirstOrDefaultAsync(c => c.ScryfallId == card.ScryfallId, ct);

		if (existing is null)
		{
			existing = new CardCache
			{
				ScryfallId = card.ScryfallId
			};

			db.Cards.Add(existing);
		}

		existing.Name = card.Name;
		existing.ManaCost = card.ManaCost;
		existing.TypeLine = card.TypeLine;
		existing.OracleText = card.OracleText;
		existing.SetCode = card.SetCode;
		existing.SetName = card.SetName;
		existing.CollectorNumber = card.CollectorNumber;
		existing.ImageSmallUrl = card.ImageSmallUrl;
		existing.ImageNormalUrl = card.ImageNormalUrl;
		existing.ImageLargeUrl = card.ImageLargeUrl;
		existing.Power = card.Power;
		existing.Toughness = card.Toughness;
		existing.Rarity = card.Rarity;
		existing.PauperLegality = card.PauperLegality;
		existing.LastFetchedAt = DateTimeOffset.UtcNow;
		existing.InvalidatedAt = null;

		await db.SaveChangesAsync(ct);

		return existing;
	}

	public static async Task<bool> InvalidateAsync(
		DataDbContext db,
		Guid scryfallId,
		CancellationToken ct)
	{
		var existing = await db.Cards
			.FirstOrDefaultAsync(c => c.ScryfallId == scryfallId, ct);

		if (existing is null)
			return false;

		existing.InvalidatedAt = DateTimeOffset.UtcNow;
		await db.SaveChangesAsync(ct);

		return true;
	}

	public static async Task<int> InvalidateAllAsync(
		DataDbContext db,
		CancellationToken ct)
	{
		var cards = await db.Cards
			.Where(c => c.InvalidatedAt == null)
			.ToListAsync(ct);

		var now = DateTimeOffset.UtcNow;

		foreach (var card in cards)
			card.InvalidatedAt = now;

		await db.SaveChangesAsync(ct);
		return cards.Count;
	}
}