namespace PauperVault.Contracts.Decks.Requests;

public record CreateDeckRequest(
	string Name, 
	string? Description
);
