using Microsoft.EntityFrameworkCore;
using PauperVault.Api.Contracts.Decks;
using PauperVault.Api.Infrastructure.Auth;
using PauperVault.Api.Infrastructure.Data;
using PauperVault.Core.Domain.Decks;

namespace PauperVault.Api.Endpoints;

public static class DeckEndpoints
{
	public static void MapDeckEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapGet("/public/decks", async (
		int? skip,
		int? take,
		DataDbContext db,
		CancellationToken ct) =>
		{
			var safeSkip = Math.Max(0, skip ?? 0);
			var safeTake = Math.Clamp(take ?? 10, 1, 100);

			var decks = await db.Decks
				.AsNoTracking()
				.OrderByDescending(d => d.UpdatedAt)
				.Skip(safeSkip)
				.Take(safeTake)
				.Select(d => new
				{
					d.Id,
					d.Name,
					d.UpdatedAt,
					FirstCardId = d.Cards
						.OrderBy(c => c.Zone)
						.ThenBy(c => c.ScryfallId)
						.Select(c => (Guid?)c.ScryfallId)
						.FirstOrDefault()
				})
				.ToListAsync(ct);

			var firstCardIds = decks
				.Where(x => x.FirstCardId.HasValue)
				.Select(x => x.FirstCardId!.Value)
				.Distinct()
				.ToList();

			var imageByCardId = await db.Cards
				.AsNoTracking()
				.Where(c => firstCardIds.Contains(c.ScryfallId))
				.ToDictionaryAsync(c => c.ScryfallId, c => c.ImageSmallUrl, ct);

			var result = decks
				.Select(d =>
				{
					string? coverImageUrl = null;

					if (d.FirstCardId.HasValue &&
						imageByCardId.TryGetValue(d.FirstCardId.Value, out var imageUrl) &&
						!string.IsNullOrWhiteSpace(imageUrl))
					{
						coverImageUrl = imageUrl;
					}

					return new PublicDeckListItemDto(
						d.Id,
						d.Name,
						d.UpdatedAt,
						coverImageUrl);
				})
				.ToList();

			return Results.Ok(result);
		});

		var group = app.MapGroup("/decks")
			.RequireAuthorization();

		// 1) Liste des decks du user
		group.MapGet("/", async (DataDbContext db, HttpContext ctx) =>
		{
			var userId = ctx.User.GetUserIdOrThrow();

			var decks = await db.Decks
				.Where(d => d.OwnerUserId == userId)
				.OrderByDescending(d => d.UpdatedAt)
				.Select(d => new DeckListItemDto(d.Id, d.Name, d.UpdatedAt))
				.ToListAsync();

			return Results.Ok(decks);
		});

		// 2) Détails d'un deck (avec cartes)
		group.MapGet("/{id:guid}", async (Guid id, DataDbContext db, HttpContext ctx) =>
		{
			var userId = ctx.User.GetUserIdOrThrow();

			var deck = await db.Decks
				.AsNoTracking()
				.Include(d => d.Cards)
				.FirstOrDefaultAsync(d => d.Id == id && d.OwnerUserId == userId);

			if (deck is null) return Results.NotFound();

			var scryfallIds = deck.Cards
				.Select(c => c.ScryfallId)
				.Distinct()
				.ToList();

			var cardNamesById = await db.Cards
				.AsNoTracking()
				.Where(c => scryfallIds.Contains(c.ScryfallId))
				.ToDictionaryAsync(c => c.ScryfallId, c => c.Name);

			var dto = new DeckDetailsDto(
				deck.Id,
				deck.Name,
				deck.Description,
				deck.CreatedAt,
				deck.UpdatedAt,
				deck.Cards
					.OrderBy(c => c.Zone)
					.ThenBy(c => cardNamesById.ContainsKey(c.ScryfallId) ? cardNamesById[c.ScryfallId] : c.ScryfallId.ToString())
					.Select(c => new DeckCardDto(
						c.ScryfallId,
						cardNamesById.TryGetValue(c.ScryfallId, out var name) ? name : c.ScryfallId.ToString(),
						c.Zone,
						c.Quantity))
					.ToList()
			);

			return Results.Ok(dto);
		});

