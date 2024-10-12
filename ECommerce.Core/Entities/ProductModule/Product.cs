namespace ECommerce.Core.Entities.ProductModule
{
	public class Product : BaseEntity
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string PictureUrl { get; set; }
		public decimal Price { get; set; }

		public virtual Brand Brand { get; set; }
		public int BrandId { get; set; }

		public virtual Category Category { get; set; }
		public int CategoryId { get; set; }

		public string NormalizedName { get; set; }

	}
}
