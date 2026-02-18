namespace PauperVault.Web.Infrastructure.Http.PauperVault
{
	public interface IPauperVaultApiClient
	{
		Task<string> LoginAsync(string email, string password, CancellationToken ct = default);

		Task RegisterAsync(string email, string password, CancellationToken ct = default);

		Task<MeResponse> MeAsync(CancellationToken ct = default);

		Task<string> GoogleLoginAsync(string idToken, CancellationToken ct = default);
	}
}
