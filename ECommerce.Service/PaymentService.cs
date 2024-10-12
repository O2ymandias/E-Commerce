using ECommerce.Core.Entities.BasketModule;
using ECommerce.Core.Entities.OrderModule;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Repositories.Contract;
using ECommerce.Core.Interfaces.Services.Contract;
using Microsoft.Extensions.Configuration;
using Stripe;
using Product = ECommerce.Core.Entities.ProductModule.Product;

namespace ECommerce.Service
{
	public class PaymentService : IPaymentService
	{
		private readonly IBasketRepository _basketRepo;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IConfiguration _configuration;

		public PaymentService(IBasketRepository basketRepo,
			IUnitOfWork unitOfWork,
			IConfiguration configuration)
		{
			_basketRepo = basketRepo;
			_unitOfWork = unitOfWork;
			_configuration = configuration;
		}

		public async Task<Basket?> CreateOrUpdatePaymentIntent(string basketId)
		{
			var basket = await _basketRepo.GetBasketAsync(basketId);
			if (basket is null || basket.Items.Count == 0)
				return null;


			var productRepo = _unitOfWork.Repository<Product>();
			var basketItemsToRemove = new List<BasketItem>();
			foreach (var basketItem in basket.Items)
			{
				var product = await productRepo.GetAsync(basketItem.Id);

				if (product is null)
				{
					basketItemsToRemove.Add(basketItem);
					continue;
				}

				if (basketItem.Price != product.Price)
					basketItem.Price = product.Price;
			}
			if (basketItemsToRemove.Count > 0)
				foreach (var item in basketItemsToRemove)
					basket.Items.Remove(item);
			var subTotal = basket.Items.Sum(basketItem => basketItem.Price * basketItem.Quantity);


			var shippingCost = 0.0M;
			if (basket.DeliveryMethodId.HasValue)
			{
				var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(basket.DeliveryMethodId.Value) ??
					throw new Exception("Invalid Delivery Method!");
				shippingCost = deliveryMethod.Cost;
				basket.ShippingPrice = shippingCost;
			}
			else
				throw new Exception("No Delivery Method Has Been Selected!");


			StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
			var paymentIntentService = new PaymentIntentService();
			PaymentIntent paymentIntent;
			if (string.IsNullOrEmpty(basket.PaymentIntentId))
			{
				var createOptions = new PaymentIntentCreateOptions()
				{
					Amount = (long)(subTotal + shippingCost) * 100,
					Currency = "usd",
					PaymentMethodTypes = new List<string>() { "card" }
				};
				paymentIntent = await paymentIntentService.CreateAsync(createOptions);
				basket.PaymentIntentId = paymentIntent.Id;
				basket.ClientSecret = paymentIntent.ClientSecret;
			}
			else
			{
				var updateOptions = new PaymentIntentUpdateOptions()
				{
					Amount = (long)(subTotal + shippingCost) * 100
				};
				paymentIntent = await paymentIntentService.UpdateAsync(basket.PaymentIntentId, updateOptions);
			}

			return await _basketRepo.CreateOrUpdateBasketAsync(basket);
		}
	}
}
