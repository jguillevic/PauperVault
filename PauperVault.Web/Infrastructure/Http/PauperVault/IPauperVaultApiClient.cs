using PauperVault.Web.Contracts.Cards;
using PauperVault.Web.Contracts.Decks;

namespace PauperVault.Web.Infrastructure.Http.PauperVault
{
	public interface IPauperVaultApiClient
	{
		// Account

		Task<string> LoginAsync(string email, string password, CancellationToken ct = default);

		Task RegisterAsync(string email, string password, CancellationToken ct = default);

		Task<MeResponse> MeAsync(CancellationToken ct = default);

		Task<string> GoogleLoginAsync(string idToken, CancellationToken ct = default);

		// Decks

		Task<IReadOnlyList<DeckListItemDto>> GetDecksAsync(CancellationToken ct = default);

		Task<Guid> CreateDeckAsync(CreateDeckRequest request, CancellationToken ct = default);

		Task<DeckDetailsDto> GetDeckAsync(Guid deckId, CancellationToken ct = default);

		Task UpdateDeckAsync(Guid deckId, UpdateDeckRequest request, CancellationToken ct = default);

		Task DeleteDeckAsync(Guid deckId, CancellationToken ct = default);

		Task AddOrUpdateDeckCardAsync(Guid deckId, AddOrUpdateDeckCardRequest request, CancellationToken ct = default);

		Task<IReadOnlyList<PublicDeckListItemDto>> GetPublicDecksAsync(
			int skip = 0,
			int take = 10,
			CancellationToken ct = default
		);

		// Cards

		Task<CardAutocompleteDto> AutocompleteCardsAsync(string q, CancellationToken ct = default);

		Task<CardDto> ResolveCardAsync(string name, CancellationToken ct = default);
	}
}
