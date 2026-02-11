using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PauperVault.Api.Infrastructure.Auth;

public class AuthDbContext
	: IdentityDbContext<ApplicationUser>
{
	public AuthDbContext(DbContextOptions<AuthDbContext> options)
		: base(options)
	{
	}
}
