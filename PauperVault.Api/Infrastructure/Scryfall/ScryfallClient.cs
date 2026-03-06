using System.Net;
using System.Net.Http.Json;

namespace PauperVault.Api.Infrastructure.Scryfall;

public sealed class ScryfallClient(
	HttpClient http
)
{
	public async Task<IReadOnlyList<string>> AutocompleteAsync(string q, CancellationToken ct)
	{
		var url = $"cards/autocomplete?q={Uri.EscapeDataString(q)}";
		var resp = await http.GetFromJsonAsync<ScryfallAutocompleteResponse>(url, ct);
		return resp?.Data ?? new List<string>();
	}

	public async Task<ScryfallCard?> ResolveByFuzzyNameAsync(string name, CancellationToken ct)
	{
		var url = $"cards/named?fuzzy={Uri.EscapeDataString(name)}";

		using var res = await http.GetAsync(url, ct);
		if (res.StatusCode == HttpStatusCode.NotFound)
			return null;

		res.EnsureSuccessStatusCode();
		return await res.Content.ReadFromJsonAsync<ScryfallCard>(cancellationToken: ct);
	}
}