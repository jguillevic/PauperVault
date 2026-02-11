using PauperVault.Web.Services.Account;

namespace PauperVault.Web.Bootstrap;

public static class ServiceCollectionExtensions
{
	public static void AddPauperVaultServices(this WebApplicationBuilder builder)
	{
		builder.Services.AddRazorPages();
		builder.Services.AddHttpContextAccessor();

		builder.Services.AddScoped<ITokenStore, CookieTokenStore>();
		builder.Services.AddScoped<IAccountService, AccountService>();
		builder.Services.AddTransient<PvBearerTokenHandler>();
	}
}
