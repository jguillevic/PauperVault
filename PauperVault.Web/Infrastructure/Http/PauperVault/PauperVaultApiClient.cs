using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace PauperVault.Web.Infrastructure.Http.PauperVault
{
	public sealed class PauperVaultApiClient : IPauperVaultApiClient
	{
		private readonly HttpClient _http;

		public PauperVaultApiClient(HttpClient http) => _http = http;

		private record LoginRequest(string Email, string Password);
		private record LoginResponse(string Token);

		public async Task<string> LoginAsync(string email, string password, CancellationToken ct = default)
		{
			var resp = await _http.PostAsJsonAsync("auth/login", new LoginRequest(email, password), ct);
			resp.EnsureSuccessStatusCode();

			var data = await resp.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: ct);
			if (data is null || string.IsNullOrWhiteSpace(data.Token))
				throw new InvalidOperationException("Token missing from API response.");

			return data.Token;
		}

		private record RegisterRequest(string Email, string Password);

		public async Task RegisterAsync(string email, string password, CancellationToken ct = default)
		{
			var resp = await _http.PostAsJsonAsync("auth/register", new RegisterRequest(email, password), ct);

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
			var resp = await _http.GetAsync("auth/me", ct);

			if (resp.StatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedAccessException("JWT invalid or expired.");

			resp.EnsureSuccessStatusCode();

			var data = await resp.Content.ReadFromJsonAsync<MeApiResponse>(cancellationToken: ct);
			if (data is null)
				throw new InvalidOperationException("Unexpected API response for /auth/me.");

			return new MeResponse(data.UserId, data.Email);
		}
	}
}
