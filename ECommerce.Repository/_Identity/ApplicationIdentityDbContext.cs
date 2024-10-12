using ECommerce.Core.Entities.IdentityModule;
using ECommerce.Repository._Identity.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repository._Identity
{
	public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<Address>().ToTable("Addresses");
			builder.Entity<RefreshToken>().ToTable("RefreshTokens");
			builder.ApplyConfiguration(new ApplicationUserConfig());
			base.OnModelCreating(builder);
		}
	}
}
