using PauperVault.Api.Features.Auth;
using PauperVault.Api.Features.Cards;
using PauperVault.Api.Features.Decks;
using PauperVault.Api.Infrastructure.Configuration;

namespace PauperVault.Api.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring the HTTP request pipeline.
/// </summary>
public static class ApplicationPipelineExtensions
{
	public static WebApplication UseApplicationPipeline(this WebApplication app)
	{
		if (app.Environment.IsEnvironment(ConfigurationKeys.DevelopmentEnvironment))
			app.UseDeveloperExceptionPage();

		app.UseHttpsRedirection();
		app.UseAuthentication();
		app.UseAuthorization();

		app.MapAuthEndpoints();
		app.MapDeckEndpoints();
		app.MapCardEndpoints();

		return app;
	}
}
