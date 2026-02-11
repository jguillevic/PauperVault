using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PauperVault.Api.Infrastructure.Auth;

public static class JwtTokenGenerator
{
	public static string GenerateToken(ApplicationUser user, IConfiguration config)
	{
		var key = config["Jwt:Key"]!;
		var issuer = config["Jwt:Issuer"]!;
		var audience = config["Jwt:Audience"]!;
		var expiresMinutes = int.Parse(config["Jwt:ExpiresMinutes"]!);

		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, user.Id),
			new(ClaimTypes.NameIdentifier, user.Id),

			new(ClaimTypes.Email, user.Email ?? ""),
			new(JwtRegisteredClaimNames.Email, user.Email ?? ""),

			new(ClaimTypes.Name, user.UserName ?? user.Email ?? "")
		};

		var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
		var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: issuer,
			audience: audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
			signingCredentials: creds
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}
