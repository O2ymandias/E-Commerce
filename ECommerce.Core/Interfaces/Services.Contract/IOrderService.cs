using ECommerce.Core.Entities.OrderModule;
using ECommerce.Core.Entities.OrderModule.Utilities;

namespace ECommerce.Core.Interfaces.Services.Contract
{
	public interface IOrderService
	{
		Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress);
		Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
		Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string userEmail);
		Task<Order?> GetOrderByIdForUserAsync(string userEmail, int orderId);
		Task<Order?> UpdateOrderStatus(string paymentIntentId, bool isPaid);
	}
}
