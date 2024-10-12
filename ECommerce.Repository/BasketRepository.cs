using ECommerce.Core.Entities.BasketModule;
using ECommerce.Core.Interfaces.Repositories.Contract;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.Repository
{
	public class BasketRepository : IBasketRepository
	{
		private const int _daysBeforeExpiry = 7;
		private readonly IDatabase _database;
		public BasketRepository(IConnectionMultiplexer connection)
		{
			_database = connection.GetDatabase();
		}
		public async Task<Basket?> CreateOrUpdateBasketAsync(Basket basket)
		{
			var serializedBasket = JsonSerializer.Serialize(basket);
			var isSetSuccessfully = await _database.StringSetAsync(basket.Id, serializedBasket, TimeSpan.FromDays(_daysBeforeExpiry));
			return isSetSuccessfully
				? await GetBasketAsync(basket.Id)
				: null;
		}
		public async Task<Basket?> GetBasketAsync(string basketId)
		{
			var basket = await _database.StringGetAsync(basketId);
			return basket.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Basket>(basket!);
		}

		public async Task<bool> DeleteBasketAsync(string basketId)
			=> await _database.KeyDeleteAsync(basketId);

	}
}
