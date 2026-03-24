using Microsoft.EntityFrameworkCore;
using PauperVault.Api.Infrastructure.Configuration;
using PauperVault.Api.Infrastructure.Data;

namespace PauperVault.Api.Infrastructure.Data;

/// <summary>
/// Service for applying database migrations at application startup.
/// </summary>
public static class MigrationService
{
	public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.CreateScope();
		var logger = scope.ServiceProvider
			.GetRequiredService<ILoggerFactory>()
			.CreateLogger(ConfigurationKeys.DbMigrationsLoggerName);

		try
		{
			var authDb = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
			await authDb.Database.MigrateAsync();

			var dataDb = scope.ServiceProvider.GetRequiredService<DataDbContext>();
			await dataDb.Database.MigrateAsync();

			logger.LogInformation(ConfigurationKeys.SuccessMigrationLog);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ConfigurationKeys.ErrorMigrationLog);
			throw;
		}
	}
}
