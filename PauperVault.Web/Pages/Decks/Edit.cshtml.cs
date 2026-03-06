using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PauperVault.Core.Domain.Decks;
using PauperVault.Web.Contracts.Decks;
using PauperVault.Web.Infrastructure.Http.PauperVault;
using System.ComponentModel.DataAnnotations;

namespace PauperVault.Web.Pages.Decks;

public class EditModel(IPauperVaultApiClient api) : PageModel
{
	[FromRoute]
	public Guid Id { get; set; }

	public DeckDetailsDto Deck { get; private set; } = default!;

	[BindProperty]
	public UpdateDeckInput DeckForm { get; set; } = new();

	[BindProperty]
	public AddCardInput CardForm { get; set; } = new();

	public string? ErrorMessage { get; private set; }

	public class UpdateDeckInput
	{
		[Required(ErrorMessage = "Le nom du deck est obligatoire.")]
		public string Name { get; set; } = "";

		public string? Description { get; set; }
	}

	public class AddCardInput
	{
		[Required(ErrorMessage = "Le nom de la carte est obligatoire.")]
		public string CardName { get; set; } = "";

		[Required(ErrorMessage = "La zone est obligatoire.")]
		public DeckZone Zone { get; set; } = DeckZone.Main;

		[Range(1, 99, ErrorMessage = "La quantité doit être d’au moins 1.")]
		public int Quantity { get; set; } = 1;
	}

	public async Task<IActionResult> OnGetAsync(CancellationToken ct)
	{
		await LoadDeckAsync(ct, hydrateDeckForm: true);
		return Page();
	}

	public async Task<IActionResult> OnPostSaveDeckAsync(CancellationToken ct)
	{
		ModelState.Clear();

		if (!TryValidateModel(DeckForm, nameof(DeckForm)))
		{
			await LoadDeckAsync(ct);
			return Page();
		}

		try
		{
			await api.UpdateDeckAsync(
				Id,
				new UpdateDeckRequest(DeckForm.Name, DeckForm.Description),
				ct);

			return RedirectToPage(new { id = Id, saved = true });
		}
		catch (Exception)
		{
			ErrorMessage = "Impossible d’enregistrer le deck.";
			await LoadDeckAsync(ct);
			return Page();
		}
	}

	public async Task<IActionResult> OnPostAddCardAsync(CancellationToken ct)
	{
		ModelState.Clear();

		if (!TryValidateModel(CardForm, nameof(CardForm)))
		{
			await LoadDeckAsync(ct);
			return Page();
		}

		try
		{
			var card = await api.ResolveCardAsync(CardForm.CardName, ct);

			await api.AddOrUpdateDeckCardAsync(
				Id,
				new AddOrUpdateDeckCardRequest(
					card.ScryfallId,
					CardForm.Zone,
					CardForm.Quantity),
				ct);

			return RedirectToPage(new { id = Id, added = true });
		}
		catch (Exception)
		{
			ErrorMessage = "Impossible d’ajouter cette carte au deck.";
			await LoadDeckAsync(ct);
			return Page();
		}
	}

	public async Task<JsonResult> OnGetCardAutocompleteAsync(string q, CancellationToken ct)
	{
		var result = await api.AutocompleteCardsAsync(q, ct);
		return new JsonResult(result);
	}

	private async Task LoadDeckAsync(CancellationToken ct, bool hydrateDeckForm = false)
	{
		Deck = await api.GetDeckAsync(Id, ct);

		if (hydrateDeckForm)
		{
			DeckForm = new UpdateDeckInput
			{
				Name = Deck.Name,
				Description = Deck.Description
			};
		}
	}
}