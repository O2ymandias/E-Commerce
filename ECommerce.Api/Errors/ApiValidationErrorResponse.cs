namespace ECommerce.Api.Errors
{
	public class ApiValidationErrorResponse : ApiErrorResponse
	{
		public ApiValidationErrorResponse()
			: base(StatusCodes.Status400BadRequest)
		{
			Errors = new List<string>();
		}
		public List<string> Errors { get; set; }
	}
}
