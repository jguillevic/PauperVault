using PauperVault.Core.Domain.Decks;

namespace PauperVault.Api.Contracts.Decks;

public record DeckListItemDto(Guid Id, string Name, DateTimeOffset UpdatedAt);

public record DeckCardDto(Guid ScryfallId, DeckZone Zone, int Quantity);

public record DeckDetailsDto(
	Guid Id,
	string Name,
	string? Description,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt,
	IReadOnlyList<DeckCardDto> Cards);