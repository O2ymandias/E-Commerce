namespace ECommerce.Core.Interfaces.Services.Contract
{
	public interface ICacheResponseService
	{
		Task<bool> SetCacheResponseAsync(string cacheKey, object response, TimeSpan expiryTime);
		Task<string?> GetCacheResponseAsync(string cacheKey);
	}
}