		// 3) Créer un deck
		group.MapPost("/", async (CreateDeckRequest req, DataDbContext db, HttpContext ctx) =>
		{
			var userId = ctx.User.GetUserIdOrThrow();

			var name = (req.Name ?? "").Trim();
			if (name.Length < 2) return Results.BadRequest("Deck name is required.");

			var deck = new Deck
			{
				OwnerUserId = userId,
				Name = name,
				Description = req.Description?.Trim(),
				CreatedAt = DateTimeOffset.UtcNow,
				UpdatedAt = DateTimeOffset.UtcNow
			};

			db.Decks.Add(deck);
			await db.SaveChangesAsync();

			return Results.Created($"/decks/{deck.Id}", new { deck.Id });
		});

		// 4) Modifier le deck (meta)
		group.MapPut("/{id:guid}", async (Guid id, UpdateDeckRequest req, DataDbContext db, HttpContext ctx) =>
		{
			var userId = ctx.User.GetUserIdOrThrow();

			var deck = await db.Decks.FirstOrDefaultAsync(d => d.Id == id && d.OwnerUserId == userId);
			if (deck is null) return Results.NotFound();

			var name = (req.Name ?? "").Trim();
			if (name.Length < 2) return Results.BadRequest("Deck name is required.");

			deck.Name = name;
			deck.Description = req.Description?.Trim();
			deck.UpdatedAt = DateTimeOffset.UtcNow;

			await db.SaveChangesAsync();
			return Results.NoContent();
		});

		// 5) Supprimer un deck
		group.MapDelete("/{id:guid}", async (Guid id, DataDbContext db, HttpContext ctx) =>
		{
			var userId = ctx.User.GetUserIdOrThrow();

			var deck = await db.Decks.FirstOrDefaultAsync(d => d.Id == id && d.OwnerUserId == userId);
			if (deck is null) return Results.NotFound();

			db.Decks.Remove(deck);
			await db.SaveChangesAsync();
			return Results.NoContent();
		});

		// 6) Ajouter ou mettre à jour une carte dans un deck (upsert)
		group.MapPost("/{id:guid}/cards", async (Guid id, AddOrUpdateDeckCardRequest req, DataDbContext db, HttpContext ctx) =>
		{
			var userId = ctx.User.GetUserIdOrThrow();

			if (req.Quantity <= 0) return Results.BadRequest("Quantity must be >= 1.");

			var deck = await db.Decks.Include(d => d.Cards)
				.FirstOrDefaultAsync(d => d.Id == id && d.OwnerUserId == userId);

			if (deck is null) return Results.NotFound();

			var existing = deck.Cards.FirstOrDefault(c =>
				c.ScryfallId == req.ScryfallId && c.Zone == req.Zone);

			if (existing is null)
			{
				deck.Cards.Add(new DeckCard
				{
					DeckId = deck.Id,
					ScryfallId = req.ScryfallId,
					Zone = req.Zone,
					Quantity = req.Quantity
				});
			}
			else
			{
				existing.Quantity = req.Quantity;
			}

			deck.UpdatedAt = DateTimeOffset.UtcNow;

			await db.SaveChangesAsync();
			return Results.NoContent();
		});

		// 7) Supprimer une carte d’un deck
		group.MapDelete("/{id:guid}/cards/{scryfallId:guid}/{zone}", async (
			Guid id,
			Guid scryfallId,
			DeckZone zone,
			DataDbContext db,
			HttpContext ctx) =>
		{
			var userId = ctx.User.GetUserIdOrThrow();

			var deck = await db.Decks.Include(d => d.Cards)
				.FirstOrDefaultAsync(d => d.Id == id && d.OwnerUserId == userId);

			if (deck is null) return Results.NotFound();

			var card = deck.Cards.FirstOrDefault(c => c.ScryfallId == scryfallId && c.Zone == zone);
			if (card is null) return Results.NotFound();

			deck.Cards.Remove(card);
			deck.UpdatedAt = DateTimeOffset.UtcNow;

			await db.SaveChangesAsync();
			return Results.NoContent();
		});
	}
}