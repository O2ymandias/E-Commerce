using ECommerce.Core.Entities.BasketModule;

namespace ECommerce.Core.Interfaces.Services.Contract
{
	public interface IPaymentService
	{
		Task<Basket?> CreateOrUpdatePaymentIntent(string basketId);
	}
}
