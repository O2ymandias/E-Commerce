using ECommerce.Core.Entities.ProductModule;
using ECommerce.Core.Specifications.ProductSpecifications;

namespace ECommerce.Core.Interfaces.Services.Contract
{
	public interface IProductService
	{
		Task<IReadOnlyList<Product>> GetAllProductsAsync(ProductsSpecificationsParams specParams);
		Task<int> GetCountOfSpecificProductsAsync(ProductsSpecificationsParams specParams);
		Task<Product?> GetProductByIdAsync(int id);
		Task<IReadOnlyList<Brand>> GetAllBrandsAsync();
		Task<IReadOnlyList<Category>> GetAllCategoriesAsync();
	}
}
