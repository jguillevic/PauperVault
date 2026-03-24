using PauperVault.Api.Infrastructure.Configuration;
using PauperVault.Api.Infrastructure.Scryfall;
using System.Net.Http.Headers;

namespace PauperVault.Api.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring HTTP client services.
/// </summary>
public static class HttpClientServiceExtensions
{
	public static IServiceCollection AddApplicationHttpClients(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddHttpClient<ScryfallClient>(http =>
		{
			http.BaseAddress = new Uri(configuration[ConfigurationKeys.ScryfallBaseUrl]!);
			http.Timeout = TimeSpan.FromSeconds(ConfigurationKeys.ScryfallTimeoutSeconds);
			http.DefaultRequestHeaders.UserAgent.ParseAdd(ConfigurationKeys.ScryfallUserAgent);
			http.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue(ConfigurationKeys.JsonMediaType));
		});

		return services;
	}
}
