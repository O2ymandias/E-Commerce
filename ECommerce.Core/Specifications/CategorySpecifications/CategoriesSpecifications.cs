using ECommerce.Core.Entities.ProductModule;

namespace ECommerce.Core.Specifications.CategorySpecifications
{
	public class CategoriesSpecifications : BaseSpecifications<Category>
	{
		public CategoriesSpecifications(string? categoryName)
			: base(category => string.IsNullOrEmpty(categoryName) || category.Name.Contains(categoryName))
		{
		}
	}
}
