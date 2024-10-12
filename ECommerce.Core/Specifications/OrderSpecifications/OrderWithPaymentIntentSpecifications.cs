using ECommerce.Core.Entities.OrderModule;

namespace ECommerce.Core.Specifications.OrderSpecifications
{
	public class OrderWithPaymentIntentSpecifications : BaseSpecifications<Order>
	{
		public OrderWithPaymentIntentSpecifications(string paymentIntentId)
			: base(order => order.PaymentIntentId == paymentIntentId)
		{
		}
	}
}
