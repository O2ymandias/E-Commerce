using ECommerce.Core.Constants;
using ECommerce.Core.Entities.IdentityModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashboard.Controllers.ApiControllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = Permissions.Users.Delete)]
	public class UsersController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<UsersController> _logger;

		public UsersController(UserManager<ApplicationUser> userManager, ILogger<UsersController> logger)
		{
			_userManager = userManager;
			_logger = logger;
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user is null)
				return NotFound();

			var result = await _userManager.DeleteAsync(user);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
					_logger.LogError("Error: {Description}", error.Description);
				return BadRequest();
			}

			_logger.LogInformation("Message: User '{UserName}' has been deleted successfully.", user.UserName);
			return Ok();
		}
	}
}
