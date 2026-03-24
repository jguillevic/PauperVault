namespace PauperVault.Api.Infrastructure.Configuration;

/// <summary>
/// Configuration constants for the application.
/// </summary>
internal static class ConfigurationKeys
{
	// Connection Strings
	internal const string ConnectionString_AuthDb = "AuthDb";
	internal const string ConnectionString_DataDb = "DataDb";

	// JWT Configuration
	internal const string JwtIssuer = "Jwt:Issuer";
	internal const string JwtAudience = "Jwt:Audience";
	internal const string JwtKey = "Jwt:Key";
	internal const string JwtExpiresMinutes = "Jwt:ExpiresMinutes";

	// Scryfall Configuration
	internal const string ScryfallBaseUrl = "Scryfall:BaseUrl";

	// EF Core
	internal const string AuthDbMigrationsHistoryTable = "__EFMigrationsHistory_Auth";
	internal const string DataDbMigrationsHistoryTable = "__EFMigrationsHistory_Data";

	// HTTP Client
	internal const string ScryfallUserAgent = "PauperVault/1.0";
	internal const string JsonMediaType = "application/json";
	internal const int ScryfallTimeoutSeconds = 10;

	// Logging
	internal const string DbMigrationsLoggerName = "DbMigrations";
	internal const string SuccessMigrationLog = "✅ EF Core migrations applied successfully (AuthDb + DataDb).";
	internal const string ErrorMigrationLog = "❌ Failed to apply EF Core migrations (AuthDb and/or DataDb).";

	// Environment
	internal const string DevelopmentEnvironment = "Development";

	internal const string SmallVersion = "small";
	internal const string NormalVersion = "normal";
	internal const string LargeVersion = "large";
}
