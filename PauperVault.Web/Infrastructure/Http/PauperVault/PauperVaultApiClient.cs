using PauperVault.Web.Contracts.Cards;
using PauperVault.Web.Contracts.Decks;
using System.Net;
using System.Text.Json.Serialization;

namespace PauperVault.Web.Infrastructure.Http.PauperVault
{
	public sealed class PauperVaultApiClient(HttpClient http) : IPauperVaultApiClient
	{
		private record LoginRequest(string Email, string Password);
		private record LoginResponse(string Token);

		public async Task<string> LoginAsync(string email, string password, CancellationToken ct = default)
		{
			var resp = await http.PostAsJsonAsync("auth/login", new LoginRequest(email, password), ct);
			resp.EnsureSuccessStatusCode();

			var data = await resp.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: ct);
			if (data is null || string.IsNullOrWhiteSpace(data.Token))
				throw new InvalidOperationException("Token missing from API response.");

			return data.Token;
		}

		private record RegisterRequest(string Email, string Password);

		public async Task RegisterAsync(string email, string password, CancellationToken ct = default)
		{
			var resp = await http.PostAsJsonAsync("auth/register", new RegisterRequest(email, password), ct);

			// Si l'API renvoie 400 avec des erreurs, on renvoie un message lisible
			if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				var body = await resp.Content.ReadAsStringAsync(ct);
				throw new InvalidOperationException(body);
			}

			resp.EnsureSuccessStatusCode();
		}

		private sealed record MeApiResponse(
			[property: JsonPropertyName("userId")] string UserId,
			[property: JsonPropertyName("email")] string Email
		);

		public async Task<MeResponse> MeAsync(CancellationToken ct = default)
		{
			var resp = await http.GetAsync("auth/me", ct);

			if (resp.StatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedAccessException("JWT invalid or expired.");

			resp.EnsureSuccessStatusCode();

			var data = await resp.Content.ReadFromJsonAsync<MeApiResponse>(cancellationToken: ct);
			if (data is null)
				throw new InvalidOperationException("Unexpected API response for /auth/me.");

			return new MeResponse(data.UserId, data.Email);
		}

		private sealed record GoogleLoginRequest(string IdToken);

		public async Task<string> GoogleLoginAsync(string idToken, CancellationToken ct = default)
		{
			var resp = await http.PostAsJsonAsync("auth/google", new GoogleLoginRequest(idToken), ct);
			resp.EnsureSuccessStatusCode();

			var data = await resp.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: ct);
			if (data is null || string.IsNullOrWhiteSpace(data.Token))
				throw new InvalidOperationException("Token missing from API response.");

			return data.Token;
		}

		public async Task<IReadOnlyList<DeckListItemDto>> GetDecksAsync(CancellationToken ct = default)
		{
			var res = await http.GetAsync("/decks", ct);
			res.EnsureSuccessStatusCode();
			return (await res.Content.ReadFromJsonAsync<List<DeckListItemDto>>(cancellationToken: ct))!;
		}

		public async Task<Guid> CreateDeckAsync(CreateDeckRequest request, CancellationToken ct = default)
		{
			var res = await http.PostAsJsonAsync("/decks", request, ct);
			res.EnsureSuccessStatusCode();

			var payload = await res.Content.ReadFromJsonAsync<CreateDeckResponse>(cancellationToken: ct);
			return payload!.Id;
		}

		public async Task<DeckDetailsDto> GetDeckAsync(Guid deckId, CancellationToken ct = default)
		{
			var res = await http.GetAsync($"/decks/{deckId}", ct);
			res.EnsureSuccessStatusCode();
			return (await res.Content.ReadFromJsonAsync<DeckDetailsDto>(cancellationToken: ct))!;
		}

		public async Task UpdateDeckAsync(Guid deckId, UpdateDeckRequest request, CancellationToken ct = default)
		{
			var res = await http.PutAsJsonAsync($"/decks/{deckId}", request, ct);
			res.EnsureSuccessStatusCode();
		}

		public async Task DeleteDeckAsync(Guid deckId, CancellationToken ct = default)
		{
			var res = await http.DeleteAsync($"/decks/{deckId}", ct);
			res.EnsureSuccessStatusCode();
		}

		public async Task AddOrUpdateDeckCardAsync(Guid deckId, AddOrUpdateDeckCardRequest request, CancellationToken ct = default)
		{
			var res = await http.PostAsJsonAsync($"/decks/{deckId}/cards", request, ct);
			res.EnsureSuccessStatusCode();
		}

		public async Task<CardAutocompleteDto> AutocompleteCardsAsync(string q, CancellationToken ct = default)
		{
			return await http.GetFromJsonAsync<CardAutocompleteDto>($"/cards/autocomplete?q={Uri.EscapeDataString(q)}", ct)
				?? new CardAutocompleteDto(Array.Empty<string>());
		}

		public async Task<CardDto> ResolveCardAsync(string name, CancellationToken ct = default)
		{
			var res = await http.GetAsync($"/cards/resolve?name={Uri.EscapeDataString(name)}", ct);
			res.EnsureSuccessStatusCode();
			return (await res.Content.ReadFromJsonAsync<CardDto>(cancellationToken: ct))!;
		}
	}
}
