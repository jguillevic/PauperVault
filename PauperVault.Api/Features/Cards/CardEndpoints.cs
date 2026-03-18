namespace PauperVault.Api.Features.Cards;

public static class CardEndpoints
{
	public static void MapCardEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/cards")
			.RequireAuthorization();

		group.MapGet("/autocomplete", CardHandlers.AutocompleteAsync);
		group.MapGet("/resolve", CardHandlers.ResolveAsync);
		group.MapGet("/{scryfallId:guid}", CardHandlers.GetByIdAsync);
	}
}