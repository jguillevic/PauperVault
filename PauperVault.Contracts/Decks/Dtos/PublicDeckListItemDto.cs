namespace PauperVault.Contracts.Decks.Dtos;

public record PublicDeckListItemDto(
	Guid Id,
	string Name,
	DateTimeOffset UpdatedAt,
	string? CoverImageUrl
);
