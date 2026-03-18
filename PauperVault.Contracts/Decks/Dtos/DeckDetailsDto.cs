namespace PauperVault.Contracts.Decks.Dtos;

public record DeckDetailsDto(
	Guid Id,
	string Name,
	string? Description,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt,
	IReadOnlyList<DeckCardDto> Cards
);