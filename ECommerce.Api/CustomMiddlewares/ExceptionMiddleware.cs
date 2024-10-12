
using ECommerce.Api.Errors;

namespace ECommerce.Api.CustomMiddlewares
{
	public class ExceptionMiddleware : IMiddleware
	{
		private readonly IWebHostEnvironment _environment;
		private readonly ILogger<ExceptionMiddleware> _logger;

		public ExceptionMiddleware(IWebHostEnvironment environment, ILogger<ExceptionMiddleware> logger)
		{
			_environment = environment;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			try
			{
				await next.Invoke(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);

				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				context.Response.ContentType = "application/json";

				if (_environment.IsDevelopment())
					await context.Response.WriteAsJsonAsync(new ApiExceptionErrorResponse(ex.Message, ex.StackTrace));

				else
					await context.Response.WriteAsJsonAsync(new ApiErrorResponse(StatusCodes.Status500InternalServerError));
			}
		}
	}
}
