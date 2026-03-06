using System;

namespace PauperVault.Core.Domain.Decks;

public class DeckCard
{
	public Guid DeckId { get; set; }
	public Deck Deck { get; set; } = default!;

	public Guid ScryfallId { get; set; }

	public DeckZone Zone { get; set; } = DeckZone.Main;

	public int Quantity { get; set; } = 1;
}