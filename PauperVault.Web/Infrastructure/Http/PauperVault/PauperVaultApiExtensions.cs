using Microsoft.Extensions.Options;
using PauperVault.Web.Services.Account;

namespace PauperVault.Web.Infrastructure.Http.PauperVault;

public static class PauperVaultApiExtensions
{
	public static IHostApplicationBuilder AddPauperVaultApi(this IHostApplicationBuilder builder)
	{
		builder.Services.AddOptions<PauperVaultApiOptions>()
			.Bind(builder.Configuration.GetSection(PauperVaultApiOptions.SectionName))
			.Validate(o => !string.IsNullOrWhiteSpace(o.PauperVaultUrl), "Api:PauperVaultUrl is required")
			.ValidateOnStart();

		builder.Services.AddHttpClient<IPauperVaultApiClient, PauperVaultApiClient>((sp, client) =>
		{
			var opts = sp.GetRequiredService<IOptions<PauperVaultApiOptions>>().Value;
			var url = EnsureTrailingSlash(opts.PauperVaultUrl);

			if (!Uri.TryCreate(url, UriKind.Absolute, out var baseUri))
				throw new InvalidOperationException("Api:PauperVaultUrl must be an absolute URL (https://...).");

			client.BaseAddress = baseUri;
			client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
		})
		.AddHttpMessageHandler<PvBearerTokenHandler>();

		return builder;
	}

	private static string EnsureTrailingSlash(string url)
		=> url.EndsWith("/") ? url : url + "/";
}
