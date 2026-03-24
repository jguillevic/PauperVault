using PauperVault.Api.Features.Cards;

namespace PauperVault.Api.Tests.Features.Cards;

[TestClass]
public class CardQueriesTests
{
	[TestMethod]
	public void EmptyAutocomplete_ReturnsEmptyList()
	{
		// Act
		var result = CardQueries.EmptyAutocomplete();

		// Assert
		Assert.IsNotNull(result);
		Assert.HasCount(0, result.Suggestions);
	}

	[TestMethod]
	public void EmptyAutocomplete_ReturnsDtoWithEmptyArray()
	{
		// Act
		var result = CardQueries.EmptyAutocomplete();

		// Assert
		Assert.IsNotNull(result);
		Assert.IsInstanceOfType(result, typeof(Contracts.Cards.Dto.CardAutocompleteDto));
		Assert.HasCount(0, result.Suggestions);
	}
}
