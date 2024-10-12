using ECommerce.Core.Entities.OrderModule;
using ECommerce.Core.Entities.ProductModule;
using System.Text.Json;

namespace ECommerce.Repository._Data
{
	public static class StoreDbContextSeed
	{
		public static async Task SeedDataAsync(StoreDbContext dbContext)
		{

			if (!dbContext.Brands.Any())
			{
				var path = "../ECommerce.Repository/_Data/DataSeed/brands.json";
				if (File.Exists(path))
				{
					var brandsData = File.ReadAllText(path);
					var brands = JsonSerializer.Deserialize<List<Brand>>(brandsData);

					if (brands?.Count > 0)
					{
						foreach (var brand in brands)
							dbContext.Brands.Add(brand);

						await dbContext.SaveChangesAsync();
					}
				}
			}

			if (!dbContext.Categories.Any())
			{
				var path = "../ECommerce.Repository/_Data/DataSeed/categories.json";

				if (File.Exists(path))
				{
					var categoriesData = File.ReadAllText(path);
					var categories = JsonSerializer.Deserialize<List<Category>>(categoriesData);
					if (categories?.Count > 0)
					{
						foreach (var category in categories)
							dbContext.Categories.Add(category);

						await dbContext.SaveChangesAsync();
					}
				}
			}

			if (!dbContext.Products.Any())
			{
				var path = "../ECommerce.Repository/_Data/DataSeed/products.json";
				if (File.Exists(path))
				{
					var productsData = File.ReadAllText(path);
					var products = JsonSerializer.Deserialize<List<Product>>(productsData);
					if (products?.Count > 0)
					{
						foreach (var product in products)
							dbContext.Products.Add(product);

						await dbContext.SaveChangesAsync();
					}
				}
			}

			if (!dbContext.DeliveryMethods.Any())
			{
				var path = "../ECommerce.Repository/_Data/DataSeed/delivery.json";
				if (File.Exists(path))
				{
					var deliveryMethodsData = File.ReadAllText(path);
					var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);
					if (deliveryMethods?.Count > 0)
					{
						foreach (var deliveryMethod in deliveryMethods)
							dbContext.DeliveryMethods.Add(deliveryMethod);

						await dbContext.SaveChangesAsync();
					}
				}
			}
		}

	}
}
