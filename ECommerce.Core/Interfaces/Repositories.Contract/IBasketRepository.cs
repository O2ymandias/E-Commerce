using ECommerce.Core.Entities.BasketModule;

namespace ECommerce.Core.Interfaces.Repositories.Contract
{
	public interface IBasketRepository
	{
		Task<Basket?> CreateOrUpdateBasketAsync(Basket basket);
		Task<Basket?> GetBasketAsync(string basketId);
		Task<bool> DeleteBasketAsync(string basketId);
	}
}
