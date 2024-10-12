using ECommerce.Core.Entities.ProductModule;

namespace ECommerce.Core.Specifications.ProductSpecifications
{
	public class CountOfSpecificProducts : BaseSpecifications<Product>
	{
		public CountOfSpecificProducts(ProductsSpecificationsParams specParams)
			: base(product =>
			(!specParams.BrandId.HasValue || product.BrandId == specParams.BrandId.Value)
			&&
			(!specParams.CategoryId.HasValue || product.CategoryId == specParams.CategoryId)
			&&
			(string.IsNullOrEmpty(specParams.Search) || product.NormalizedName.Contains(specParams.Search)))
		{
		}
	}
}
