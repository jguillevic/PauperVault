using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PauperVault.Api.Endpoints;
using PauperVault.Api.Infrastructure.Auth;
using PauperVault.Api.Infrastructure.Data;
using PauperVault.Api.Infrastructure.Scryfall;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Bases de données
builder.Services.AddDbContext<AuthDbContext>(options =>
{
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("AuthDb"),
		sql => sql.MigrationsHistoryTable("__EFMigrationsHistory_Auth"));
});

builder.Services.AddDbContext<DataDbContext>(options =>
{
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DataDb"),
		sql => sql.MigrationsHistoryTable("__EFMigrationsHistory_Data"));
});

// 2. ASP.NET Core Identity
builder.Services
	.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<AuthDbContext>()
	.AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();

// Auth JWT
builder.Services
	.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,

			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
			)
		};
	});

builder.Services.AddAuthorization();

builder.Services.AddHttpClient<ScryfallClient>(http =>
{
	http.BaseAddress = new Uri("https://api.scryfall.com/");
	http.Timeout = TimeSpan.FromSeconds(10);
	http.DefaultRequestHeaders.UserAgent.ParseAdd("PauperVault/1.0");
	http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
	app.UseDeveloperExceptionPage();

// ✅ Appliquer les migrations au démarrage (AuthDb + DataDb)
using (var scope = app.Services.CreateScope())
{
	var logger = scope.ServiceProvider
		.GetRequiredService<ILoggerFactory>()
		.CreateLogger("DbMigrations");

	try
	{
		var authDb = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
		authDb.Database.Migrate();

		var dataDb = scope.ServiceProvider.GetRequiredService<DataDbContext>();
		dataDb.Database.Migrate();

		logger.LogInformation("✅ EF Core migrations applied successfully (AuthDb + DataDb).");
	}
	catch (Exception ex)
	{
		logger.LogError(ex, "❌ Failed to apply EF Core migrations (AuthDb and/or DataDb).");
		throw;
	}
}

// 3. Pipeline HTTP minimal
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapDeckEndpoints();
app.MapCardEndpoints();

app.Run();