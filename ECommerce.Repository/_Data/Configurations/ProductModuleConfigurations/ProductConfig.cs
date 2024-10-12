using ECommerce.Core.Entities.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Repository._Data.Configurations.ProductModuleConfigurations
{
    internal class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder
                .Property(p => p.Name)
                .IsRequired();

            builder
                .Property(p => p.Description)
                .IsRequired();

            builder
                .Property(p => p.PictureUrl)
                .IsRequired();

            builder
                .Property(p => p.Price)
                .HasColumnType("Decimal(18,2)");

            builder
                .HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.BrandId);

            builder
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId);
        }
    }
}
