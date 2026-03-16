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
		private record RegisterRequest(string Email, string Password);
		private sealed record GoogleLoginRequest(string IdToken);

		private sealed record MeApiResponse(
			[property: JsonPropertyName("userId")] string UserId,
			[property: JsonPropertyName("email")] string Email
		);

		private static async Task ThrowIfErrorAsync(
			HttpResponseMessage response,
			string operationName,
			CancellationToken ct = default)
		{
			if (response.IsSuccessStatusCode)
				return;

			if (response.StatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedAccessException("Accès non autorisé ou session expirée.");

			if (response.StatusCode == HttpStatusCode.BadRequest)
				throw new InvalidOperationException("La requête envoyée à l'API est invalide.");

			var body = await response.Content.ReadAsStringAsync(ct);

			throw new HttpRequestException(
				$"Erreur API pendant '{operationName}'. Statut HTTP : {(int)response.StatusCode} ({response.StatusCode})." +
				(string.IsNullOrWhiteSpace(body) ? string.Empty : $" Réponse : {body}")
			);
		}

		private static async Task<T> ReadRequiredJsonAsync<T>(
			HttpResponseMessage response,
			string operationName,
			CancellationToken ct = default)
		{
			var data = await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);

			if (data is null)
				throw new InvalidOperationException($"Réponse API invalide ou vide pour '{operationName}'.");

			return data;
		}

		public async Task<string> LoginAsync(string email, string password, CancellationToken ct = default)
		{
			var resp = await http.PostAsJsonAsync("auth/login", new LoginRequest(email, password), ct);

			if (resp.StatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedAccessException("Adresse e-mail ou mot de passe invalide.");

			if (resp.StatusCode == HttpStatusCode.BadRequest)
				throw new InvalidOperationException("Les informations de connexion sont invalides.");

			await ThrowIfErrorAsync(resp, "auth/login", ct);

			var data = await ReadRequiredJsonAsync<LoginResponse>(resp, "auth/login", ct);

			if (string.IsNullOrWhiteSpace(data.Token))
				throw new InvalidOperationException("La réponse de connexion ne contient pas de jeton valide.");

			return data.Token;
		}

		public async Task RegisterAsync(string email, string password, CancellationToken ct = default)
		{
			var resp = await http.PostAsJsonAsync("auth/register", new RegisterRequest(email, password), ct);

			if (resp.StatusCode == HttpStatusCode.BadRequest)
				throw new InvalidOperationException("Les informations d'inscription sont invalides.");

			await ThrowIfErrorAsync(resp, "auth/register", ct);
		}

		public async Task<MeResponse> MeAsync(CancellationToken ct = default)
		{
			var resp = await http.GetAsync("auth/me", ct);

			if (resp.StatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedAccessException("Session invalide ou expirée.");

			await ThrowIfErrorAsync(resp, "auth/me", ct);

			var data = await ReadRequiredJsonAsync<MeApiResponse>(resp, "auth/me", ct);

			return new MeResponse(data.UserId, data.Email);
		}

		public async Task<string> GoogleLoginAsync(string idToken, CancellationToken ct = default)
		{
			var resp = await http.PostAsJsonAsync("auth/google", new GoogleLoginRequest(idToken), ct);

			if (resp.StatusCode == HttpStatusCode.BadRequest)
				throw new InvalidOperationException("Connexion Google impossible.");

			if (resp.StatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedAccessException("Connexion Google refusée.");

			await ThrowIfErrorAsync(resp, "auth/google", ct);

			var data = await ReadRequiredJsonAsync<LoginResponse>(resp, "auth/google", ct);

			if (string.IsNullOrWhiteSpace(data.Token))
				throw new InvalidOperationException("La réponse de connexion Google ne contient pas de jeton valide.");

			return data.Token;
		}

		public async Task<IReadOnlyList<DeckListItemDto>> GetDecksAsync(CancellationToken ct = default)
		{
			var res = await http.GetAsync("/decks", ct);

			if (res.StatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedAccessException("Accès non autorisé.");

			return await ReadRequiredJsonAsync<List<DeckListItemDto>>(res, "/decks", ct);
		}

		public async Task<Guid> CreateDeckAsync(CreateDeckRequest request, CancellationToken ct = default)
		{
			var res = await http.PostAsJsonAsync("/decks", request, ct);

			if (res.StatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedAccessException("Accès non autorisé.");

			if (res.StatusCode == HttpStatusCode.BadRequest)
				throw new InvalidOperationException("La demande de création du deck est invalide.");

			await ThrowIfErrorAsync(res, "POST /decks", ct);

			var payload = await ReadRequiredJsonAsync<CreateDeckResponse>(res, "POST /decks", ct);

			return payload.Id;
		}

		public async Task<DeckDetailsDto> GetDeckAsync(Guid deckId, CancellationToken ct = default)
		{
			var res = await http.GetAsync($"/decks/{deckId}", ct);

			if (res.StatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedAccessException("Accès non autorisé.");

			await ThrowIfErrorAsync(res, $"GET /decks/{deckId}", ct);

			return await ReadRequiredJsonAsync<DeckDetailsDto>(res, $"GET /decks/{deckId}", ct);
		}

		public async Task UpdateDeckAsync(Guid deckId, UpdateDeckRequest request, CancellationToken ct = default)
		{
			var res = await http.PutAsJsonAsync($"/decks/{deckId}", request, ct);

			if (res.StatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedAccessException("Accès non autorisé.");

			if (res.StatusCode == HttpStatusCode.BadRequest)
				throw new InvalidOperationException("La mise à jour du deck est invalide.");

			await ThrowIfErrorAsync(res, $"PUT /decks/{deckId}", ct);
		}

		public async Task DeleteDeckAsync(Guid deckId, CancellationToken ct = default)
		{
			var res = await http.DeleteAsync($"/decks/{deckId}", ct);

			if (res.StatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedAccessException("Accès non autorisé.");

			await ThrowIfErrorAsync(res, $"DELETE /decks/{deckId}", ct);
		}

		public async Task AddOrUpdateDeckCardAsync(Guid deckId, AddOrUpdateDeckCardRequest request, CancellationToken ct = default)
		{
			var res = await http.PostAsJsonAsync($"/decks/{deckId}/cards", request, ct);

			if (res.StatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedAccessException("Accès non autorisé.");

			if (res.StatusCode == HttpStatusCode.BadRequest)
				throw new InvalidOperationException("La carte ne peut pas être ajoutée au deck.");

			await ThrowIfErrorAsync(res, $"POST /decks/{deckId}/cards", ct);
		}

		public async Task<IReadOnlyList<PublicDeckListItemDto>> GetPublicDecksAsync(
			int skip = 0,
			int take = 10,
			CancellationToken ct = default
		)
		{
			var res = await http.GetAsync($"/public/decks?skip={skip}&take={take}", ct);

			await ThrowIfErrorAsync(res, "GET /public/decks", ct);

			return await ReadRequiredJsonAsync<List<PublicDeckListItemDto>>(res, "GET /public/decks", ct);
		}

		public async Task<PublicDeckDetailsDto> GetPublicDeckAsync(Guid deckId, CancellationToken ct = default)
		{
			var res = await http.GetAsync($"/public/decks/{deckId}", ct);

			await ThrowIfErrorAsync(res, $"GET /public/decks/{deckId}", ct);

			return await ReadRequiredJsonAsync<PublicDeckDetailsDto>(res, $"GET /public/decks/{deckId}", ct);
		}

		public async Task<CardAutocompleteDto> AutocompleteCardsAsync(string q, CancellationToken ct = default)
		{
			return await http.GetFromJsonAsync<CardAutocompleteDto>($"/cards/autocomplete?q={Uri.EscapeDataString(q)}", ct)
				?? new CardAutocompleteDto(Array.Empty<string>());
		}

		public async Task<CardDto> ResolveCardAsync(string name, CancellationToken ct = default)
		{
			var res = await http.GetAsync($"/cards/resolve?name={Uri.EscapeDataString(name)}", ct);

			await ThrowIfErrorAsync(res, "GET /cards/resolve", ct);

			return await ReadRequiredJsonAsync<CardDto>(res, "GET /cards/resolve", ct);
		}
	}
}