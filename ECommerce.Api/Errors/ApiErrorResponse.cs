
namespace ECommerce.Api.Errors
{
	public class ApiErrorResponse
	{
		public ApiErrorResponse(int statusCode, string? message = null)
		{
			StatusCode = statusCode;
			Message = message ?? GenerateMessageBasedOnStatusCode(statusCode);
		}


		public int StatusCode { get; set; }
		public string? Message { get; set; }

		private static string? GenerateMessageBasedOnStatusCode(int statusCode)
		{
			return statusCode switch
			{
				StatusCodes.Status400BadRequest => "Bad Request, You've Made",
				StatusCodes.Status401Unauthorized => "Unauthorized, You Are",
				StatusCodes.Status403Forbidden => "You're Forbidden",
				StatusCodes.Status404NotFound => "Resource Is Not Found",
				StatusCodes.Status500InternalServerError => "Internal Server Error",
				_ => null
			};
		}
	}
}
