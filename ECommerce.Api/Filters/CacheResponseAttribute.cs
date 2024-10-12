using ECommerce.Core.Interfaces.Services.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace ECommerce.Api.Filters
{
	public class CacheResponseAttribute : ActionFilterAttribute
	{
		private readonly int _minutesBeforeExpiry;

		public CacheResponseAttribute(int minutesBeforeExpiry)
			=> _minutesBeforeExpiry = minutesBeforeExpiry;

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var cacheResponseService = context.HttpContext.RequestServices.GetRequiredService<ICacheResponseService>();
			var cacheKey = GenerateCacheKeyBasedOnRequest(context.HttpContext.Request);

			var cachedResponse = await cacheResponseService.GetCacheResponseAsync(cacheKey);

			if (!string.IsNullOrEmpty(cachedResponse))
			{
				context.Result = new ContentResult()
				{
					Content = cachedResponse,
					ContentType = "application/json",
					StatusCode = StatusCodes.Status200OK
				};
				return;
			}

			var actionExecutedContext = await next.Invoke();
			if (actionExecutedContext.Result is OkObjectResult okResult && okResult.Value is not null)
				await cacheResponseService.SetCacheResponseAsync(cacheKey, okResult.Value, TimeSpan.FromMinutes(_minutesBeforeExpiry));
		}

		private static string GenerateCacheKeyBasedOnRequest(HttpRequest request)
		{
			var cacheKeyBuilder = new StringBuilder();
			cacheKeyBuilder.Append(request.Path);

			foreach (var kvp in request.Query.OrderBy(kvp => kvp.Key))
				cacheKeyBuilder.Append($"|{kvp.Key}={kvp.Value}");

			return cacheKeyBuilder.ToString();
		}
	}
}
