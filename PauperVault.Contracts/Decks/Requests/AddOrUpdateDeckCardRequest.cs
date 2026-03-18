using PauperVault.Core.Domain.Decks;

namespace PauperVault.Contracts.Decks.Requests;

public record AddOrUpdateDeckCardRequest(
	Guid ScryfallId,
	DeckZone Zone, 
	int Quantity
);