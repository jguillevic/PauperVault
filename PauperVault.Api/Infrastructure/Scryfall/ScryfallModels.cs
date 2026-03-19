using System.Text.Json.Serialization;

namespace PauperVault.Api.Infrastructure.Scryfall;

public sealed class ScryfallAutocompleteResponse
{
	[JsonPropertyName("data")]
	public List<string> Data { get; set; } = new();
}

public sealed class ScryfallCardFace
{
	[JsonPropertyName("image_uris")]
	public Dictionary<string, string>? ImageUris { get; set; }
}

public sealed class ScryfallCard
{
	[JsonPropertyName("id")]
	public Guid Id { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; } = default!;

	[JsonPropertyName("mana_cost")]
	public string? ManaCost { get; set; }

	[JsonPropertyName("type_line")]
	public string? TypeLine { get; set; }

	[JsonPropertyName("oracle_text")]
	public string? OracleText { get; set; }

	[JsonPropertyName("set")]
	public string? SetCode { get; set; }

	[JsonPropertyName("set_name")]
	public string? SetName { get; set; }

	[JsonPropertyName("collector_number")]
	public string? CollectorNumber { get; set; }

	[JsonPropertyName("power")]
	public string? Power { get; set; }

	[JsonPropertyName("toughness")]
	public string? Toughness { get; set; }

	[JsonPropertyName("rarity")]
	public string? Rarity { get; set; }

	[JsonPropertyName("legalities")]
	public Dictionary<string, string>? Legalities { get; set; }

	[JsonPropertyName("image_uris")]
	public Dictionary<string, string>? ImageUris { get; set; }

	[JsonPropertyName("card_faces")]
	public List<ScryfallCardFace>? CardFaces { get; set; }
}