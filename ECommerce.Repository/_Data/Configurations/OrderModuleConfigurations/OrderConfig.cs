using ECommerce.Core.Entities.OrderModule;
using ECommerce.Core.Entities.OrderModule.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Repository._Data.Configurations.OrderModuleConfigurations
{
	internal class OrderConfig : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder
				.Property(order => order.Status)
				.HasConversion(to => to.ToString(),
				from => Enum.Parse<OrderStatus>(from));

			builder
				.OwnsOne(order => order.ShippingAddress, owned =>
				{
					owned.WithOwner();

					owned
					.Property(sa => sa.Country)
					.HasMaxLength(50);

					owned
					.Property(sa => sa.City)
					.HasMaxLength(50);

					owned
					.Property(sa => sa.Street)
					.HasMaxLength(100);
				});


			builder
				.HasOne(order => order.DeliveryMethod)
				.WithMany()
				.HasForeignKey(order => order.DeliveryMethodId)
				.OnDelete(DeleteBehavior.SetNull);

			builder
				.HasMany(order => order.Items)
				.WithOne()
				.OnDelete(DeleteBehavior.Cascade);

			builder
				.Property(order => order.SubTotal)
				.HasColumnType("Decimal(18,2)");
		}
	}
}
