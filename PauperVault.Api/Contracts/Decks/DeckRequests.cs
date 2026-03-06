using PauperVault.Core.Domain.Decks;

namespace PauperVault.Api.Contracts.Decks;

public record AddOrUpdateDeckCardRequest(Guid ScryfallId, DeckZone Zone, int Quantity);
public record CreateDeckRequest(string Name, string? Description);
public record UpdateDeckRequest(string Name, string? Description);
