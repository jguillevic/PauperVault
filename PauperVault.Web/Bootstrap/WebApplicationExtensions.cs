using PauperVault.Web.Endpoints;

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

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		// Endpoints
		app.MapAuthEndpoints();
		app.MapRazorPages();
	}
}
