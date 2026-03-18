using PauperVault.Core.Domain.Decks;

namespace PauperVault.Contracts.Decks.Dtos;

public record DeckCardDto(
	Guid ScryfallId,
	string Name, 
	DeckZone Zone, 
	int Quantity
);
