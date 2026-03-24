using PauperVault.Api.Features.Auth;

namespace PauperVault.Api.Tests.Features.Auth;

[TestClass]
public class GoogleLoginCommandResultTests
{
	[TestMethod]
	public void Success_WithToken_StoresToken()
	{
		// Arrange
		var token = "google-login-token-123";

		// Act
		var result = new GoogleLoginCommandResult.Success(token);

		// Assert
		Assert.AreEqual(token, result.Token);
	}

	[TestMethod]
	public void ValidationError_WithPayload_StoresPayload()
	{
		// Arrange
		var payload = new { error = "Invalid token" };

		// Act
		var result = new GoogleLoginCommandResult.ValidationError(payload);

		// Assert
		Assert.AreEqual(payload, result.Payload);
	}

	[TestMethod]
	public void Success_TwoInstancesWithSameToken_AreEqual()
	{
		// Arrange
		var token = "test-token";
		var result1 = new GoogleLoginCommandResult.Success(token);
		var result2 = new GoogleLoginCommandResult.Success(token);

		// Act & Assert
		Assert.AreEqual(result1, result2);
	}

	[TestMethod]
	public void Success_TwoInstancesWithDifferentTokens_AreNotEqual()
	{
		// Arrange
		var result1 = new GoogleLoginCommandResult.Success("token1");
		var result2 = new GoogleLoginCommandResult.Success("token2");

		// Act & Assert
		Assert.AreNotEqual(result1, result2);
	}
}
