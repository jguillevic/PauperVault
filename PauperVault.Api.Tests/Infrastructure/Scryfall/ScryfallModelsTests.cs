using PauperVault.Api.Infrastructure.Scryfall;

namespace PauperVault.Api.Tests.Infrastructure.Scryfall;

[TestClass]
public class ScryfallModelsTests
{
	[TestMethod]
	public void ScryfallAutocompleteResponse_WithData_StoresData()
	{
		// Arrange
		var data = new List<string> { "Black Lotus", "Lightning Bolt" };

		// Act
		var response = new ScryfallAutocompleteResponse { Data = data };

		// Assert
		Assert.HasCount(2, response.Data);
		Assert.AreEqual("Black Lotus", response.Data[0]);
		Assert.AreEqual("Lightning Bolt", response.Data[1]);
	}

	[TestMethod]
	public void ScryfallAutocompleteResponse_WithEmptyData_StoresEmptyData()
	{
		// Arrange
		var data = new List<string>();

		// Act
		var response = new ScryfallAutocompleteResponse { Data = data };

		// Assert
		Assert.HasCount(0, response.Data);
	}

	[TestMethod]
	public void ScryfallCard_WithAllProperties_StoresValues()
	{
		// Arrange
		var card = new ScryfallCard
		{
			Id = Guid.NewGuid(),
			Name = "Test Card",
			ManaCost = "{2}{U}",
			TypeLine = "Creature — Test",
			OracleText = "Test ability",
			SetCode = "TST",
			SetName = "Test Set",
			CollectorNumber = "1",
			Power = "2",
			Toughness = "3",
			Rarity = "rare",
			Legalities = new Dictionary<string, string> { { "pauper", "legal" } }
		};

		// Act & Assert
		Assert.AreEqual("Test Card", card.Name);
		Assert.AreEqual("{2}{U}", card.ManaCost);
		Assert.AreEqual("TST", card.SetCode);
		Assert.AreEqual("2", card.Power);
		Assert.AreEqual("3", card.Toughness);
	}

	[TestMethod]
	public void ScryfallCard_WithCardFaces_StoresFaces()
	{
		// Arrange
		var imageUris = new Dictionary<string, string>
		{
			{ "small", "https://example.com/small.jpg" },
			{ "normal", "https://example.com/normal.jpg" }
		};

		var cardFace = new ScryfallCardFace
		{
			ImageUris = imageUris
		};

		var card = new ScryfallCard
		{
			CardFaces = new List<ScryfallCardFace> { cardFace }
		};

		// Act & Assert
		Assert.IsNotNull(card.CardFaces);
		Assert.HasCount(1, card.CardFaces);
		Assert.IsNotNull(card.CardFaces[0].ImageUris);
	}

	[TestMethod]
	public void ScryfallCard_WithImageUris_StoresUrls()
	{
		// Arrange
		var imageUris = new Dictionary<string, string>
		{
			{ "small", "https://example.com/small.jpg" },
			{ "normal", "https://example.com/normal.jpg" },
			{ "large", "https://example.com/large.jpg" }
		};

		var card = new ScryfallCard
		{
			ImageUris = imageUris
		};

		// Act & Assert
		Assert.IsNotNull(card.ImageUris);
		Assert.AreEqual("https://example.com/small.jpg", card.ImageUris["small"]);
		Assert.AreEqual("https://example.com/normal.jpg", card.ImageUris["normal"]);
		Assert.AreEqual("https://example.com/large.jpg", card.ImageUris["large"]);
	}
}
