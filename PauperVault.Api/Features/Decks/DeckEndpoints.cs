namespace PauperVault.Api.Features.Decks;

public static class DeckEndpoints
{
	public static void MapDeckEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapGet("/public/decks", DeckHandlers.GetPublicDecks);
		app.MapGet("/public/decks/{id:guid}", DeckHandlers.GetPublicDeckById);

		var group = app.MapGroup("/decks")
			.RequireAuthorization();

		group.MapGet("/", DeckHandlers.GetMyDecks);
		group.MapGet("/{id:guid}", DeckHandlers.GetDeckById);
		group.MapPost("/", DeckHandlers.CreateDeck);
		group.MapPut("/{id:guid}", DeckHandlers.UpdateDeck);
		group.MapDelete("/{id:guid}", DeckHandlers.DeleteDeck);
		group.MapPost("/{id:guid}/cards", DeckHandlers.UpsertDeckCard);
		group.MapDelete("/{id:guid}/cards/{scryfallId:guid}/{zone}", DeckHandlers.DeleteDeckCard);
	}
}