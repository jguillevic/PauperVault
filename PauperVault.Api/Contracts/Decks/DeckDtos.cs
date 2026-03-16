using PauperVault.Core.Domain.Decks;

namespace PauperVault.Api.Contracts.Decks;

public record DeckListItemDto(Guid Id, string Name, DateTimeOffset UpdatedAt);

public record DeckCardDto(Guid ScryfallId, string Name, DeckZone Zone, int Quantity);

public record DeckDetailsDto(
	Guid Id,
	string Name,
	string? Description,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt,
	IReadOnlyList<DeckCardDto> Cards);

public record PublicDeckListItemDto(
	Guid Id,
	string Name,
	DateTimeOffset UpdatedAt,
	string? CoverImageUrl);

public record PublicDeckDetailsDto(
	Guid Id,
	string Name,
	string? Description,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt,
	IReadOnlyList<DeckCardDto> Cards,
	bool CanEdit);