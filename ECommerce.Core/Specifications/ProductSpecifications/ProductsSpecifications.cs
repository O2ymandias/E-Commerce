using ECommerce.Core.Entities.ProductModule;

namespace ECommerce.Core.Specifications.ProductSpecifications
{
	public class ProductsSpecifications : BaseSpecifications<Product>
	{
		public ProductsSpecifications(ProductsSpecificationsParams specParams)
			: base(product =>
				(!specParams.BrandId.HasValue || product.BrandId == specParams.BrandId.Value)
				&&
				(!specParams.CategoryId.HasValue || product.CategoryId == specParams.CategoryId.Value)
				&&
				(string.IsNullOrEmpty(specParams.Search) || product.NormalizedName.Contains(specParams.Search)))
		{
			AddIncludes();
			SetSortingType(specParams.Sort);
			ApplyPagination(specParams.PageIndex, specParams.PageSize);
		}

		public ProductsSpecifications(string? productName)
			: base(product => string.IsNullOrEmpty(productName) || product.NormalizedName.Contains(productName))
		{
			AddIncludes();
		}


		public ProductsSpecifications(int id)
			: base(product => product.Id == id)
		{
			AddIncludes();
		}



		private void AddIncludes()
		{
			Includes.Add(product => product.Brand);
			Includes.Add(product => product.Category);
		}
		private void SetSortingType(string? sortType)
		{
			if (!string.IsNullOrEmpty(sortType))
			{
				switch (sortType)
				{
					case "PRICEASC":
						SortAsc = p => p.Price;
						break;

					case "PRICEDESC":
						SortDesc = p => p.Price;
						break;

					default:
						SortAsc = p => p.Name;
						break;
				}
			}
			else
				SortAsc = p => p.Name;
		}

	}
}
