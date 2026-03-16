using Microsoft.EntityFrameworkCore;
using PauperVault.Api.Infrastructure.Data;
using PauperVault.Core.Domain.Decks;

namespace PauperVault.Api.Features.Decks;

public static class DeckCommands
{
	public static async Task<Guid> CreateDeckAsync(
		DataDbContext db,
		string userId,
		string name,
		string? description,
		CancellationToken ct)
	{
		var now = DateTimeOffset.UtcNow;

		var deck 
			= Deck.Create(
				userId,
				name,
				description,
				now
			);

		db.Decks.Add(deck);
		await db.SaveChangesAsync(ct);

		return deck.Id;
	}

	public static async Task<bool> UpdateDeckAsync(
		DataDbContext db,
		Guid deckId,
		string userId,
		string name,
		string? description,
		CancellationToken ct)
	{
		var deck = await FindOwnedDeckAsync(db, deckId, userId, ct);
		if (deck is null)
			return false;

		deck.Rename(name, description);

		await db.SaveChangesAsync(ct);
		return true;
	}

	public static async Task<bool> DeleteDeckAsync(
		DataDbContext db,
		Guid deckId,
		string userId,
		CancellationToken ct)
	{
		var deck = await FindOwnedDeckAsync(db, deckId, userId, ct);
		if (deck is null)
			return false;

		db.Decks.Remove(deck);
		await db.SaveChangesAsync(ct);

		return true;
	}

	public static async Task<bool> UpsertDeckCardAsync(
		DataDbContext db,
		Guid deckId,
		string userId,
		Guid scryfallId,
		DeckZone zone,
		int quantity,
		CancellationToken ct)
	{
		var deck = await db.Decks
			.Include(d => d.Cards)
			.FirstOrDefaultAsync(d => d.Id == deckId && d.OwnerUserId == userId, ct);

		if (deck is null)
			return false;

		deck.AddOrUpdateCard(scryfallId, zone, quantity);

		await db.SaveChangesAsync(ct);
		return true;
	}

	public static async Task<bool> DeleteDeckCardAsync(
		DataDbContext db,
		Guid deckId,
		string userId,
		Guid scryfallId,
		DeckZone zone,
		CancellationToken ct)
	{
		var deck = await db.Decks
			.Include(d => d.Cards)
			.FirstOrDefaultAsync(d => d.Id == deckId && d.OwnerUserId == userId, ct);

		if (deck is null)
			return false;

		var removed = deck.RemoveCard(scryfallId, zone);
		if (!removed)
			return false;

		await db.SaveChangesAsync(ct);
		return true;
	}

	private static Task<Deck?> FindOwnedDeckAsync(
		DataDbContext db,
		Guid deckId,
		string userId,
		CancellationToken ct)
	{
		return db.Decks.FirstOrDefaultAsync(
			d => d.Id == deckId && d.OwnerUserId == userId,
			ct);
	}
}