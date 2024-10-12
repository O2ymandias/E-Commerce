using ECommerce.Core.Entities.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Repository._Data.Configurations.OrderModuleConfigurations
{
	internal class DeliveryMethodConfig : IEntityTypeConfiguration<DeliveryMethod>
	{
		public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
		{
			builder
				.Property(dm => dm.Cost)
				.HasColumnType("Decimal(18,2)");
		}
	}
}
