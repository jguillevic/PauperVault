using Microsoft.Extensions.Configuration;
using Moq;
using PauperVault.Api.Infrastructure.Auth;
using System.IdentityModel.Tokens.Jwt;

namespace PauperVault.Api.Tests.Infrastructure.Auth;

[TestClass]
public class JwtTokenGeneratorTests
{
	private Mock<IConfiguration> _mockConfig = new Mock<IConfiguration>();

	[TestInitialize]
	public void Setup()
	{
		_mockConfig = new Mock<IConfiguration>();
	}

	[TestMethod]
	public void GenerateToken_WithValidUser_ReturnsValidToken()
	{
		// Arrange
		var user = new ApplicationUser
		{
			Id = "test-user-123",
			Email = "test@example.com",
			UserName = "testuser"
		};

		_mockConfig.Setup(c => c["Jwt:Key"]).Returns("test-secret-key-that-is-long-enough-for-encryption");
		_mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
		_mockConfig.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
		_mockConfig.Setup(c => c["Jwt:ExpiresMinutes"]).Returns("60");

		// Act
		var token = JwtTokenGenerator.GenerateToken(user, _mockConfig.Object);

		// Assert
		Assert.IsNotNull(token);
		Assert.IsFalse(string.IsNullOrWhiteSpace(token));
	}

	[TestMethod]
	public void GenerateToken_GeneratedToken_CanBeRead()
	{
		// Arrange
		var user = new ApplicationUser
		{
			Id = "user-123",
			Email = "user@example.com",
			UserName = "username"
		};

		_mockConfig.Setup(c => c["Jwt:Key"]).Returns("test-secret-key-that-is-long-enough-for-encryption");
		_mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
		_mockConfig.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
		_mockConfig.Setup(c => c["Jwt:ExpiresMinutes"]).Returns("60");

		// Act
		var token = JwtTokenGenerator.GenerateToken(user, _mockConfig.Object);
		var handler = new JwtSecurityTokenHandler();
		var jwtToken = handler.ReadJwtToken(token);

		// Assert
		Assert.IsNotNull(jwtToken);
		var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
		Assert.IsNotNull(subClaim);
		Assert.AreEqual("user-123", subClaim.Value);
	}

	[TestMethod]
	public void GenerateToken_WithUserEmail_IncludesEmailInClaims()
	{
		// Arrange
		var email = "test@example.com";
		var user = new ApplicationUser
		{
			Id = "user-123",
			Email = email,
			UserName = "username"
		};

		_mockConfig.Setup(c => c["Jwt:Key"]).Returns("test-secret-key-that-is-long-enough-for-encryption");
		_mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
		_mockConfig.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
		_mockConfig.Setup(c => c["Jwt:ExpiresMinutes"]).Returns("60");

		// Act
		var token = JwtTokenGenerator.GenerateToken(user, _mockConfig.Object);
		var handler = new JwtSecurityTokenHandler();
		var jwtToken = handler.ReadJwtToken(token);

		// Assert
		var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email);
		Assert.IsNotNull(emailClaim);
		Assert.AreEqual(email, emailClaim.Value);
	}

	[TestMethod]
	public void GenerateToken_WithUserName_IncludesNameInClaims()
	{
		// Arrange
		var userName = "testuser";
		var user = new ApplicationUser
		{
			Id = "user-123",
			Email = "test@example.com",
			UserName = userName
		};

		_mockConfig.Setup(c => c["Jwt:Key"]).Returns("test-secret-key-that-is-long-enough-for-encryption");
		_mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
		_mockConfig.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
		_mockConfig.Setup(c => c["Jwt:ExpiresMinutes"]).Returns("60");

		// Act
		var token = JwtTokenGenerator.GenerateToken(user, _mockConfig.Object);
		var handler = new JwtSecurityTokenHandler();
		var jwtToken = handler.ReadJwtToken(token);

		// Assert
		var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name);
		Assert.IsNotNull(nameClaim);
		Assert.AreEqual(userName, nameClaim.Value);
	}

	[TestMethod]
	public void GenerateToken_WithNullUserEmail_HandlesMissing()
	{
		// Arrange
		var user = new ApplicationUser
		{
			Id = "user-123",
			Email = null,
			UserName = "username"
		};

		_mockConfig.Setup(c => c["Jwt:Key"]).Returns("test-secret-key-that-is-long-enough-for-encryption");
		_mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
		_mockConfig.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
		_mockConfig.Setup(c => c["Jwt:ExpiresMinutes"]).Returns("60");

		// Act
		var token = JwtTokenGenerator.GenerateToken(user, _mockConfig.Object);

		// Assert
		Assert.IsNotNull(token);
		Assert.IsFalse(string.IsNullOrWhiteSpace(token));
	}

	[TestMethod]
	public void GenerateToken_WithDifferentExpirations_TokensHaveDifferentExpiry()
	{
		// Arrange
		var user = new ApplicationUser
		{
			Id = "user-123",
			Email = "test@example.com",
			UserName = "username"
		};

		_mockConfig.Setup(c => c["Jwt:Key"]).Returns("test-secret-key-that-is-long-enough-for-encryption");
		_mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
		_mockConfig.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

		// Token 1 with 30 minutes
		_mockConfig.Setup(c => c["Jwt:ExpiresMinutes"]).Returns("30");
		var token1 = JwtTokenGenerator.GenerateToken(user, _mockConfig.Object);
		var handler = new JwtSecurityTokenHandler();
		var jwtToken1 = handler.ReadJwtToken(token1);
		var expiry1 = jwtToken1.ValidTo;

		// Token 2 with 60 minutes
		_mockConfig.Setup(c => c["Jwt:ExpiresMinutes"]).Returns("60");
		var token2 = JwtTokenGenerator.GenerateToken(user, _mockConfig.Object);
		var jwtToken2 = handler.ReadJwtToken(token2);
		var expiry2 = jwtToken2.ValidTo;

		// Assert
		Assert.IsTrue(expiry2 > expiry1);
	}
}
