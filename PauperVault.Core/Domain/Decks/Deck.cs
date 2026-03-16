using PauperVault.Core.Domain.Common;

namespace PauperVault.Core.Domain.Decks;

public class Deck
{
	public Guid Id { get; set; }
	public string OwnerUserId { get; set; } = default!;
	public string Name { get; set; } = default!;
	public string? Description { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset UpdatedAt { get; set; }

	public List<DeckCard> Cards { get; set; } = new();

	public void Rename(string name, string? description)
	{
		if (!DeckRules.IsValidName(name))
			throw new DomainException("Deck name is invalid.");

		Name = name.Trim();
		Description = description?.Trim();
		Touch();
	}

	public void AddOrUpdateCard(Guid scryfallId, DeckZone zone, int quantity)
	{
		if (!DeckRules.IsValidQuantity(quantity))
			throw new DomainException("Quantity must be >= 1.");

		var existing = Cards.FirstOrDefault(c =>
			c.ScryfallId == scryfallId &&
			c.Zone == zone);

		if (existing is null)
		{
			Cards.Add(new DeckCard
			{
				DeckId = Id,
				ScryfallId = scryfallId,
				Zone = zone,
				Quantity = quantity
			});
		}
		else
		{
			existing.Quantity = quantity;
		}

		Touch();
	}

	public bool RemoveCard(Guid scryfallId, DeckZone zone)
	{
		var existing = Cards.FirstOrDefault(c =>
			c.ScryfallId == scryfallId &&
			c.Zone == zone);

		if (existing is null)
			return false;

		Cards.Remove(existing);
		Touch();
		return true;
	}

	public static Deck Create(string ownerUserId, string name, string? description, DateTimeOffset now)
	{
		if (!DeckRules.IsValidName(name))
			throw new DomainException("Deck name is invalid.");

		return new Deck
		{
			OwnerUserId = ownerUserId,
			Name = name.Trim(),
			Description = description?.Trim(),
			CreatedAt = now,
			UpdatedAt = now
		};
	}

	private void Touch()
	{
		UpdatedAt = DateTimeOffset.UtcNow;
	}
}