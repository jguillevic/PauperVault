using System;

namespace PauperVault.Core.Domain.Decks;

public class Deck
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public string OwnerUserId { get; set; } = default!;

	public string Name { get; set; } = default!;
	public string? Description { get; set; }

	public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

	public List<DeckCard> Cards { get; set; } = new();
}