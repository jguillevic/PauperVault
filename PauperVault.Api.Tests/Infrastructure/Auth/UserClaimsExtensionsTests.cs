using PauperVault.Api.Infrastructure.Auth;
using System.Security.Claims;

namespace PauperVault.Api.Tests.Infrastructure.Auth;

[TestClass]
public class UserClaimsExtensionsTests
{
	[TestMethod]
	public void GetUserIdOrThrow_WithValidNameIdentifier_ReturnsUserId()
	{
		// Arrange
		var userId = "test-user-id-123";
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, userId)
		};
		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		// Act
		var result = claimsPrincipal.GetUserIdOrThrow();

		// Assert
		Assert.AreEqual(userId, result);
	}

	[TestMethod]
	public void GetUserIdOrThrow_WithoutNameIdentifier_ThrowsInvalidOperationException()
	{
		// Arrange
		var claims = new List<Claim>();
		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		// Act & Assert
		try
		{
			claimsPrincipal.GetUserIdOrThrow();
			Assert.Fail("Expected InvalidOperationException to be thrown");
		}
		catch (InvalidOperationException ex)
		{
			Assert.Contains("Missing NameIdentifier claim", ex.Message);
		}
	}

	[TestMethod]
	public void GetUserIdOrThrow_WithEmptyNameIdentifier_ThrowsInvalidOperationException()
	{
		// Arrange
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, "")
		};
		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		// Act & Assert
		Assert.Throws<InvalidOperationException>(()  => claimsPrincipal.GetUserIdOrThrow());
	}

	[TestMethod]
	public void GetUserIdOrNull_WithValidNameIdentifier_ReturnsUserId()
	{
		// Arrange
		var userId = "test-user-id-123";
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, userId)
		};
		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		// Act
		var result = claimsPrincipal.GetUserIdOrNull();

		// Assert
		Assert.AreEqual(userId, result);
	}

	[TestMethod]
	public void GetUserIdOrNull_WithoutNameIdentifier_ReturnsNull()
	{
		// Arrange
		var claims = new List<Claim>();
		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		// Act
		var result = claimsPrincipal.GetUserIdOrNull();

		// Assert
		Assert.IsNull(result);
	}

	[TestMethod]
	public void GetUserIdOrNull_WithEmptyNameIdentifier_ReturnsNull()
	{
		// Arrange
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, "")
		};
		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		// Act
		var result = claimsPrincipal.GetUserIdOrNull();

		// Assert
		Assert.IsNull(result);
	}

	[TestMethod]
	public void GetEmailOrNull_WithEmailClaim_ReturnsEmail()
	{
		// Arrange
		var email = "test@example.com";
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Email, email)
		};
		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		// Act
		var result = claimsPrincipal.GetEmailOrNull();

		// Assert
		Assert.AreEqual(email, result);
	}

	[TestMethod]
	public void GetEmailOrNull_WithoutEmailClaim_ReturnsNull()
	{
		// Arrange
		var claims = new List<Claim>();
		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		// Act
		var result = claimsPrincipal.GetEmailOrNull();

		// Assert
		Assert.IsNull(result);
	}

	[TestMethod]
	public void GetEmailOrNull_WithEmptyEmailClaim_ReturnsEmptyString()
	{
		// Arrange
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Email, "")
		};
		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

		// Act
		var result = claimsPrincipal.GetEmailOrNull();

		// Assert
		Assert.AreEqual("", result);
	}
}
