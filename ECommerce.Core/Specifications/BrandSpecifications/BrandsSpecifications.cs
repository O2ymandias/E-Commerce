using ECommerce.Core.Entities.ProductModule;

namespace ECommerce.Core.Specifications.BrandSpecifications
{
	public class BrandsSpecifications : BaseSpecifications<Brand>
	{
		public BrandsSpecifications(string? brandName)
			: base(brand => string.IsNullOrEmpty(brandName) || brand.Name.Contains(brandName))
		{
		}
	}
}
