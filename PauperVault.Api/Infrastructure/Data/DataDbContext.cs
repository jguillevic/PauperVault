using Microsoft.EntityFrameworkCore;
using PauperVault.Core.Domain.Cards;
using PauperVault.Core.Domain.Decks;

namespace PauperVault.Api.Infrastructure.Data;

public class DataDbContext : DbContext
{
	public DataDbContext(DbContextOptions<DataDbContext> options) : base(options) { }

	public DbSet<Deck> Decks => Set<Deck>();
	public DbSet<DeckCard> DeckCards => Set<DeckCard>();
	public DbSet<CardCache> Cards => Set<CardCache>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Deck
		modelBuilder.Entity<Deck>()
			.HasMany(d => d.Cards)
			.WithOne(dc => dc.Deck)
			.HasForeignKey(dc => dc.DeckId);

		// DeckCard: clé composite (DeckId + ScryfallId + Zone)
		modelBuilder.Entity<DeckCard>()
			.HasKey(dc => new { dc.DeckId, dc.ScryfallId, dc.Zone });

		// CardCache: clé = ScryfallId
		modelBuilder.Entity<CardCache>()
			.HasKey(c => c.ScryfallId);

		// (Optionnel) index utile
		modelBuilder.Entity<CardCache>()
			.HasIndex(c => c.Name);
	}
}