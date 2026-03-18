using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PauperVault.Contracts.Decks.Requests;
using PauperVault.Web.Infrastructure.Http.PauperVault;
using System.ComponentModel.DataAnnotations;

namespace PauperVault.Web.Pages.Decks;

public class CreateModel(
	IPauperVaultApiClient api
) : PageModel
{
	[BindProperty]
	public CreateDeckInput Input { get; set; } = new();

	public string? ErrorMessage { get; private set; }

	public class CreateDeckInput
	{
		[Required]
		[MinLength(2)]
		[MaxLength(80)]
		public string Name { get; set; } = "";

		[MaxLength(500)]
		public string? Description { get; set; }
	}

	public void OnGet()
	{
	}

	public async Task<IActionResult> OnPostAsync(CancellationToken ct)
	{
		if (!ModelState.IsValid)
			return Page();

		try
		{
			var id = await api.CreateDeckAsync(
				new CreateDeckRequest(Input.Name.Trim(), Input.Description?.Trim()),
				ct);

			// Redirige vers l'édition du deck
			return RedirectToPage("/Decks/Edit", new { id });
		}
		catch (Exception)
		{
			// Tu peux affiner (HttpRequestException, 401, etc.)
			ErrorMessage = "Impossible de créer le deck. Réessaie.";
			return Page();
		}
	}
}