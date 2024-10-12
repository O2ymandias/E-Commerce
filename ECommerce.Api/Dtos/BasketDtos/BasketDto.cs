using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Dtos.BasketDtos
{
	public class BasketDto
	{
		[Required]
		public string Id { get; set; }
		public List<BasketItemDto> Items { get; set; }
		public string? PaymentIntentId { get; set; }
		public string? ClientSecret { get; set; }
		public int? DeliveryMethodId { get; set; }
		public decimal? ShippingPrice { get; set; }
	}
}
