using ECommerce.Core.Entities.OrderModule.Utilities;

namespace ECommerce.Core.Entities.OrderModule
{
	public class Order : BaseEntity
	{
		public string BuyerEmail { get; set; }
		public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
		public OrderStatus Status { get; set; } = OrderStatus.Pending;
		public Address ShippingAddress { get; set; }
		public virtual DeliveryMethod DeliveryMethod { get; set; }
		public int? DeliveryMethodId { get; set; }
		public virtual ICollection<OrderItem> Items { get; set; }
		public decimal SubTotal { get; set; }
		public string PaymentIntentId { get; set; }

		public decimal GetTotal()
			=> SubTotal + DeliveryMethod.Cost;
	}
}
