using System;

namespace PauperVault.Core.Domain.Cards;

public class CardCache
{
	public Guid ScryfallId { get; set; }

	public string Name { get; set; } = default!;
	public string? ManaCost { get; set; }
	public string? TypeLine { get; set; }
	public string? OracleText { get; set; }

	public string? SetCode { get; set; }
	public string? CollectorNumber { get; set; }

	public string? ImageSmallUrl { get; set; }

	public string? Rarity { get; set; }
	public string? PauperLegality { get; set; }

	public DateTimeOffset LastFetchedAt { get; set; } = DateTimeOffset.UtcNow;
}