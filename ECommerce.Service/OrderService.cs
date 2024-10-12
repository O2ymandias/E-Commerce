using ECommerce.Core.Entities.OrderModule;
using ECommerce.Core.Entities.OrderModule.Utilities;
using ECommerce.Core.Entities.ProductModule;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Repositories.Contract;
using ECommerce.Core.Interfaces.Services.Contract;
using ECommerce.Core.Specifications.OrderSpecifications;

namespace ECommerce.Service
{
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository _basketRepo;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IPaymentService _paymentService;

		public OrderService(IBasketRepository basketRepo,
			IUnitOfWork unitOfWork,
			IPaymentService paymentService)
		{
			_basketRepo = basketRepo;
			_unitOfWork = unitOfWork;
			_paymentService = paymentService;
		}

		public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
		{
			var productRepo = _unitOfWork.Repository<Product>();
			var deliveryMethodRepo = _unitOfWork.Repository<DeliveryMethod>();
			var orderRepo = _unitOfWork.Repository<Order>();

			var basket = await _basketRepo.GetBasketAsync(basketId);
			if (basket is null || basket.Items.Count == 0)
				return null;


			var orderItems = new List<OrderItem>();
			foreach (var basketItem in basket.Items)
			{
				var product = await productRepo.GetAsync(basketItem.Id);
				if (product is null)
					continue;

				var orderItem = new OrderItem()
				{
					Product = new ProductAsItem()
					{
						Id = product.Id,
						Name = product.Name,
						PictureUrl = product.PictureUrl,
					},
					Price = product.Price,
					Quantity = basketItem.Quantity
				};
				orderItems.Add(orderItem);
			}
			var subTotal = orderItems.Sum(orderItem => orderItem.Price * orderItem.Quantity);


			var deliveryMethod = await deliveryMethodRepo.GetAsync(deliveryMethodId) ??
				throw new Exception($"Invalid Delivery Method");


			if (basket.PaymentIntentId is not null)
			{
				var specs = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);
				var existingOrder = await orderRepo.GetWithSpecsAsync(specs);
				if (existingOrder is not null)
				{
					orderRepo.Delete(existingOrder);
					await _paymentService.CreateOrUpdatePaymentIntent(basketId);
				}
			}


			var order = new Order()
			{
				BuyerEmail = buyerEmail,
				Items = orderItems,
				ShippingAddress = shippingAddress,
				DeliveryMethod = deliveryMethod,
				SubTotal = subTotal,
				PaymentIntentId = basket?.PaymentIntentId ?? throw new Exception("There Is No PaymentIntent Created For This Order")
			};


			orderRepo.Add(order);
			var numberOfRowsAffected = await _unitOfWork.CompleteAsync();
			return numberOfRowsAffected > 0
				? order
				: null;
		}
		public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string userEmail)
		{
			var specs = new OrderSpecifications(userEmail);
			return await _unitOfWork.Repository<Order>().GetAllWithSpecsAsync(specs);
		}
		public async Task<Order?> GetOrderByIdForUserAsync(string userEmail, int orderId)
		{
			var specs = new OrderSpecifications(userEmail, orderId);
			return await _unitOfWork.Repository<Order>().GetWithSpecsAsync(specs);
		}
		public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
			=> await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
		public async Task<Order?> UpdateOrderStatus(string paymentIntentId, bool isPaid)
		{
			var orderRepo = _unitOfWork.Repository<Order>();
			var specs = new OrderWithPaymentIntentSpecifications(paymentIntentId);
			var order = await orderRepo.GetWithSpecsAsync(specs);

			if (order is null)
				return null;

			if (isPaid)
				order.Status = OrderStatus.PaymentReceived;
			else order.Status = OrderStatus.PaymentFailed;

			orderRepo.Update(order);
			await _unitOfWork.CompleteAsync();
			return order;
		}

	}
}
