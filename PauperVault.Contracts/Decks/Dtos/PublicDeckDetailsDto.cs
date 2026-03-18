namespace PauperVault.Contracts.Decks.Dtos;

public record PublicDeckDetailsDto(
	Guid Id,
	string Name,
	string? Description,
	string AuthorName,
	DateTimeOffset UpdatedAt,
	IReadOnlyList<PublicDeckCardDto> Cards,
	bool CanEdit
);
