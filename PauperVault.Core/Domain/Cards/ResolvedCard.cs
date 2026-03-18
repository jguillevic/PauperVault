namespace PauperVault.Core.Domain.Cards;

public sealed record ResolvedCard(
	Guid ScryfallId,
	string Name,
	string? ManaCost,
	string? TypeLine,
	string? OracleText,
	string? SetCode,
	string? SetName,
	string? CollectorNumber,
	string? ImageSmallUrl,
	string? Power,
	string? Toughness,
	string? Rarity,
	string? PauperLegality
);
