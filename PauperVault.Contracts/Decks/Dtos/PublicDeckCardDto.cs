using PauperVault.Core.Domain.Decks;

namespace PauperVault.Contracts.Decks.Dtos;

public record PublicDeckCardDto(
	Guid ScryfallId,
	string Name,
	DeckZone Zone,
	int Quantity,
	string? ImageSmallUrl,
	string? ImageNormalUrl,
	string? ImageLargeUrl,
	string? ManaCost,
	string? TypeLine,
	string? OracleText,
	string? Power,
	string? Toughness,
	string? SetCode,
	string? SetName,
	string? CollectorNumber
)
{
	public string? ImageUrl => ImageNormalUrl ?? ImageSmallUrl ?? ImageLargeUrl;
}
