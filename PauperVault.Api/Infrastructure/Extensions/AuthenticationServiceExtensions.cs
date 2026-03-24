using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PauperVault.Api.Infrastructure.Configuration;
using System.Text;

namespace PauperVault.Api.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring authentication and authorization services.
/// </summary>
public static class AuthenticationServiceExtensions
{
	public static IServiceCollection AddApplicationAuthentication(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services
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

					ValidIssuer = configuration[ConfigurationKeys.JwtIssuer],
					ValidAudience = configuration[ConfigurationKeys.JwtAudience],
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(configuration[ConfigurationKeys.JwtKey]!)
					)
				};
			});

		services.AddAuthorization();

		return services;
	}
}
