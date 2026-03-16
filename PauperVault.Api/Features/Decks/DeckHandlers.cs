using PauperVault.Api.Contracts.Decks;
using PauperVault.Api.Infrastructure.Auth;
using PauperVault.Api.Infrastructure.Data;
using PauperVault.Core.Domain.Decks;

namespace PauperVault.Api.Features.Decks;

public static class DeckHandlers
{
	public static async Task<IResult> GetPublicDecks(
		int? skip,
		int? take,
		DataDbContext db,
		CancellationToken ct)
	{
		var result = await DeckQueries.GetPublicDecksAsync(db, skip, take, ct);
		return Results.Ok(result);
	}

	public static async Task<IResult> GetMyDecks(
		DataDbContext db,
		HttpContext ctx,
		CancellationToken ct)
	{
		var userId = ctx.User.GetUserIdOrThrow();
		var result = await DeckQueries.GetDeckListForUserAsync(db, userId, ct);
		return Results.Ok(result);
	}

	public static async Task<IResult> GetDeckById(
		Guid id,
		DataDbContext db,
		HttpContext ctx,
		CancellationToken ct)
	{
		var userId = ctx.User.GetUserIdOrThrow();

		var deck = await DeckQueries.GetDeckDetailsForUserAsync(db, id, userId, ct);
		return deck is null ? Results.NotFound() : Results.Ok(deck);
	}

	public static async Task<IResult> CreateDeck(
		CreateDeckRequest req,
		DataDbContext db,
		HttpContext ctx,
		CancellationToken ct)
	{
		var userId = ctx.User.GetUserIdOrThrow();

		if (!DeckRules.IsValidName(req.Name))
			return Results.BadRequest("Deck name is required.");

		var deckId = await DeckCommands.CreateDeckAsync(
			db,
			userId,
			req.Name!,
			req.Description,
			ct);

		return Results.Created($"/decks/{deckId}", new { Id = deckId });
	}

	public static async Task<IResult> UpdateDeck(
		Guid id,
		UpdateDeckRequest req,
		DataDbContext db,
		HttpContext ctx,
		CancellationToken ct)
	{
		var userId = ctx.User.GetUserIdOrThrow();

		if (!DeckRules.IsValidName(req.Name))
			return Results.BadRequest("Deck name is required.");

		var updated = await DeckCommands.UpdateDeckAsync(
			db,
			id,
			userId,
			req.Name!,
			req.Description,
			ct);

		return updated ? Results.NoContent() : Results.NotFound();
	}

	public static async Task<IResult> DeleteDeck(
		Guid id,
		DataDbContext db,
		HttpContext ctx,
		CancellationToken ct)
	{
		var userId = ctx.User.GetUserIdOrThrow();

		var deleted = await DeckCommands.DeleteDeckAsync(db, id, userId, ct);
		return deleted ? Results.NoContent() : Results.NotFound();
	}

	public static async Task<IResult> UpsertDeckCard(
		Guid id,
		AddOrUpdateDeckCardRequest req,
		DataDbContext db,
		HttpContext ctx,
		CancellationToken ct)
	{
		var userId = ctx.User.GetUserIdOrThrow();

		if (!DeckRules.IsValidQuantity(req.Quantity))
			return Results.BadRequest("Quantity is required");

		var updated = await DeckCommands.UpsertDeckCardAsync(
			db,
			id,
			userId,
			req.ScryfallId,
			req.Zone,
			req.Quantity,
			ct);

		return updated ? Results.NoContent() : Results.NotFound();
	}

	public static async Task<IResult> DeleteDeckCard(
		Guid id,
		Guid scryfallId,
		DeckZone zone,
		DataDbContext db,
		HttpContext ctx,
		CancellationToken ct)
	{
		var userId = ctx.User.GetUserIdOrThrow();

		var deleted = await DeckCommands.DeleteDeckCardAsync(
			db,
			id,
			userId,
			scryfallId,
			zone,
			ct);

		return deleted ? Results.NoContent() : Results.NotFound();
	}

	public static async Task<IResult> GetPublicDeckById(
		Guid id,
		DataDbContext db,
		HttpContext ctx,
		CancellationToken ct)
	{
		var currentUserId = ctx.User.GetUserIdOrNull();

		var deck = await DeckQueries.GetPublicDeckDetailsAsync(
			db,
			id,
			currentUserId,
			ct);

		return deck is null ? Results.NotFound() : Results.Ok(deck);
	}
}