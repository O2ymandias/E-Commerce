using ECommerce.Core.Interfaces.Services.Contract;
using ECommerce.Service.Helpers;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.Service
{
	public class CacheResponseService : ICacheResponseService
	{
		private readonly IDatabase _database;
		public CacheResponseService(IConnectionMultiplexer connection)
			=> _database = connection.GetDatabase();

		public async Task<bool> SetCacheResponseAsync(string cacheKey, object response, TimeSpan expiryTime)
		{
			if (response is null)
				return false;

			var serializedResponse = JsonSerializer.Serialize(response, CustomJsonSerializerOptions.Options);
			return await _database.StringSetAsync(cacheKey, serializedResponse, expiryTime);
		}

		public async Task<string?> GetCacheResponseAsync(string cacheKey)
		{
			var response = await _database.StringGetAsync(cacheKey);
			return response.IsNullOrEmpty
				? null
				: response.ToString();
		}

	}
}
