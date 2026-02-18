namespace PauperVault.Web.Bootstrap;

public static class WebApplicationExtensions
{
	public static void UsePauperVaultPipeline(this WebApplication app)
	{
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();

		app.Use(async (ctx, next) =>
		{
			// Limite au login si tu veux
			if (ctx.Request.Path.StartsWithSegments("/Account/Login"))
			{
				ctx.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin-allow-popups";
				// Optionnel (souvent inutile ici, mais évite des surprises)
				ctx.Response.Headers["Cross-Origin-Embedder-Policy"] = "unsafe-none";
			}

			await next();
		});

		app.UseRouting();

		// Endpoints
		app.MapRazorPages();
	}
}
