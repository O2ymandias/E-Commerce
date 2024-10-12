using ECommerce.Core.Entities.ProductModule;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Services.Contract;
using ECommerce.Core.Specifications.ProductSpecifications;

namespace ECommerce.Service
{
	public class ProductService : IProductService
	{
		private readonly IUnitOfWork _unitOfWork;

		public ProductService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IReadOnlyList<Product>> GetAllProductsAsync(ProductsSpecificationsParams specParams)
		{
			var specs = new ProductsSpecifications(specParams);
			return await _unitOfWork.Repository<Product>().GetAllWithSpecsAsync(specs);
		}

		public async Task<int> GetCountOfSpecificProductsAsync(ProductsSpecificationsParams specParams)
		{
			var specs = new CountOfSpecificProducts(specParams);
			return await _unitOfWork.Repository<Product>().GetCountWithSpecsAsync(specs);
		}

		public async Task<Product?> GetProductByIdAsync(int id)
		{
			var specs = new ProductsSpecifications(id);
			return await _unitOfWork.Repository<Product>().GetWithSpecsAsync(specs);
		}

		public async Task<IReadOnlyList<Brand>> GetAllBrandsAsync()
			=> await _unitOfWork.Repository<Brand>().GetAllAsync();

		public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync()
			=> await _unitOfWork.Repository<Category>().GetAllAsync();
	}
}
