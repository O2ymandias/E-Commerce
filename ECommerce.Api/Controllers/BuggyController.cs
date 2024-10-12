using ECommerce.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BuggyController : ControllerBase
	{
		[HttpGet("NotFound")]
		public IActionResult GetNotFoundResponse()
			=> NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound));

		[HttpGet("ServerError")]
		public IActionResult GetServerErrorResponse()
			=> throw new NullReferenceException();

		[HttpGet("BadRequest")]
		public IActionResult GetBadRequestResponse()
			=> BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));

		[HttpGet("BadRequest/{id}")]
		public IActionResult GetValidationErrorResponse(int id)
			=> Ok();

	}
}
