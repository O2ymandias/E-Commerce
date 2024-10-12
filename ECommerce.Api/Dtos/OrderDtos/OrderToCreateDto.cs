using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Dtos.OrderDtos
{
	public class OrderToCreateDto
	{
		[Required]
		public string BasketId { get; set; }

		[Required]
		public int DeliveryMethodId { get; set; }

		public AddressDto ShippingAddress { get; set; }
	}
}
