namespace ECommerce.Api.Errors
{
	public class ApiExceptionErrorResponse : ApiErrorResponse
	{
		public ApiExceptionErrorResponse(string? message = null, string? details = null)
			: base(StatusCodes.Status500InternalServerError, message)
		{
			Details = details;
		}
		public string? Details { get; set; }
	}
}
