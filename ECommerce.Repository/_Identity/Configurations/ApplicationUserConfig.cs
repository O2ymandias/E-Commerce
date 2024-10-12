using ECommerce.Core.Entities.IdentityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Repository._Identity.Configurations
{
	internal class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			builder
				.HasMany(appUser => appUser.RefreshTokens)
				.WithOne()
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
