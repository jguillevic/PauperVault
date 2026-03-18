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
		existing.Power = card.Power;
		existing.Toughness = card.Toughness;
		existing.Rarity = card.Rarity;
		existing.PauperLegality = card.PauperLegality;
		existing.LastFetchedAt = DateTimeOffset.UtcNow;

		await db.SaveChangesAsync(ct);

		return existing;
	}
}