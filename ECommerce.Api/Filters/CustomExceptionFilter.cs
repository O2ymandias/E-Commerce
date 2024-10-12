using ECommerce.Api.Errors;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommerce.Api.Filters
{
	public class CustomExceptionFilter : IExceptionFilter
	{
		private readonly ILogger<CustomExceptionFilter> _logger;
		private readonly IWebHostEnvironment _env;

		public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger,
			IWebHostEnvironment env)
		{
			_logger = logger;
			_env = env;
		}
		public void OnException(ExceptionContext context)
		{
			_logger.LogError(context.Exception, context.Exception.Message);

			var response = context.HttpContext.Response;
			response.ContentType = "application/json";
			response.StatusCode = StatusCodes.Status500InternalServerError;

			if (_env.IsDevelopment())
				response.WriteAsJsonAsync(new ApiExceptionErrorResponse(context.Exception.Message, context.Exception.StackTrace));
			else
				response.WriteAsJsonAsync(new ApiErrorResponse(StatusCodes.Status500InternalServerError));

			context.ExceptionHandled = true;
		}
	}
}
