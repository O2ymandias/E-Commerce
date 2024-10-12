using ECommerce.Core.Entities.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Repository._Data.Configurations.ProductModuleConfigurations
{
    internal class BrandConfig : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder
                .Property(b => b.Name)
                .IsRequired();
        }
    }
}
