using ECommerce.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashboard.Controllers.ApiControllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = Permissions.Roles.Delete)]
	public class RolesController : ControllerBase
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ILogger<RolesController> _logger;

		public RolesController(RoleManager<IdentityRole> roleManager, ILogger<RolesController> logger)
		{
			_roleManager = roleManager;
			_logger = logger;
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] string id)
		{
			var role = await _roleManager.FindByIdAsync(id);
			if (role is null)
				return NotFound();
			var result = await _roleManager.DeleteAsync(role);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
					_logger.LogError("Error: {Description}", error.Description);
				return BadRequest();
			}

			_logger.LogInformation("Message: Role '{Name}' has been deleted successfully.", role.Name);
			return Ok();
		}
	}
}
