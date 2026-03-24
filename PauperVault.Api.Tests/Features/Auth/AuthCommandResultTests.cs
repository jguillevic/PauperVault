using PauperVault.Api.Features.Auth;

namespace PauperVault.Api.Tests.Features.Auth;

[TestClass]
public class AuthCommandResultTests
{
	[TestMethod]
	public void Success_WithTokenOnly_StoresToken()
	{
		// Arrange
		var token = "test-token-123";

		// Act
		var result = new AuthCommandResult.Success(Token: token);

		// Assert
		Assert.AreEqual(token, result.Token);
		Assert.IsNull(result.Message);
	}

	[TestMethod]
	public void Success_WithMessageOnly_StoresMessage()
	{
		// Arrange
		var message = "User created successfully";

		// Act
		var result = new AuthCommandResult.Success(Message: message);

		// Assert
		Assert.IsNull(result.Token);
		Assert.AreEqual(message, result.Message);
	}

	[TestMethod]
	public void Success_WithBothTokenAndMessage_StoresBoth()
	{
		// Arrange
		var token = "test-token-123";
		var message = "Login successful";

		// Act
		var result = new AuthCommandResult.Success(Token: token, Message: message);

		// Assert
		Assert.AreEqual(token, result.Token);
		Assert.AreEqual(message, result.Message);
	}

	[TestMethod]
	public void ValidationError_WithErrors_StoresErrors()
	{
		// Arrange
		var errors = new[]
		{
			new AuthErrorItem("Code1", "Description1"),
			new AuthErrorItem("Code2", "Description2")
		};

		// Act
		var result = new AuthCommandResult.ValidationError(errors);

		// Assert
		Assert.IsNotNull(result.Errors);
		Assert.AreEqual(2, result.Errors.Count);
	}

	[TestMethod]
	public void Unauthorized_StoresMessage()
	{
		// Arrange
		var message = "Invalid credentials";

		// Act
		var result = new AuthCommandResult.Unauthorized(message);

		// Assert
		Assert.AreEqual(message, result.Message);
	}

	[TestMethod]
	public void Unauthorized_WithoutMessage_MessageIsNull()
	{
		// Act
		var result = new AuthCommandResult.Unauthorized();

		// Assert
		Assert.IsNull(result.Message);
	}

	[TestMethod]
	public void ConfigurationError_StoresMessage()
	{
		// Arrange
		var message = "JWT configuration missing";

		// Act
		var result = new AuthCommandResult.ConfigurationError(message);

		// Assert
		Assert.AreEqual(message, result.Message);
	}

	[TestMethod]
	public void Conflict_StoresMessage()
	{
		// Arrange
		var message = "User already exists";

		// Act
		var result = new AuthCommandResult.Conflict(message);

		// Assert
		Assert.AreEqual(message, result.Message);
	}
}

[TestClass]
public class AuthErrorItemTests
{
	[TestMethod]
	public void AuthErrorItem_WithCodeAndDescription_StoresValues()
	{
		// Arrange
		var code = "EmailRequired";
		var description = "Email is required";

		// Act
		var errorItem = new AuthErrorItem(code, description);

		// Assert
		Assert.AreEqual(code, errorItem.Code);
		Assert.AreEqual(description, errorItem.Description);
	}

	[TestMethod]
	public void AuthErrorItem_TwoInstancesWithSameValues_AreEqual()
	{
		// Arrange
		var item1 = new AuthErrorItem("Code", "Description");
		var item2 = new AuthErrorItem("Code", "Description");

		// Act & Assert
		Assert.AreEqual(item1, item2);
	}

	[TestMethod]
	public void AuthErrorItem_TwoInstancesWithDifferentValues_AreNotEqual()
	{
		// Arrange
		var item1 = new AuthErrorItem("Code1", "Description1");
		var item2 = new AuthErrorItem("Code2", "Description2");

		// Act & Assert
		Assert.AreNotEqual(item1, item2);
	}
}
