using PauperVault.Web.Features.Auth;
using PauperVault.Web.Options;

namespace PauperVault.Web.Bootstrap;

public static class ServiceCollectionExtensions
{
	public static void AddPauperVaultServices(this WebApplicationBuilder builder)
	{
		builder.Services.AddRazorPages();

		builder.Services.Configure<FirebaseWebOptions>(
			builder.Configuration.GetSection(FirebaseWebOptions.SectionName)
		);

		builder.Services
			.AddAuthentication("pv-cookie")
			.AddCookie("pv-cookie", opt =>
			{
				opt.LoginPath = "/Auth/Login";
				opt.AccessDeniedPath = "/Auth/Denied";
				opt.Cookie.Name = "pv_auth";
				opt.Cookie.HttpOnly = true;
				opt.Cookie.SameSite = SameSiteMode.Lax;

				opt.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
					? CookieSecurePolicy.SameAsRequest
					: CookieSecurePolicy.Always;
			});

		builder.Services.AddAuthorization();

		builder.Services.AddScoped<IAuthService, AuthService>();
	}
}
