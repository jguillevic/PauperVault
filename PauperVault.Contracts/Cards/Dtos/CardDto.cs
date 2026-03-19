namespace PauperVault.Contracts.Cards.Dto;

public record CardDto(
	Guid ScryfallId,
	string Name,
	string? ManaCost,
	string? TypeLine,
	string? OracleText,
	string? SetCode,
	string? CollectorNumber,
	string? Rarity,
	string? PauperLegality,
	string? ImageSmallUrl,
	string? ImageNormalUrl,
	string? ImageLargeUrl,
	DateTimeOffset? InvalidatedAt
);