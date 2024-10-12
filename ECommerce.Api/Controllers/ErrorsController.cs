using ECommerce.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
	[Route("Errors")]
	[ApiController]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class ErrorsController : ControllerBase
	{
		[HttpGet("{statusCode}")]
		public IActionResult Error(int statusCode)
			=> StatusCode(statusCode, new ApiErrorResponse(statusCode));
	}
}
