namespace PauperVault.Api.Infrastructure.Configuration;

/// <summary>
/// Configuration constants for the application.
/// </summary>
public static class ConfigurationKeys
{
	// Connection Strings
	public const string ConnectionString_AuthDb = "AuthDb";
	public const string ConnectionString_DataDb = "DataDb";

	// JWT Configuration
	public const string JwtIssuer = "Jwt:Issuer";
	public const string JwtAudience = "Jwt:Audience";
	public const string JwtKey = "Jwt:Key";

	// Scryfall Configuration
	public const string ScryfallBaseUrl = "Scryfall:BaseUrl";

	// EF Core
	public const string AuthDbMigrationsHistoryTable = "__EFMigrationsHistory_Auth";
	public const string DataDbMigrationsHistoryTable = "__EFMigrationsHistory_Data";

	// HTTP Client
	public const string ScryfallUserAgent = "PauperVault/1.0";
	public const string JsonMediaType = "application/json";
	public const int ScryfallTimeoutSeconds = 10;

	// Logging
	public const string DbMigrationsLoggerName = "DbMigrations";
	public const string SuccessMigrationLog = "✅ EF Core migrations applied successfully (AuthDb + DataDb).";
	public const string ErrorMigrationLog = "❌ Failed to apply EF Core migrations (AuthDb and/or DataDb).";

	// Environment
	public const string DevelopmentEnvironment = "Development";
}
