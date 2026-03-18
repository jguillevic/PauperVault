namespace PauperVault.Contracts.Decks.Dtos;

public record DeckListItemDto(
	Guid Id, 
	string Name, 
	DateTimeOffset UpdatedAt
);
