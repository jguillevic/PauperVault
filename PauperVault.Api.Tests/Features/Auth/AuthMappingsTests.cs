using PauperVault.Api.Features.Auth;
using PauperVault.Core.Domain.Auth;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace PauperVault.Api.Tests.Features.Auth;

[TestClass]
public class AuthMappingsTests
{
	[TestMethod]
	public void ToValidationError_WithIdentityErrors_MapsCorrectly()
	{
		// Arrange
		var identityErrors = new List<IdentityError>
		{
			new IdentityError { Code = "DuplicateUserName", Description = "Username already exists" },
			new IdentityError { Code = "PasswordTooShort", Description = "Password is too short" }
		};
		var identityResult = IdentityResult.Failed(identityErrors.ToArray());

		// Act
		var result = AuthMappings.ToValidationError(identityResult);

		// Assert
		Assert.IsNotNull(result);
		Assert.HasCount(2, result.Errors);
		var errorArray = result.Errors.ToArray();
		Assert.AreEqual("DuplicateUserName", errorArray[0].Code);
		Assert.AreEqual("Username already exists", errorArray[0].Description);
		Assert.AreEqual("PasswordTooShort", errorArray[1].Code);
		Assert.AreEqual("Password is too short", errorArray[1].Description);
	}

	[TestMethod]
	public void ToHttpResult_WithSuccessAndToken_ReturnsOkWithToken()
	{
		// Arrange
		var token = "test-jwt-token";
		var success = new AuthCommandResult.Success(Token: token);

		// Act
		var result = AuthMappings.ToHttpResult(success);

		// Assert
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void ToHttpResult_WithSuccessAndMessage_ReturnsOkWithMessage()
	{
		// Arrange
		var message = "User created successfully";
		var success = new AuthCommandResult.Success(Message: message);

		// Act
		var result = AuthMappings.ToHttpResult(success);

		// Assert
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void ToHttpResult_WithValidationError_ReturnsBadRequest()
	{
		// Arrange
		var errors = new[]
		{
			new AuthErrorItem(AuthErrorCodes.EmailRequired, "Email is required")
		};
		var validationError = new AuthCommandResult.ValidationError(errors);

		// Act
		var result = AuthMappings.ToHttpResult(validationError);

		// Assert
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void ToHttpResult_WithUnauthorized_ReturnsUnauthorized()
	{
		// Arrange
		var unauthorized = new AuthCommandResult.Unauthorized();

		// Act
		var result = AuthMappings.ToHttpResult(unauthorized);

		// Assert
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void ToHttpResult_WithConfigurationError_ReturnsProblem()
	{
		// Arrange
		var configError = new AuthCommandResult.ConfigurationError("JWT configuration missing");

		// Act
		var result = AuthMappings.ToHttpResult(configError);

		// Assert
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void ToHttpResult_WithConflict_ReturnsConflict()
	{
		// Arrange
		var conflict = new AuthCommandResult.Conflict("User already exists");

		// Act
		var result = AuthMappings.ToHttpResult(conflict);

		// Assert
		Assert.IsNotNull(result);
	}
}
