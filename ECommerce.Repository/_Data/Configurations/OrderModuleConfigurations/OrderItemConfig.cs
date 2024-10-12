using ECommerce.Core.Entities.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Repository._Data.Configurations.OrderModuleConfigurations
{
	internal class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
	{
		public void Configure(EntityTypeBuilder<OrderItem> builder)
		{
			builder.OwnsOne(orderItem => orderItem.Product, owned => owned.WithOwner());

			builder
				.Property(orderItem => orderItem.Price)
				.HasColumnType("Decimal(18,2)");
		}
	}
}
