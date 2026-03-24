using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PauperVault.Api.Infrastructure.Auth;

public class AuthDbContext
	: IdentityDbContext<ApplicationUser>
{
	public AuthDbContext(DbContextOptions<AuthDbContext> options)
		: base(options)
	{
	}
}
