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
	string AuthorName,
	DateTimeOffset UpdatedAt,
	IReadOnlyList<PublicDeckCardDto> Cards,
	bool CanEdit);

public record PublicDeckCardDto(
	Guid ScryfallId,
	string Name,
	DeckZone Zone,
	int Quantity,
	string? ImageUrl,
	string? ManaCost,
	string? TypeLine,
	string? OracleText,
	string? Power,
	string? Toughness,
	string? SetCode,
	string? SetName,
	string? CollectorNumber);