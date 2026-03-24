using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PauperVault.Api.Infrastructure.Auth;
using PauperVault.Api.Infrastructure.Configuration;
using PauperVault.Api.Infrastructure.Data;

namespace PauperVault.Api.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring database services.
/// </summary>
public static class DatabaseServiceExtensions
{
	public static IServiceCollection AddApplicationDatabases(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddDbContext<AuthDbContext>(options =>
		{
			options.UseSqlServer(
				configuration.GetConnectionString(ConfigurationKeys.ConnectionString_AuthDb),
				sql => sql.MigrationsHistoryTable(ConfigurationKeys.AuthDbMigrationsHistoryTable));
		});

		services.AddDbContext<DataDbContext>(options =>
		{
			options.UseSqlServer(
				configuration.GetConnectionString(ConfigurationKeys.ConnectionString_DataDb),
				sql => sql.MigrationsHistoryTable(ConfigurationKeys.DataDbMigrationsHistoryTable));
		});

		return services;
	}

	public static IServiceCollection AddApplicationIdentity(this IServiceCollection services)
	{
		services
			.AddIdentity<ApplicationUser, IdentityRole>()
			.AddEntityFrameworkStores<AuthDbContext>()
			.AddDefaultTokenProviders();

		return services;
	}
}
