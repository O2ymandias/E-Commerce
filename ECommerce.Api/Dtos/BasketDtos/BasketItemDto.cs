using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Dtos.BasketDtos
{
	public class BasketItemDto
	{
		[Required]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string PictureUrl { get; set; }

		[Required]
		public string Brand { get; set; }

		[Required]
		public string Category { get; set; }

		[Required]
		[Range(0.1, double.MaxValue, ErrorMessage = "Price Must Be Greater Than Zero")]
		public decimal Price { get; set; }

		[Required]
		[Range(1, 99, ErrorMessage = "Quantity Must Be At Least 1, And Maximum 99")]
		public int Quantity { get; set; }
	}
}