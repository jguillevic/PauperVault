namespace PauperVault.Contracts.Decks.Requests;

public record UpdateDeckRequest(
	string Name, 
	string? Description
);
